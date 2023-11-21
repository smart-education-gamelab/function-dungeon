using Codice.Client.BaseCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

public class LevelGeneratorWindow : EditorWindow {
    private ReorderableList learingGoalSectionsList;
    private const string levelGeneratorScenePath = "assets/Scenes/level generation.unity";
    private string learningGoalSectionsStr = "";
    private string generationString = "";
    private GenerationVariables variables = new GenerationVariables();
    private LevelGenerator levelGenerator;
    private bool readVariablesValid = true;
    private Color redColor = new Color(2f, 0.5f, 0.5f);
    private QuestionList questionList;
    private List<string> invalidSections = new List<string>();

    [MenuItem("Function Dungeon/Level Generator")]
    public static void ShowWindow() {
        GetWindow<LevelGeneratorWindow>("Level Generator");
    }

    private void OnEnable() {
        questionList = AssetDatabase.LoadAssetAtPath<QuestionList>(Constants.questionListPath);
        CreateReorderableList();
        UnityEditor.SceneManagement.EditorSceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy() {
        learingGoalSectionsList.drawElementCallback -= DrawElement;
        learingGoalSectionsList.drawHeaderCallback -= DrawHeader;
        learingGoalSectionsList.onChangedCallback -= ListChanged;
        UnityEditor.SceneManagement.EditorSceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1) {
        Repaint();
    }

    void OnGUI() {
        bool isLevelGeneratorScene = EditorSceneManager.GetActiveScene().path.Equals(levelGeneratorScenePath, StringComparison.OrdinalIgnoreCase);
        if (!isLevelGeneratorScene) {
            EditorGUILayout.LabelField("Please load the level generator scene before generating a level");
            if (GUILayout.Button("Open level generator scene")) {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    EditorSceneManager.OpenScene(levelGeneratorScenePath);
                }
            }
            return;
        }

        levelGenerator = GameObject.FindObjectOfType<LevelGenerator>();

        if (levelGenerator == null) {
            EditorGUILayout.LabelField("LevelGenerator object could not be found in scene.");
            return;
        }

        EditorGUILayout.LabelField(new GUIContent("Gameplay variables", "Variables that will change the gameplay value of the generated level"), EditorStyles.boldLabel);

        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.BeginChangeCheck();
        variables.amountOfRooms = EditorGUILayout.IntSlider(new GUIContent("Amount of rooms", "The amount of rooms/questions that will be generated."), variables.amountOfRooms, 2, 10);
        variables.seed = EditorGUILayout.IntSlider(new GUIContent("Generation seed", "The seed determines how a level is generated. If you always use the same seed and the same settings, you will always generate the same world."), variables.seed, 10000, 99999);
        variables.complexity = (int)Utils.RangeSliderWithLabels(new GUIContent("Complexity", "Linear is a straight path from start to end, Maze has a lot of branching."), "Linear", "Maze", variables.complexity, 0, 50);
        variables.deadEnds = (int)Utils.RangeSliderWithLabels(new GUIContent("Dead ends", "Influences how many alleys will result in dead ends."), "None ", "Many", variables.deadEnds, 0, 100);
        if (EditorGUI.EndChangeCheck()) {
            CheckEnoughExercises();
            InvalidateGenerationString();
        }
        EditorGUILayout.Space();
        GUILayout.EndVertical();
        Utils.GuiLine();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Educational variables", "Variables that will change the learning experience offered by the generated level"), EditorStyles.boldLabel);
        GUILayout.BeginVertical(EditorStyles.helpBox);

        variables.randomizedQuestions = EditorGUILayout.ToggleLeft(new GUIContent("Randomized questions", "If enabled, questions in rooms will randomly picked from the learning goal sections. If disabled, levels with the same seed will always generate with exactly the same questions."), variables.randomizedQuestions);
        EditorGUI.BeginChangeCheck();
        learingGoalSectionsList.DoLayoutList();
        if (EditorGUI.EndChangeCheck()) {
            CheckEnoughExercises();
            InvalidateGenerationString();
        }
        EditorGUILayout.LabelField("Selected learning goal sections: " + learningGoalSectionsStr);

        if (invalidSections.Count > 0) {
            string errorStr = "The following learning goal sections do not contain enough exercises to fill all rooms in this level:";
            foreach (string invalidSection in invalidSections) {
                errorStr += "\n" + invalidSection;
            }
            EditorGUILayout.HelpBox(errorStr, MessageType.Error);
        }
        GUILayout.EndVertical();
        EditorGUILayout.LabelField(new GUIContent("Generation string", "A string that represents the variables for the consistent generation of a level"), EditorStyles.boldLabel);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.BeginChangeCheck();


        Color color = GUI.backgroundColor;
        if (!readVariablesValid) GUI.backgroundColor = redColor;
        generationString = GUILayout.TextField(generationString);
        GUI.backgroundColor = color;
        if (EditorGUI.EndChangeCheck()) {
            CheckGenerationString();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy to clipboard", GUILayout.Width(250))) {
            GUIUtility.systemCopyBuffer = generationString;
        }
        GUILayout.FlexibleSpace();
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.alignment = TextAnchor.MiddleRight;
        if (GUILayout.Button("Read generation string", GUILayout.Width(250))) {
            ReadGenerationString();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Generate level", GUILayout.Height(30))) {
            Generate();
        }
    }

    private void InvalidateGenerationString() {
        generationString = GenerationString.Serialize(variables);
        readVariablesValid = true;
    }

    private void CheckGenerationString() {
        GenerationVariables readVariables = GenerationString.Deserialize(generationString);
        readVariablesValid = readVariables != null;
    }


    private void ReadGenerationString() {
        GenerationVariables readVariables = GenerationString.Deserialize(generationString);
        if (readVariables != null) {
            variables = (GenerationVariables)readVariables.Clone();
            learingGoalSectionsList.list = variables.learningGoalSections;
            Repaint();
        }
    }

    private void CreateReorderableList() {
        learingGoalSectionsList = new ReorderableList(variables.learningGoalSections, typeof(LearningGoalSectionDefinition), true, true, true, true);
        learingGoalSectionsList.drawElementCallback = DrawElement;
        learingGoalSectionsList.drawHeaderCallback = DrawHeader;
        learingGoalSectionsList.onChangedCallback = ListChanged;
    }

    private void CheckEnoughExercises() {
        invalidSections = GenerationVariables.CheckIfEnoughExercises(questionList, variables.learningGoalSections, variables.amountOfRooms);
    }

    private void UpdateSelectedLearningGoalSections() {
        HashSet<int> indicesSet = new HashSet<int>();
        foreach (LearningGoalSectionDefinition def in variables.learningGoalSections) {
            for (int i = def.min; i <= def.max; i++) {
                indicesSet.Add(i);
            }
        }

        if (indicesSet.Count == 0) {
            learningGoalSectionsStr = "1.1-3.5";
            return;
        }

        List<int> indices = indicesSet.ToList();

        indices.Sort();
        int start = indices[0];
        int end = indices[0];

        string result = "";
        for (int i = 1; i < indices.Count; i++) {
            if (indices[i] == end + 1) {
                end = indices[i];
            } else {
                if (start == end) {
                    result += Constants.learningGoalLevels[start] + ", ";
                } else {
                    result += Constants.learningGoalLevels[start] + "-" + Constants.learningGoalLevels[end] + ", ";
                }
                start = end = indices[i];
            }
        }

        if (start == end) {
            result += Constants.learningGoalLevels[start];
        } else {
            result += Constants.learningGoalLevels[start] + "-" + Constants.learningGoalLevels[end];
        }
        learningGoalSectionsStr = result;
    }

    //Draws an element in the reorderable list, including search filtering.
    private void DrawElement(Rect rect, int index, bool active, bool focused) {
        float x = rect.position.x;
        float y = rect.position.y;
        float w = rect.width;
        float h = rect.height;

        LearningGoalSectionDefinition def = variables.learningGoalSections[index];
        EditorGUI.LabelField(new Rect(x + 5, y, w, h), Constants.learningGoalLevels[def.min]);
        EditorGUI.LabelField(new Rect(x + w - 20, y, w, h), Constants.learningGoalLevels[def.max]);
        float min = def.min;
        float max = def.max;

        EditorGUI.BeginChangeCheck();
        EditorGUI.MinMaxSlider(new Rect(x + 30, y, w - 60, h), ref min, ref max, 0, Constants.learningGoalLevels.Count - 1);
        if (EditorGUI.EndChangeCheck()) {
            def.min = Mathf.RoundToInt(min);
            def.max = Mathf.RoundToInt(max);
            //UpdateSelectedLearningGoalSections();
            //CheckEnoughExercises();
            //InvalidateGenerationString();
        }
    }

    private void DrawHeader(Rect rect) {
        GUI.Label(rect, new GUIContent("Learning goal sections", "The learning goal section that will occur in the generated level. Each slider can define a learning goal section or a range between two sections. The level generator will evenly divide the selected learning goals over the generated rooms."));
    }

    private void ListChanged(ReorderableList list) {
        UpdateSelectedLearningGoalSections();
        CheckEnoughExercises();
        InvalidateGenerationString();
    }

    private void Generate() {
        levelGenerator.Generate(variables.seed, true, variables);
    }
}
