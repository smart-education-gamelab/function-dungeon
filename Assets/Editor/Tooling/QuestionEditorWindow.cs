using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System.Linq;
using System.Reflection;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using static UnityEditor.Rendering.FilterWindow;
using Malee.List;

public class QuestionEditorWindow : EditorWindow {
    private SerializedObject serializedQuestionList;
    private SerializedProperty questionsProperty;
    private ReorderableList mathList;
    private SerializedProperty selectedQuestion;
    private Vector2 scrollPosition;
    private string searchString = "";
    private int activeLocalizationTab = 0;

    [MenuItem("Function Dungeon/Question Editor")]
    public static void ShowWindow() {
        GetWindow<QuestionEditorWindow>("Question Editor");
    }

    //Subscribe to the undo/redo event, load the question list and set up the reorderable list.
    private void OnEnable() {
        // Load the Scriptable Object
        QuestionList questionList = AssetDatabase.LoadAssetAtPath<QuestionList>(Constants.questionListPath);
        serializedQuestionList = new SerializedObject(questionList);

        if (questionList != null) {
            questionsProperty = serializedQuestionList.FindProperty("questions");
            mathList = new ReorderableList(questionsProperty, true, true, true);
            mathList.expandable = false;
            mathList.elementLabels = false;
            mathList.elementDisplayType = ReorderableList.ElementDisplayType.SingleLine;
            mathList.sortable = true;
            mathList.paginate = true;
            mathList.drawElementCallback += DrawElement;
            mathList.drawHeaderCallback += DrawHeader;
            mathList.onSelectCallback += OnSelect;
            mathList.onRemoveCallback += OnRemove;
        }
    }

    private void OnRemove(ReorderableList list) {
        selectedQuestion = null;
        list.Remove(mathList.Selected);
    }

    //Unregisters event handlers and triggers a save if the data is dirty.
    private void OnDestroy() {
        mathList.drawElementCallback -= DrawElement;
        mathList.drawHeaderCallback -= DrawHeader;
        mathList.onSelectCallback -= OnSelect;
        mathList.onRemoveCallback -= OnRemove;
        serializedQuestionList.ApplyModifiedProperties();
    }

    private void OnSelect(ReorderableList list) {
        if (list.Selected.Length == 1) {
            selectedQuestion = list.GetItem(list.Selected[0]);
        } else {
            selectedQuestion = null;
        }
    }

    //Draws an element in the reorderable list, including search filtering.
    private void DrawElement(Rect rect, SerializedProperty elementProperty, GUIContent guiLabel, bool selected, bool focused) {
        SerializedProperty labelProperty = elementProperty.FindPropertyRelative("name");
        SerializedProperty learningGoalLevelProperty = elementProperty.FindPropertyRelative("learningGoalLevel");

        string label = labelProperty.stringValue;
        if (label.Length == 0) label = "{Empty}";
        string learningGoal = "Invalid learning goal";
        int learningGoalLevel = learningGoalLevelProperty.intValue;
        if (Utils.Within(learningGoalLevel, 0, Constants.learningGoalLevels.Count)) {
            learningGoal = Constants.learningGoalLevels[learningGoalLevel];
        }

        void drawLabel() {
            float indexWidth = GUI.skin.label.CalcSize(new GUIContent(learningGoal)).x;
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - indexWidth, rect.height);
            Rect indexRect = new Rect(rect.xMax - indexWidth, rect.y, indexWidth, rect.height);
            GUI.Label(labelRect, label);
            GUI.Label(indexRect, learningGoal);
        }

        if (string.IsNullOrEmpty(searchString)) {
            drawLabel();
        } else {
            bool contains = false;
            SerializedProperty textArrayProperty = elementProperty.FindPropertyRelative("text");

            for (int i = 0; i < textArrayProperty.arraySize; i++) {
                SerializedProperty textProperty = textArrayProperty.GetArrayElementAtIndex(i);
                contains |= CheckPropertyContains(textProperty, "question", searchString);
                contains |= CheckPropertyContains(textProperty, "correct", searchString);
                contains |= CheckPropertyContains(textProperty, "wrong1", searchString);
                contains |= CheckPropertyContains(textProperty, "wrong2", searchString);
                contains |= CheckPropertyContains(textProperty, "wrong3", searchString);
                contains |= CheckPropertyContains(textProperty, "feedback", searchString);
            }

            contains |= label.Contains(searchString, StringComparison.OrdinalIgnoreCase);
            contains |= learningGoal.Contains(searchString, StringComparison.OrdinalIgnoreCase);
            if (contains) {
                drawLabel();
            }
        }
    }

    private bool CheckPropertyContains(SerializedProperty textProperty, string propertyName, string searchString) {
        SerializedProperty property = textProperty.FindPropertyRelative(propertyName);
        return property.stringValue.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    }

    private void DrawHeader(Rect rect, GUIContent label) {
        string str1 = "Question";
        string str2 = "Learning goal";
        float width = GUI.skin.label.CalcSize(new GUIContent(str2)).x;
        Rect rect1 = new Rect(rect.x, rect.y, rect.width - width, rect.height);
        Rect rect2 = new Rect(rect.xMax - width, rect.y, width, rect.height);
        GUI.Label(rect1, str1);
        GUI.Label(rect2, str2);
    }

    //Handles the main GUI layout, including drawing the list and properties.
    private void OnGUI() {
        serializedQuestionList.Update();
        EditorGUILayout.BeginHorizontal();
        DrawQuestionList();
        DrawQuestionProperties();
        EditorGUILayout.EndHorizontal();
        //if (!EditorGUIUtility.editingTextField)
        serializedQuestionList.ApplyModifiedProperties();
    }

    //Draws the list of questions and handles user input.
    private void DrawQuestionList() {
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.Space(1);
        searchString = EditorGUILayout.TextField("Search:", searchString);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        mathList.DoLayoutList();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    //Displays editable properties for the selected question.
    private void DrawQuestionProperties() {
        if (selectedQuestion == null) {
            EditorGUILayout.LabelField("No question selected");
            return;
        }

        SerializedProperty nameProperty = selectedQuestion.FindPropertyRelative("name");
        SerializedProperty learningGoalLevelProperty = selectedQuestion.FindPropertyRelative("learningGoalLevel");
        SerializedProperty questionTypeProperty = selectedQuestion.FindPropertyRelative("type");

        GUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUI.BeginDisabledGroup(selectedQuestion == null);
        Utils.DeselectOnFocusTextField(nameProperty, new GUIContent("Name:"));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Question type", GUILayout.Width(EditorGUIUtility.labelWidth - 2));
        if (EditorGUILayout.DropdownButton(new GUIContent(questionTypeProperty.enumNames[questionTypeProperty.enumValueIndex]), FocusType.Passive)) {
            GenericMenu menu = new GenericMenu();

            foreach (QuestionType type in Enum.GetValues(typeof(QuestionType))) {
                bool isSelected = type == (QuestionType)questionTypeProperty.intValue;
                menu.AddItem(new GUIContent(type.ToString()), isSelected, () => {
                    questionTypeProperty.intValue = (int)type;
                    serializedQuestionList.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Learning goal section:");
        var controlRect = EditorGUILayout.GetControlRect();
        float x = controlRect.position.x;
        float y = controlRect.position.y;
        float w = controlRect.width;
        float h = controlRect.height;

        // Use SerializedProperty for label field
        EditorGUI.LabelField(new Rect(x, y, w, h), Constants.learningGoalLevels[learningGoalLevelProperty.intValue]);

        // Use SerializedProperty for int slider
        learningGoalLevelProperty.intValue = EditorGUI.IntSlider(new Rect(x + 30, y, w - 30, h), learningGoalLevelProperty.intValue, 0, Constants.learningGoalLevels.Count - 1);

        Utils.GuiLine();
        GUILayout.Space(10);

        QuestionType questionType = (QuestionType)questionTypeProperty.enumValueIndex;
        switch (questionType) {
            case QuestionType.MULTIPLECHOICE: DrawQuestionMultipleChoice(); break;
            case QuestionType.CUSTOM: DrawQuestionCustom(); break;
        }

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }

    private void DrawQuestionMultipleChoice() {
        SerializedProperty imageProperty = selectedQuestion.FindPropertyRelative("image");
        SerializedProperty variationProperty = selectedQuestion.FindPropertyRelative("variation");
        SerializedProperty dialogueProperty = selectedQuestion.FindPropertyRelative("dialogue");
        EditorGUILayout.PropertyField(variationProperty, new GUIContent("Variation:"));
        EditorGUILayout.PropertyField(dialogueProperty, new GUIContent("Dialogue:"));
        EditorGUILayout.PropertyField(imageProperty, new GUIContent("Image:"));

        GUILayout.Space(10);
        Utils.GuiLine();

        DrawLocalizationSection();
    }

    private void DrawQuestionCustom() {
        SerializedProperty puzzlePrefabProperty = selectedQuestion.FindPropertyRelative("puzzlePrefab");
        EditorGUILayout.PropertyField(puzzlePrefabProperty, new GUIContent("Puzzle prefab:"));

        GameObject prefab = (GameObject)puzzlePrefabProperty.objectReferenceValue;
        if (prefab != null && !prefab.GetComponent<CustomPuzzle>()) {
            EditorGUILayout.HelpBox("Selected prefab does not contain a CustomPuzzle script.", MessageType.Error);
        }
    }

    // Displays localization options and allows editing localized text.
    private void DrawLocalizationSection() {
        if (LocalizationSettings.AvailableLocales.Locales.Count == 0) {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("AvailableLocales can't be retrieved from LocalizationSettings");
            return;
        }
        EditorGUILayout.LabelField("Localization");
        activeLocalizationTab = GUILayout.Toolbar(activeLocalizationTab, LocalizationSettings.AvailableLocales.Locales.Select(x => x.name).ToArray());
        GUILayout.BeginVertical(EditorStyles.helpBox);
        Locale activeLocale = LocalizationSettings.AvailableLocales.Locales[activeLocalizationTab];

        SerializedProperty localizedQuestionText = FindOrAddLocalizedText(activeLocale);
        FilteredTextArea(localizedQuestionText.FindPropertyRelative("question"), new GUIContent("Question:"));
        FilteredTextArea(localizedQuestionText.FindPropertyRelative("correct"), new GUIContent("Correct:"));
        FilteredTextArea(localizedQuestionText.FindPropertyRelative("wrong1"), new GUIContent("Wrong 1:"));
        FilteredTextArea(localizedQuestionText.FindPropertyRelative("wrong2"), new GUIContent("Wrong 2:"));
        FilteredTextArea(localizedQuestionText.FindPropertyRelative("wrong3"), new GUIContent("Wrong 3:"));
        FilteredTextArea(localizedQuestionText.FindPropertyRelative("feedback"), new GUIContent("Feedback:"));

        EditorGUILayout.EndVertical();
    }

    //Find the property with localizations for the current language. If it doesn't exist, add it.
    private SerializedProperty FindOrAddLocalizedText(Locale locale) {
        SerializedProperty propTexts = selectedQuestion.FindPropertyRelative("text");
        for (int i = 0; i < propTexts.arraySize; i++) {
            SerializedProperty propText = propTexts.GetArrayElementAtIndex(i);
            if (propText.FindPropertyRelative("locale").objectReferenceValue == locale) {
                return propText;
            }
        }

        propTexts.arraySize++;
        SerializedProperty newPropText = propTexts.GetArrayElementAtIndex(propTexts.arraySize - 1);
        newPropText.FindPropertyRelative("locale").objectReferenceValue = locale;
        return newPropText;
    }

    //Draws a text area but uses the search string to highlight the area
    private void FilteredTextArea(SerializedProperty text, GUIContent label) {
        Color color = GUI.backgroundColor;
        if (searchString.Length != 0 && text.stringValue.Contains(searchString, StringComparison.OrdinalIgnoreCase)) GUI.backgroundColor = Color.green;
        Utils.DeselectOnFocusTextArea(text, label, GUILayout.ExpandHeight(true));
        GUI.backgroundColor = color;
    }
}