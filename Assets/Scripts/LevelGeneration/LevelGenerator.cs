using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using Mono.Cecil;


#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

public class LevelGenerator : MonoBehaviour {
#if UNITY_EDITOR
    private EditorCoroutine generateCoroutine = null;
#else
    private Coroutine generateCoroutine = null;
#endif

    public bool newLevelOnPlay = false;

    public List<LearningGoalSectionDefinition> learningGoalSections = new List<LearningGoalSectionDefinition>();

    private int generationAttempts = 0;
    private bool visualize = true;

    public static int maxGrammarSize = 7;
    private int generationSeed = 0;
    [SerializeField] public int lastGenerationWidth = 32;
    [SerializeField] public int lastGenerationHeight = 32;

    [SerializeField] public int xMin, xMax, yMin, yMax; //Used during generation to know the min and max positions at which any tiles have been changed
    [SerializeField] public int realXMin, realXMax, realYMin, realYMax; //Used to find the min and max size of the generated level
    [SerializeField] public int xGenerationOffset, yGenerationOffset;

    public List<Recipe> recipes = new List<Recipe>();
    public List<Symbol> alphabet = new List<Symbol>();
    public Dictionary<char, Symbol> alphabetLookupTable = new Dictionary<char, Symbol>();
    public int width = 32;
    public int height = 32;
    public Tilemap layoutTileMap, ceilingTileMap, npc1TileMap, npc2TileMap;

    // Array of tile definitions used during level generation. This keeps track of symbols and extra data that can't be stored in the tile maps.
    private TileDefinition[,] tiles2D;
    public TileBase npcTile;

    [SerializeField] private GenerationVariables variables;

    public TileDefinition GetTile(int x, int y) {
        if (x < 0 || x >= lastGenerationWidth || y < 0 || y >= lastGenerationHeight) return null;
        return tiles2D[x, y];
    }

    public void Start() {
        UpdateAlphabetLookupTable();
        CalculateLevelDimensions();

        //UpdateMap();

        if (newLevelOnPlay || (Globals.IsInitialized() && Globals.LevelGenerationVariables != null)) Generate((int)Time.time, false, Globals.LevelGenerationVariables);
        else if (Application.isPlaying) {
            FindObjectOfType<PreviewCamera>(true).GeneratePreview();
            Globals.UIManager.BlackScreenFadeOut(2.0f);
        }
    }

    private void UpdateAlphabetLookupTable() {
        alphabetLookupTable = LevelGenerationUtils.ListToDictionary(alphabet, "LevelGenerator", x => x.character);
        foreach (Recipe recipe in recipes) {
            if (!recipe.enabled) continue;
            foreach (Grammar grammar in recipe.grammars) {
                grammar.GenerateVariations();
            }
        }
    }

    IEnumerator GenerateCoroutine() {
#if UNITY_EDITOR
        if (!Application.isPlaying) EditorSceneManager.MarkAllScenesDirty();
#endif
        UnityEngine.Random.InitState(generationSeed + generationAttempts);
        lastGenerationWidth = width;
        lastGenerationHeight = height;

        UpdateAlphabetLookupTable();
        layoutTileMap.ClearAllTiles();
        ceilingTileMap.ClearAllTiles();
        npc1TileMap.ClearAllTiles();
        npc2TileMap.ClearAllTiles();

        //Initialize tiles array and set first hook tile to start generating from.
        tiles2D = new TileDefinition[lastGenerationWidth, lastGenerationHeight];
        for (int y = 0; y < lastGenerationHeight; y++) {
            for (int x = 0; x < lastGenerationWidth; x++) {
                tiles2D[x, y] = new TileDefinition('s', x, y);
            }
        }
        SetTile((int)lastGenerationWidth / 2, (int)lastGenerationHeight / 2, 'h');
        xMin = lastGenerationWidth / 2 - 1;
        xMax = lastGenerationWidth / 2 + 1;
        yMin = lastGenerationHeight / 2 - 1;
        yMax = lastGenerationHeight / 2 + 1;

        Dictionary<string, int> redirectionCounts = new Dictionary<string, int>();

        // Process each recipe in the list while respecting redirections.
        int nextRecipe = 0;
        while (nextRecipe < recipes.Count) {
            //LoadingScreenManager.SetLogText($"Level generation attempt #{generationAttempts} RecipeIndex:{nextRecipe + 1}|{recipes.Count}");
            Recipe recipe = recipes[nextRecipe];
            nextRecipe++;
            if (!recipe.enabled) continue;
            recipe.onRecipeStartCallback?.Invoke(this);

            // Process recipe based on its type
            if (recipe.type == RecipeType.GRAMMAR) {
                int executionCount = 0;
                int maxExecutions = recipe.maxTimesToExecute == -1 ? 1000 : recipe.maxTimesToExecute;
                for (int i = 0; i < maxExecutions; i++) {
                    yield return null;
                    if (!ExecuteRecipe(recipe, executionCount)) break;
                    else executionCount++;
                    UpdateMap();
                }
                if (recipe.minTimesToExecute != -1 && executionCount < recipe.minTimesToExecute) {
                    Restart($"Recipe '{recipe.name}' didn't execute enough times");
                }
            } else if (recipe.type == RecipeType.CUSTOMGRAMMAR) {
                int executionCount = 0;
                int maxExecutions = recipe.maxTimesToExecute == -1 ? 1000 : recipe.maxTimesToExecute;
                for (int i = 0; i < maxExecutions; i++) {
                    yield return null;
                    if (!ExecuteCustomGrammar(recipe, executionCount)) break;
                    else executionCount++;
                    UpdateMap();
                }
                if (recipe.minTimesToExecute != -1 && executionCount < recipe.minTimesToExecute) {
                    Restart($"Recipe '{recipe.name}' didn't execute enough times");
                    continue;
                }
            } else if (recipe.type == RecipeType.REDIRECTION) {
                if (!redirectionCounts.ContainsKey(recipe.name)) {
                    redirectionCounts[recipe.name] = 0;
                }
                redirectionCounts[recipe.name]++;
                if (redirectionCounts[recipe.name] > recipe.amountOfRedirections) continue;

                int index = recipes.FindIndex(x => x.name == recipe.redirectionName);
                if (index == -1) throw new Exception($"Recipe {recipe.name} redirection {recipe.redirectionName} not found!");
                nextRecipe = index;
            }
            recipe.onRecipeEndCallback?.Invoke(this);
        }

        UpdateMap(true);
        CalculateLevelDimensions();
        Time.timeScale = 1;
        Debug.Log("Succesfully generated level");
        if (Application.isPlaying) {
            Globals.UIManager.BlackScreenFadeOut(2.0f);
            FindObjectOfType<PreviewCamera>(true).GeneratePreview();
            FindObjectOfType<MathSetup>().Awake();

            foreach (SetDoors setDoors in FindObjectsOfType<SetDoors>()) {
                setDoors.Awake();
            }
        }
    }

    private void CalculateLevelDimensions() {
        for (int y = 0; y < layoutTileMap.size.x; y++) {
            for (int x = 0; x < layoutTileMap.size.y; x++) {
                if (layoutTileMap.GetTile(new Vector3Int(x, y, 0)).name != "Dungeon Background") {
                    realXMin = Mathf.Min(xMin, x);
                    realXMax = Mathf.Max(xMax, x);
                    realYMin = Mathf.Min(yMin, y);
                    realYMax = Mathf.Max(yMax, y);
                }
            }
        }
    }

    private void StartCoroutine() {
#if UNITY_EDITOR
        generateCoroutine = EditorCoroutineUtility.StartCoroutine(GenerateCoroutine(), this);
#else
        generateCoroutine =  StartCoroutine(GenerateCoroutine());
#endif
    }

    private void StopCoroutine() {
#if UNITY_EDITOR
        if (generateCoroutine != null) EditorCoroutineUtility.StopCoroutine(generateCoroutine);
#else
        if (generateCoroutine != null) StopCoroutine(generateCoroutine);
#endif
    }

    public void Generate(int seed, bool shouldVisualize, GenerationVariables variables = null) {
        this.variables = variables;
        generationSeed = seed;
        if (variables != null) SetVariables(variables);
        Time.timeScale = 0;
        visualize = shouldVisualize;
        generationAttempts = 0;

        StopCoroutine();
        StartCoroutine();
    }

    public void StopGenerating() {
        StopCoroutine();
        Time.timeScale = 1;
    }

    private void Restart(string reason) {
        generationAttempts++;
        Debug.LogWarning("Level Generation failed because: " + reason);
#if UNITY_EDITOR
        if (generationAttempts > 10) {
            Debug.LogError("Level Generation attemts exceeded 10. Failed to generate level.");
            Time.timeScale = 1;
            return;
        }
#endif

        StopCoroutine();
        StartCoroutine();
    }

    bool ExecuteCustomGrammar(Recipe recipe, int executionCount) {
        if (recipe.customGrammars.Count == 0) return false;
        bool toReturn = true;
        foreach (int y in Enumerable.Range(0, lastGenerationHeight).OrderBy(x => UnityEngine.Random.Range(-1000, 1000))) {
            foreach (int x in Enumerable.Range(0, lastGenerationWidth).OrderBy(x => UnityEngine.Random.Range(-1000, 1000))) {
                foreach (var customGrammar in recipe.customGrammars) {
                    CustomGrammarEvent e = new CustomGrammarEvent(GetTile(x, y));
                    customGrammar.Invoke(this, e);
                    if (e.newSymbol != e.tile.character) {
                        SetTile(x, y, e.newSymbol);
                    }
                    if (e.shouldStop) toReturn = false;
                }
            }
        }
        return toReturn;
    }

    public Recipe GetRecipe(string name) {
        return recipes.Find(x => x.name == name);
    }

    struct PotentialGrammar {
        public Grammar grammar;
        public int x, y;
        public int rotation;
        public int executionCount;
        public PotentialGrammar(Grammar grammar, int x, int y, int rotation, int executionCount) {
            this.grammar = grammar;
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.executionCount = executionCount;
        }
    }

    bool ExecuteRecipe(Recipe recipe, int executionCount) {
        //Loop through all grammars in a recipe and check if they can be placed.
        List<PotentialGrammar> potentialGrammars = new List<PotentialGrammar>();
        int maxGrammarSizePlusOne = maxGrammarSize + 10;
        void Loop(int y) {
            for (int x = xMin - maxGrammarSizePlusOne; x < xMax + maxGrammarSizePlusOne; x++) {
                foreach (Grammar grammar in recipe.grammars) {
                    if (!grammar.enabled) continue;
                    if (UnityEngine.Random.Range(0f, 1f) < grammar.chanceToSkip) continue;
                    for (int variation = 0; variation < grammar.patternVariations.Count; variation++) {
                        if (CheckGrammar(x, y, grammar, variation)) {
                            potentialGrammars.Add(new PotentialGrammar(grammar, x, y, variation, executionCount));
                        }
                    }
                }
            }
        }

        //#if UNITY_WEBGL
        for (int y = yMin - maxGrammarSizePlusOne; y < yMax + maxGrammarSizePlusOne; y++) Loop(y);
        //#else
        //            Parallel.For(0, lastGenerationHeight, y => Loop(y));
        //#endif

        //Select a random grammar that can be placed.
        if (potentialGrammars.Count == 0) return false;
        int random = UnityEngine.Random.Range(0, potentialGrammars.Count);
        PotentialGrammar randomGrammar = potentialGrammars[random];
        ApplyGrammar(randomGrammar.x, randomGrammar.y, randomGrammar.grammar, randomGrammar.rotation, randomGrammar.executionCount);
        return true;
    }

    bool CheckGrammar(int x, int y, Grammar grammar, int variation) {
        int w = grammar.patternVariations[variation].width;
        int h = grammar.patternVariations[variation].height;
        for (int yy = 0; yy < h; yy++) {
            for (int xx = 0; xx < w; xx++) {
                int index = (yy * w) + xx;
                Symbol terminal = alphabetLookupTable[grammar.patternVariations[variation].pattern[index]];
                if (terminal.type == SymbolType.WILDCARD) continue;
                TileDefinition tile = GetTile(x + xx, y + yy);
                if (tile == null) return false;
                if (terminal.character != alphabetLookupTable[tile.character].character) {
                    return false;
                }
            }
        }
        return true;
    }

    void ApplyGrammar(int x, int y, Grammar grammar, int variation, int executionCount) {
        int w = grammar.patternVariations[variation].width;
        int h = grammar.patternVariations[variation].height;
        string rewritePattern = grammar.patternVariations[variation].rewritePattern;
        for (int yy = 0; yy < h; yy++) {
            for (int xx = 0; xx < w; xx++) {
                TileDefinition tile = GetTile(x + xx, y + yy);
                if (tile == null) continue;
                int index = (yy * w) + xx;
                char character = rewritePattern[index];
                Symbol newTerminal = alphabetLookupTable[character];
                if (newTerminal.type != SymbolType.WILDCARD) {
                    tile.character = newTerminal.character;
                    //if (tile.character != 's') {
                    //
                    //}
                }
                TileDefinition tileDefinition = GetTile(x + xx, y + yy);
                xMin = Mathf.Min(xMin, tileDefinition.x);
                xMax = Mathf.Max(xMax, tileDefinition.x);
                yMin = Mathf.Min(yMin, tileDefinition.y);
                yMax = Mathf.Max(yMax, tileDefinition.y);
                GrammarPlacedEvent gpEvent = new GrammarPlacedEvent(tileDefinition, character, executionCount, rewritePattern);
                grammar.patternPlacedCallback.Invoke(this, gpEvent);

                if (gpEvent.newCharacter != character) {
                    newTerminal = alphabetLookupTable[gpEvent.newCharacter];
                    tile.character = newTerminal.character;
                }
            }
        }
    }

    void SetTile(int x, int y, char tileType) {
        tiles2D[x, y].character = tileType;
        UpdateMap();
    }

    void UpdateMap(bool final = false) {
        if (!visualize && !final) return;
        xGenerationOffset = ((xMax + xMin) / 2) - lastGenerationWidth / 2;
        yGenerationOffset = ((yMax + yMin) / 2) - lastGenerationHeight / 2;
        xGenerationOffset = 0; //Comment out for centered levels
        yGenerationOffset = 0; //Comment out for centered levels

        for (int y = 0; y < lastGenerationHeight; y++) {
            for (int x = 0; x < lastGenerationWidth; x++) {
                if (x - xGenerationOffset < 0 || x - xGenerationOffset > lastGenerationWidth) continue;
                if (y - yGenerationOffset < 0 || y - yGenerationOffset > lastGenerationHeight) continue;
                TileDefinition tile = GetTile(x, y);
                char tileChar = tile.character;
                if (alphabetLookupTable.TryGetValue(tileChar, out Symbol tileTypePair)) {
                    layoutTileMap.SetTile(new Vector3Int(x - xGenerationOffset, y - yGenerationOffset, 0), tileTypePair.tileBase);

                    if (tileTypePair.character == 's' || tileTypePair.character == 'n') {
                        ceilingTileMap.SetTile(new Vector3Int(x - xGenerationOffset, y - yGenerationOffset, 0), tileTypePair.tileBase);
                        if (tileTypePair.character == 's')
                            ceilingTileMap.SetColliderType(new Vector3Int(x, y), Tile.ColliderType.Grid);
                    } else {
                        ceilingTileMap.SetTile(new Vector3Int(x - xGenerationOffset, y - yGenerationOffset, 0), null);
                    }
                } else throw new KeyNotFoundException($"Tile definition for {tileChar} not found");
            }
        }
    }

    public bool IsRedirectValid(Recipe recipe) {
        return recipes.Any(x => x.name == recipe.redirectionName);
    }

    public void SetNPCOneRequestTile(int x, int y) {
        npc1TileMap.SetTile(new Vector3Int(x, y, 0), npcTile);
    }

    public void SetNPCTwoRequestTile(int x, int y) {
        npc2TileMap.SetTile(new Vector3Int(x, y, 0), npcTile);
    }

    public GenerationVariables GetGenerationVariables() {
        return variables;
    }

    private void SetVariables(GenerationVariables variables) {
        generationSeed = variables.seed;

        Recipe roomsAmountRecipe = GetRecipe("Redirect back to Alleys");
        roomsAmountRecipe.amountOfRedirections = variables.amountOfRooms - 2;
        Recipe cleanupRecipe = GetRecipe("Cleanup");
        cleanupRecipe.maxTimesToExecute = (100 - (int)variables.deadEnds) / 2;

        float complexity = (float)variables.complexity / 100f;
        Recipe alleysRecipe = GetRecipe("Alleys");
        Grammar alleysLinearGrammar = alleysRecipe.GetGrammar("Alleys linear");
        alleysLinearGrammar.chanceToSkip = complexity / 2.0f;
        Grammar alleysComplexGrammar = alleysRecipe.GetGrammar("Alleys complex");
        alleysComplexGrammar.chanceToSkip = 1.0f - complexity;

        Recipe reservedSpacesRecipe = GetRecipe("Reserved spaces");
        Grammar reservedSpacesLinearRecipe = reservedSpacesRecipe.GetGrammar("Reserved spaces linear");
        reservedSpacesLinearRecipe.chanceToSkip = complexity / 2.0f;
        Grammar reservedSpacesComplexRecipe = reservedSpacesRecipe.GetGrammar("Reserved spaces complex");
        reservedSpacesComplexRecipe.chanceToSkip = 1.0f - complexity;

        learningGoalSections.Clear();
        foreach (LearningGoalSectionDefinition learningGoalSection in variables.learningGoalSections) {
            learningGoalSections.Add((LearningGoalSectionDefinition)learningGoalSection.Clone());
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (tiles2D == null) return;
        GUIStyle style = GUI.skin.textField;
        style.alignment = TextAnchor.MiddleCenter;

        //Handles.DrawSolidRectangleWithOutline(new Rect(xMin, yMin, xMax - xMin, yMax - yMin), UnityEngine.Color.white, UnityEngine.Color.red);

        for (int y = 0; y < lastGenerationHeight; y++) {
            for (int x = 0; x < lastGenerationWidth; x++) {
                TileDefinition tile = GetTile(x, y);
                if (tile != null && tile.extraData.information.Length != 0) {
                    Handles.Label(layoutTileMap.transform.position + new UnityEngine.Vector3(x + 0.5f + xGenerationOffset, y + 0.5f + yGenerationOffset), $"{tile.extraData.information}", style);
                }
                //if (tile != null) {
                //    Handles.Label(layoutTileMap.transform.position + new UnityEngine.Vector3(x + 0.5f + xGenerationOffset, y + 0.5f + yGenerationOffset), $"{tile.character}", style);
                //}
            }
        }
        style.alignment = TextAnchor.UpperLeft;
    }
#endif
}