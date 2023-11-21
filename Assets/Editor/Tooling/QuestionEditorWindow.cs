using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using static PlasticGui.WorkspaceWindow.CodeReview.Summary.CommentSummaryData;
using static UnityEditor.Undo;

public class QuestionEditorWindow : EditorWindow {
    //private QuestionList questionList, originalQuestionList;
    //private ReorderableList mathList;
    //private Question selectedQuestion;
    //private Vector2 scrollPosition;
    //private string filter = "Name";
    //private string searchString = "";
    //private bool dirty = false;
    //private bool shouldSave = false;
    //private int activeLocalizationTab = 0;
    //
    //[MenuItem("Function Dungeon/Question Editor")]
    //public static void ShowWindow() {
    //    GetWindow<QuestionEditorWindow>("Question Editor");
    //}
    //
    ////Used to import data from the old exercise list to the new question list format.
    //private void ImportFromOldExerciseList() {
    //    //var _exerciseList = AssetDatabase.LoadAssetAtPath<ExerciseList>("Assets/Scriptable Objects/Math/ExerciseList/Question List.asset");
    //
    //    //
    //    //foreach (var math in _exerciseList.list) {
    //    //    Question question = new Question();
    //    //    _questionList.questions.Add(question);
    //    //    question.name = math.id;
    //    //    question.image = math.image;
    //    //    question.section = math.section;
    //    //    question.variation = math.variation;
    //    //    question.learningGoal = math.learningGoal;
    //    //    question.dialogue = math.dialogue;
    //    //    question.used = math.used;
    //    //
    //    //    string g = math.section;
    //    //    if (g == "x") g = "1.1";
    //    //    int index = Constants.learningGoalLevels.IndexOf(g);
    //    //    question.learningGoalLevel = index;
    //    //
    //    //    foreach (var text in math.text) {
    //    //        QuestionText qt = new QuestionText();
    //    //        question.text.Add(qt);
    //    //        qt.locale = text.locale;
    //    //        qt.question = text.question;
    //    //        qt.correct = text.correct;
    //    //        qt.wrong1 = text.wrong1;
    //    //        qt.wrong2 = text.wrong2;
    //    //        qt.wrong3 = text.wrong3;
    //    //        qt.feedback = text.feedback;
    //    //    }
    //    //}
    //}
    //
    ////Subscribe to the undo/redo event, load the question list and set up the reorderable list.
    //private void OnEnable() {
    //    Undo.undoRedoPerformed += UndoRedoCallback;
    //    // Load the Scriptable Object
    //    originalQuestionList = AssetDatabase.LoadAssetAtPath<QuestionList>(Constants.questionListPath);
    //    questionList = Instantiate(originalQuestionList);
    //    questionList.hideFlags = HideFlags.HideAndDontSave;
    //    //ImportFromOldExerciseList();
    //
    //    if (questionList != null) {
    //        LoadQuestionData();
    //
    //        // Initialize the ReorderableList
    //        mathList = new ReorderableList(questionList.questions, typeof(Question), true, true, true, true);
    //
    //        mathList.onAddCallback = AddItem;
    //        mathList.drawElementCallback = DrawElement;
    //        mathList.drawHeaderCallback = DrawHeader;
    //
    //        mathList.onSelectCallback = (ReorderableList list) => {
    //            selectedQuestion = questionList.questions[list.index];
    //        };
    //    }
    //}
    //
    //private void OnDisable() {
    //    Undo.undoRedoPerformed -= UndoRedoCallback;
    //}
    //
    ////Unregisters event handlers and triggers a save if the data is dirty.
    //private void OnDestroy() {
    //    mathList.drawElementCallback -= DrawElement;
    //    mathList.drawHeaderCallback -= DrawHeader;
    //    mathList.onAddCallback -= AddItem;
    //    SavePopupIfDirty();
    //}
    //
    ////Callback function for undo/redo events. Marks the data as dirty.
    //void UndoRedoCallback() {
    //    MarkDirty();
    //}
    //
    ////Adds a new item to the reorderable list and marks the data as dirty.
    //private void AddItem(ReorderableList list) {
    //    questionList.questions.Add(new Question());
    //    MarkDirty();
    //}
    //
    ////Draws an element in the reorderable list, including search filtering.
    //private void DrawElement(Rect rect, int index, bool active, bool focused) {
    //    string name = questionList.questions[index].name;
    //    if (name.Length == 0) name = "{Empty}";
    //    string learningGoal = "Invalid learning goal";
    //    if (Utils.Within(questionList.questions[index].learningGoalLevel, 0, Constants.learningGoalLevels.Count)) {
    //        learningGoal = Constants.learningGoalLevels[questionList.questions[index].learningGoalLevel];
    //    }
    //
    //    void drawName() {
    //        float indexWidth = GUI.skin.label.CalcSize(new GUIContent(learningGoal)).x;
    //        Rect nameRect = new Rect(rect.x, rect.y, rect.width - indexWidth, rect.height);
    //        Rect indexRect = new Rect(rect.xMax - indexWidth, rect.y, indexWidth, rect.height);
    //        GUI.Label(nameRect, name);
    //        GUI.Label(indexRect, learningGoal);
    //    }
    //
    //    if (string.IsNullOrEmpty(searchString)) {
    //        drawName();
    //    } else {
    //        bool contains = false;
    //        foreach (var text in questionList.questions[index].text) {
    //            contains |= text.question.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //            contains |= text.correct.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //            contains |= text.wrong1.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //            contains |= text.wrong2.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //            contains |= text.wrong3.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //            contains |= text.feedback.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //        }
    //        contains |= name.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //        contains |= learningGoal.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    //        if (contains) {
    //            drawName();
    //        }
    //    }
    //}
    //
    //private void DrawHeader(Rect rect) {
    //    string str1 = "Question";
    //    string str2 = "Learning goal";
    //    float width = GUI.skin.label.CalcSize(new GUIContent(str2)).x;
    //    Rect rect1 = new Rect(rect.x, rect.y, rect.width - width, rect.height);
    //    Rect rect2 = new Rect(rect.xMax - width, rect.y, width, rect.height);
    //    GUI.Label(rect1, str1);
    //    GUI.Label(rect2, str2);
    //}
    //
    ////Handles the main GUI layout, including drawing the list and properties.
    //private void OnGUI() {
    //    Undo.RecordObject(questionList, "QuestionList");
    //    EditorGUILayout.BeginHorizontal();
    //    DrawQuestionList();
    //    DrawQuestionProperties();
    //    EditorGUILayout.EndHorizontal();
    //
    //
    //    if (shouldSave) {
    //        StoreQuestionData();
    //        EditorUtility.SetDirty(questionList);
    //        AssetDatabase.SaveAssets();
    //        MarkDirty(false);
    //        shouldSave = false;
    //    }
    //}
    //
    ////Draws the list of questions and handles user input.
    //private void DrawQuestionList() {
    //    EditorGUILayout.BeginVertical(GUILayout.Width(300));
    //    EditorGUILayout.Space(1);
    //    searchString = EditorGUILayout.TextField("Search:", searchString);
    //    DrawFilter();
    //    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
    //    EditorGUI.BeginChangeCheck();
    //    mathList.DoLayoutList();
    //    if (EditorGUI.EndChangeCheck()) {
    //        MarkDirty();
    //    }
    //    EditorGUILayout.EndScrollView();
    //    EditorGUILayout.EndVertical();
    //}
    //
    ////Draws a filter dropdown for sorting the reorderable list.
    //private void DrawFilter() {
    //    EditorGUILayout.BeginHorizontal();
    //    EditorGUILayout.LabelField("Filter: ");
    //    if (EditorGUILayout.DropdownButton(new GUIContent(filter), FocusType.Passive, GUILayout.Width(100))) {
    //        GenericMenu menu = new GenericMenu();
    //
    //        menu.AddItem(new GUIContent("Name"), false, () => {
    //            filter = "Name";
    //            questionList.questions.Sort(delegate (Question x, Question y) {
    //                return StringLogicalComparer.Compare(x.name, y.name);
    //            });
    //            Repaint();
    //        });
    //        menu.AddItem(new GUIContent("Learning goal"), false, () => {
    //            filter = "Learning goal";
    //            questionList.questions.Sort(delegate (Question x, Question y) {
    //                return x.learningGoalLevel.CompareTo(y.learningGoalLevel);
    //            });
    //            Repaint();
    //        });
    //        menu.ShowAsContext();
    //    }
    //    EditorGUILayout.EndHorizontal();
    //}
    //
    ////Displays editable properties for the selected question.
    //private void DrawQuestionProperties() {
    //    GUILayout.BeginVertical();
    //    GUILayout.BeginVertical(EditorStyles.helpBox);
    //    EditorGUI.BeginChangeCheck();
    //    string name = selectedQuestion != null ? selectedQuestion.name : "";
    //    int learningGoalLevel = selectedQuestion != null ? selectedQuestion.learningGoalLevel : 0;
    //
    //    QuestionType questionType = selectedQuestion != null ? selectedQuestion.type : QuestionType.MULTIPLECHOICE;
    //    EditorGUI.BeginDisabledGroup(selectedQuestion == null);
    //    name = EditorGUILayout.TextField("Name:", name);
    //    GUILayout.BeginHorizontal();
    //    GUILayout.Label("Question type", GUILayout.Width(EditorGUIUtility.labelWidth - 2));
    //    if (EditorGUILayout.DropdownButton(new GUIContent(questionType.ToString()), FocusType.Passive)) {
    //        GenericMenu menu = new GenericMenu();
    //
    //        foreach (QuestionType type in Enum.GetValues(typeof(QuestionType))) {
    //            menu.AddItem(new GUIContent(type.ToString()), selectedQuestion.type == type, x => { selectedQuestion.type = (QuestionType)x; Repaint(); MarkDirty(); }, type);
    //        }
    //
    //        menu.ShowAsContext();
    //    }
    //    GUILayout.EndHorizontal();
    //
    //    EditorGUILayout.LabelField("Learning goal section:");
    //    //Learning goal slider
    //    var controlRect = EditorGUILayout.GetControlRect();
    //    float x = controlRect.position.x;
    //    float y = controlRect.position.y;
    //    float w = controlRect.width;
    //    float h = controlRect.height;
    //
    //    EditorGUI.LabelField(new Rect(x, y, w, h), Constants.learningGoalLevels[learningGoalLevel]);
    //    learningGoalLevel = EditorGUI.IntSlider(new Rect(x + 30, y, w - 30, h), learningGoalLevel, 0, Constants.learningGoalLevels.Count - 1);
    //
    //    if (EditorGUI.EndChangeCheck() && selectedQuestion != null) {
    //        selectedQuestion.name = name;
    //        selectedQuestion.section = Constants.learningGoalLevels[learningGoalLevel];
    //        selectedQuestion.learningGoalLevel = learningGoalLevel;
    //        MarkDirty();
    //    }
    //    Utils.GuiLine();
    //    GUILayout.Space(10);
    //
    //    switch (questionType) {
    //        case QuestionType.MULTIPLECHOICE: DrawQuestionMultipleChoice(); break;
    //        case QuestionType.CUSTOM: DrawQuestionCustom(); break;
    //    }
    //
    //    EditorGUILayout.EndVertical();
    //    EditorGUI.EndDisabledGroup();
    //
    //    //Save button
    //    EditorGUILayout.BeginHorizontal();
    //    GUILayout.FlexibleSpace();
    //    if (GUILayout.Button("Save changes", GUILayout.Width(100))) {
    //        Save();
    //    }
    //    EditorGUILayout.EndHorizontal();
    //    EditorGUILayout.EndVertical();
    //}
    //
    //private void DrawQuestionMultipleChoice() {
    //    Sprite image = selectedQuestion != null ? selectedQuestion.image : null;
    //    string variation = selectedQuestion != null ? selectedQuestion.variation : "";
    //    Dialogue dialogue = selectedQuestion != null ? selectedQuestion.dialogue : null;
    //    variation = EditorGUILayout.TextField("Variation:", variation);
    //
    //    dialogue = (Dialogue)EditorGUILayout.ObjectField("Dialogue:", dialogue, typeof(Dialogue), false);
    //    image = (Sprite)EditorGUILayout.ObjectField("Image:", image, typeof(Sprite), false);
    //    //Save changes
    //    if (EditorGUI.EndChangeCheck() && selectedQuestion != null) {
    //        selectedQuestion.image = image;
    //        selectedQuestion.variation = variation;
    //        selectedQuestion.dialogue = dialogue;
    //        MarkDirty();
    //    }
    //
    //    GUILayout.Space(10);
    //    Utils.GuiLine();
    //
    //    DrawLocalizationSection();
    //}
    //
    //private void DrawQuestionCustom() {
    //    EditorGUI.BeginChangeCheck();
    //    GameObject prefab = selectedQuestion != null ? selectedQuestion.puzzlePrefab : null;
    //    prefab = (GameObject)EditorGUILayout.ObjectField("Puzzle prefab:", prefab, typeof(GameObject), false);
    //    if (EditorGUI.EndChangeCheck() && selectedQuestion != null) {
    //        selectedQuestion.puzzlePrefab = prefab;
    //        MarkDirty();
    //    }
    //
    //    if(selectedQuestion != null && selectedQuestion.puzzlePrefab != null) {
    //        if (!selectedQuestion.puzzlePrefab.GetComponent<CustomPuzzle>()) {
    //            EditorGUILayout.HelpBox("Selected prefab does not contain a CustomPuzzle script.", MessageType.Error);
    //        }
    //    }
    //}
    //
    //// Displays localization options and allows editing localized text.
    //private void DrawLocalizationSection() {
    //    if (LocalizationSettings.AvailableLocales.Locales.Count == 0) {
    //        GUILayout.Space(10);
    //        EditorGUILayout.LabelField("AvailableLocales can't be retrieved from LocalizationSettings");
    //        return;
    //    }
    //    EditorGUILayout.LabelField("Localization");
    //    activeLocalizationTab = GUILayout.Toolbar(activeLocalizationTab, LocalizationSettings.AvailableLocales.Locales.Select(x => x.name).ToArray());
    //    EditorGUI.BeginChangeCheck();
    //    GUILayout.BeginVertical(EditorStyles.helpBox);
    //    Locale activeLocale = LocalizationSettings.AvailableLocales.Locales[activeLocalizationTab];
    //    QuestionText localizedQuestionText = null;
    //    if (selectedQuestion != null) {
    //        //Ensure the localized math text exists in the math object
    //        localizedQuestionText = selectedQuestion.text.Find(text => text.locale == activeLocale);
    //        if (localizedQuestionText == null) {
    //            localizedQuestionText = new QuestionText();
    //            localizedQuestionText.locale = activeLocale;
    //            selectedQuestion.text.Add(localizedQuestionText);
    //        }
    //    }
    //
    //    string question = localizedQuestionText != null ? localizedQuestionText.question : "";
    //    string correct = localizedQuestionText != null ? localizedQuestionText.correct : "";
    //    string wrong1 = localizedQuestionText != null ? localizedQuestionText.wrong1 : "";
    //    string wrong2 = localizedQuestionText != null ? localizedQuestionText.wrong2 : "";
    //    string wrong3 = localizedQuestionText != null ? localizedQuestionText.wrong3 : "";
    //    string feedback = localizedQuestionText != null ? localizedQuestionText.feedback : "";
    //
    //    EditorGUILayout.LabelField("Question:");
    //    question = FilteredTextArea(question);
    //    EditorGUILayout.LabelField("Correct:");
    //    correct = FilteredTextArea(correct);
    //    EditorGUILayout.LabelField("Wrong 1:");
    //    wrong1 = FilteredTextArea(wrong1);
    //    EditorGUILayout.LabelField("Wrong 2:");
    //    wrong2 = FilteredTextArea(wrong2);
    //    EditorGUILayout.LabelField("Wrong 3:");
    //    wrong3 = FilteredTextArea(wrong3);
    //    EditorGUILayout.LabelField("Feedback:");
    //    feedback = FilteredTextArea(feedback);
    //
    //    if (EditorGUI.EndChangeCheck() && localizedQuestionText != null) {
    //        localizedQuestionText.question = question;
    //        localizedQuestionText.correct = correct;
    //        localizedQuestionText.wrong1 = wrong1;
    //        localizedQuestionText.wrong2 = wrong2;
    //        localizedQuestionText.wrong3 = wrong3;
    //        localizedQuestionText.feedback = feedback;
    //        MarkDirty();
    //    }
    //
    //    EditorGUILayout.EndVertical();
    //}
    //
    ////Draws a text area but uses the search string to highlight the area
    //private string FilteredTextArea(string text) {
    //    Color color = GUI.backgroundColor;
    //    if (searchString.Length != 0 && text.Contains(searchString, StringComparison.OrdinalIgnoreCase)) GUI.backgroundColor = Color.green;
    //    string toRet = EditorGUILayout.TextArea(text, GUILayout.ExpandHeight(true));
    //    GUI.backgroundColor = color;
    //    return toRet;
    //}
    //
    ////
    //private void MarkDirty(bool dirty = true) {
    //    this.dirty = dirty;
    //    titleContent.text = "Question Editor " + (dirty ? "*" : "");
    //}
    //
    ////Initiates a delayed save operation when changes are pending. We can't save directly because then the save button press would mark the question list as dirty again.
    //private void Save() {
    //    if (!dirty) return;
    //    shouldSave = true;
    //}
    //
    ////Create a local copy of the questions to ensure we're not directly editing the scriptable object.
    //private void LoadQuestionData() {
    //    questionList.questions.Clear();
    //    foreach (var question in originalQuestionList.questions) {
    //        questionList.questions.Add((Question)question.Clone());
    //    }
    //}
    //
    ////Return the data of the local questions to the scriptable objects. Needs to be called before saving.
    //private void StoreQuestionData() {
    //    originalQuestionList.questions.Clear();
    //    foreach (var question in questionList.questions) {
    //        originalQuestionList.questions.Add((Question)question.Clone());
    //    }
    //}
    //
    ////Displays a popup dialog to save changes if data is dirty.
    //void SavePopupIfDirty() {
    //    if (dirty) {
    //        if (EditorUtility.DisplayDialog("Question list has been modified",
    //            $"Do you want to save the changes you made to the question list?\nYour changes will be lost if you don't save them.",
    //            "Save", "Discard changes")) {
    //            StoreQuestionData();
    //            EditorUtility.SetDirty(questionList);
    //            AssetDatabase.SaveAssets();
    //            MarkDirty(false);
    //        }
    //    }
    //}
}