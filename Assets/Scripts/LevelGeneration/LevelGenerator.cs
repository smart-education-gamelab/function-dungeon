using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

    public readonly List<string> learningGoalLevels = new List<string>() { "1.1", "1.2", "1.3", "1.4", "2.1", "2.2", "2.3", "2.4", "2.5", "2.6", "2.8", "3.1", "3.2", "3.3", "3.4", "3.5" };
    public int minLearningGoalLevel = 0;
    public int maxLearningGoalLevel = 15;

    private int generationAttempts = 0;
    private bool visualize = true;

    [SerializeField] public int lastGenerationWidth = 32;
    [SerializeField] public int lastGenerationHeight = 32;

    [SerializeField] public int xMin, xMax, yMin, yMax;
    [SerializeField] public int xGenerationOffset, yGenerationOffset;

    public List<Recipe> recipes = new List<Recipe>();
    public List<Symbol> alphabet = new List<Symbol>();
    public Dictionary<char, Symbol> alphabetLookupTable = new Dictionary<char, Symbol>();
    public int width = 32;
    public int height = 32;
    public Tilemap layoutTileMap, ceilingTileMap, npc1TileMap, npc2TileMap;

    // Array of tile definitions used during level generation. This keeps track of symbols and extra data that can't be stored in the tile maps.
    public TileDefinition[] tiles = null;
    public TileBase npcTile;

    public TileDefinition GetTile(int x, int y) {
        if (x < 0 || x >= lastGenerationWidth || y < 0 || y >= lastGenerationHeight) return null;
        return tiles[y * lastGenerationWidth + x];
    }

    public void Start() {
        UpdateAlphabetLookupTable();
        UpdateMap();

        if (newLevelOnPlay) Generate(false);
        else LoadingScreenManager.BlackScreenFadeOut(2.0f);
    }

    private void UpdateAlphabetLookupTable() {
        alphabetLookupTable = LevelGenerationUtils.ListToDictionary(alphabet, "LevelGenerator", x => x.character);
        foreach (Recipe recipe in recipes) {
            foreach (Grammar grammar in recipe.grammars) {
                grammar.GenerateVariations();
            }
        }
    }

    IEnumerator GenerateCoroutine() {
        lastGenerationWidth = width;
        lastGenerationHeight = height;

        UpdateAlphabetLookupTable();
        layoutTileMap.ClearAllTiles();
        ceilingTileMap.ClearAllTiles();
        npc1TileMap.ClearAllTiles();
        npc2TileMap.ClearAllTiles();

        //Initialize tiles array and set first hook tile to start generating from.
        tiles = new TileDefinition[lastGenerationWidth * lastGenerationHeight];
        for (int y = 0; y < lastGenerationHeight; y++) {
            for (int x = 0; x < lastGenerationWidth; x++) {
                tiles[y * lastGenerationWidth + x] = new TileDefinition('s', x, y);
            }
        }
        SetTile((int)lastGenerationWidth / 2, (int)lastGenerationHeight / 2, 'h');
        xMin = lastGenerationWidth / 2;
        xMax = lastGenerationWidth / 2;
        yMin = lastGenerationHeight / 2;
        yMax = lastGenerationHeight / 2;

        Dictionary<string, int> redirectionCounts = new Dictionary<string, int>();

        // Process each recipe in the list while respecting redirections.
        int nextRecipe = 0;
        while (nextRecipe < recipes.Count) {
            LoadingScreenManager.SetLogText($"Level generation attempt #{generationAttempts} RecipeIndex:{nextRecipe + 1}|{recipes.Count}");
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
                    if (!ExecuteGrammar(recipe, executionCount)) break;
                    else executionCount++;
                    UpdateMap();
                }
                if (recipe.minTimesToExecute != -1 && executionCount < recipe.minTimesToExecute) Restart();
            } else if (recipe.type == RecipeType.CUSTOMGRAMMAR) {
                int executionCount = 0;
                int maxExecutions = recipe.maxTimesToExecute == -1 ? 1000 : recipe.maxTimesToExecute;
                for (int i = 0; i < maxExecutions; i++) {
                    yield return null;
                    if (!ExecuteCustomGrammar(recipe, executionCount)) break;
                    else executionCount++;
                    UpdateMap();
                }
                if (recipe.minTimesToExecute != -1 && executionCount < recipe.minTimesToExecute) Restart();
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
        Time.timeScale = 1;
        LoadingScreenManager.BlackScreenFadeOut(2.0f);

        if (Application.isPlaying) {
            FindObjectOfType<MathSetup>().Awake();

            foreach (SetDoors setDoors in FindObjectsOfType<SetDoors>()) {
                setDoors.Awake();
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

    public void Generate(bool shouldVisualize) {
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

    private void Restart() {
        generationAttempts++;

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

    bool ExecuteGrammar(Recipe recipe, int executionCount) {
        //Loop through all grammars in a recipe and check if they can be placed.
        ConcurrentBag<PotentialGrammar> potentialGrammars = new ConcurrentBag<PotentialGrammar>();

        void Loop(int y) {
            for (int x = 0; x < lastGenerationWidth; x++) {
                foreach (Grammar grammar in recipe.grammars) {
                    if (!grammar.enabled) continue;
                    for (int variation = 0; variation < grammar.patternVariations.Count; variation++) {
                        if (CheckGrammar(x, y, grammar, variation)) {
                            potentialGrammars.Add(new PotentialGrammar(grammar, x, y, variation, executionCount));
                        }
                    }
                }
            }
        }

#if UNITY_WEBGL
        for (int y = 0; y < lastGenerationHeight; y++) Loop(y);
#else
            Parallel.For(0, lastGenerationHeight, y => Loop(y));
#endif

        //Select a random grammar that can be placed.
        if (potentialGrammars.Count == 0) return false;
        int random = UnityEngine.Random.Range(0, potentialGrammars.Count);
        PotentialGrammar randomGrammar = potentialGrammars.ToList()[random];
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
                    if (tile.character != 's') {
                        xMin = Mathf.Min(xMin, x);
                        xMax = Mathf.Max(xMax, x);
                        yMin = Mathf.Min(yMin, y);
                        yMax = Mathf.Max(yMax, y);
                    }
                }
                GrammarPlacedEvent gpEvent = new GrammarPlacedEvent(GetTile(x + xx, y + yy), character, executionCount, rewritePattern);
                grammar.patternPlacedCallback.Invoke(this, gpEvent);

                if (gpEvent.newCharacter != character) {
                    newTerminal = alphabetLookupTable[gpEvent.newCharacter];
                    tile.character = newTerminal.character;
                }
            }
        }
    }

    void SetTile(int x, int y, char tileType) {
        tiles[y * lastGenerationWidth + x].character = tileType;
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
                TileDefinition tile = tiles[y * lastGenerationWidth + x];
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

#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (tiles == null) return;
        GUIStyle style = GUI.skin.textField;
        style.alignment = TextAnchor.MiddleCenter;


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