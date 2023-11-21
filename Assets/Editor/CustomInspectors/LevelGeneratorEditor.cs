using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static LevelGenerator;
using UnityEditor.UIElements;
using UnityEngine.Events;
using System;
using static UnityEditor.Timeline.Actions.MenuPriority;
using UnityEngine.UIElements;

[CustomEditor(typeof(LevelGenerator), true)]
public class LevelGeneratorEditor : Editor {
    private SerializedProperty newLevelOnPlayProp, widthProp, heightProp, layoutTileMapProp, ceilingTileMapProp, npc1TileMapProp, npc2TileMapProp, npcTileProp;
    private SerializedProperty alphabetProp, recipesProp;
    private ReorderableList listAlphabet, listRecipes;

    void OnEnable() {
        newLevelOnPlayProp = serializedObject.FindProperty("newLevelOnPlay");
        recipesProp = serializedObject.FindProperty("recipes");
        alphabetProp = serializedObject.FindProperty("alphabet");
        widthProp = serializedObject.FindProperty("width");
        heightProp = serializedObject.FindProperty("height");
        layoutTileMapProp = serializedObject.FindProperty("layoutTileMap");
        ceilingTileMapProp = serializedObject.FindProperty("ceilingTileMap");
        npc1TileMapProp = serializedObject.FindProperty("npc1TileMap");
        npc2TileMapProp = serializedObject.FindProperty("npc2TileMap");
        npcTileProp = serializedObject.FindProperty("npcTile");
        
        listAlphabet = new ReorderableList(serializedObject, alphabetProp, true, true, true, true);
        listAlphabet.onAddCallback += AddItemT;
        listAlphabet.drawElementCallback += DrawElementT;
        listAlphabet.drawHeaderCallback += DrawHeaderT;

        listRecipes = new ReorderableList(serializedObject, recipesProp, true, true, true, true);
        listRecipes.onAddCallback += AddItemR;
        listRecipes.drawElementCallback += DrawElementR;
        listRecipes.drawHeaderCallback += DrawHeaderR;
    }
    public override void OnInspectorGUI() {
        LevelGenerator generator = (LevelGenerator)target;
        if (GUILayout.Button("Generate")) {
            generator.Generate(true);
        }
        if (GUILayout.Button("Stop generating")) {
            generator.StopGenerating();
        }
        serializedObject.Update();
        EditorGUILayout.PropertyField(newLevelOnPlayProp, new GUIContent("Generate level on scene start"));
        EditorGUILayout.PropertyField(widthProp);
        EditorGUILayout.PropertyField(heightProp);
        EditorGUILayout.PropertyField(layoutTileMapProp);
        EditorGUILayout.PropertyField(ceilingTileMapProp);
        EditorGUILayout.PropertyField(npc1TileMapProp);
        EditorGUILayout.PropertyField(npc2TileMapProp);
        EditorGUILayout.PropertyField(npcTileProp);
        DrawAlphabet();
        DrawRecipes();
        DrawLearningGoals();
    }

    void DrawAlphabet() {
        EditorGUILayout.Space(14);
        EditorGUILayout.LabelField("Alphabet", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginVertical(EditorStyles.helpBox);
        listAlphabet.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        if (listAlphabet.index != -1 && listAlphabet.index < alphabetProp.arraySize) {
            EditorGUILayout.Space(-16);
            EditorGUILayout.LabelField("Alphabet entry properties", EditorStyles.boldLabel);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            serializedObject.Update();
            SerializedProperty terminalProp = alphabetProp.GetArrayElementAtIndex(listAlphabet.index);
            PropertyField(terminalProp, "name");
            PropertyField(terminalProp, "character");
            PropertyField(terminalProp, "type");
            PropertyField(terminalProp, "tileBase");

            EditorGUILayout.EndVertical();
        }
        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndVertical();
    }

    void DrawRecipes() {
        LevelGenerator generator = target as LevelGenerator;
        EditorGUILayout.Space(14);
        EditorGUILayout.LabelField("Recipes", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginVertical(EditorStyles.helpBox);
        listRecipes.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        if (listRecipes.index != -1 && listRecipes.index < recipesProp.arraySize) {
            EditorGUILayout.Space(-16);
            EditorGUILayout.LabelField("Recipe properties", EditorStyles.boldLabel);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            serializedObject.Update();
            SerializedProperty recipeProp = recipesProp.GetArrayElementAtIndex(listRecipes.index);
            PropertyField(recipeProp, "name");
            PropertyField(recipeProp, "type");
            if (generator.recipes[listRecipes.index].type != RecipeType.NONE) {
                PropertyField(recipeProp, "minTimesToExecute");
                PropertyField(recipeProp, "maxTimesToExecute");
            }

            EditorGUILayout.Space(10);
            switch (generator.recipes[listRecipes.index].type) {
                case RecipeType.GRAMMAR:
                    EditorGUI.indentLevel++;
                    PropertyField(recipeProp, "grammars");
                    EditorGUI.indentLevel--;
                    break;
                case RecipeType.CUSTOMGRAMMAR:
                    EditorGUI.indentLevel++;
                    PropertyField(recipeProp, "customGrammars");
                    EditorGUI.indentLevel--;
                    break;
                case RecipeType.REDIRECTION:
                    Color color = GUI.backgroundColor;
                    GUI.backgroundColor = generator.IsRedirectValid(generator.recipes[listRecipes.index]) ? color : Color.red;
                    PropertyField(recipeProp, "redirectionName");
                    GUI.backgroundColor = color;
                    PropertyField(recipeProp, "amountOfRedirections");
                    break;
            }

            EditorGUILayout.Space(10);
            PropertyField(recipeProp, "onRecipeStartCallback");
            PropertyField(recipeProp, "onRecipeEndCallback");
            EditorGUILayout.EndVertical();
        }
        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndVertical();
    }

    void DrawLearningGoals() {
        LevelGenerator generator = (LevelGenerator)target;

        EditorGUILayout.Space(14);
        EditorGUILayout.LabelField("Learning goals", EditorStyles.boldLabel);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Learning goals levels");

        var controlRect = EditorGUILayout.GetControlRect();
        float x = controlRect.position.x;
        float y = controlRect.position.y;
        float w = controlRect.width;
        float h = controlRect.height;

        EditorGUI.LabelField(new Rect(x + 5, y, w, h), generator.learningGoalLevels[generator.minLearningGoalLevel]);
        EditorGUI.LabelField(new Rect(x + w - 20, y, w, h), generator.learningGoalLevels[generator.maxLearningGoalLevel]);
        float min = generator.minLearningGoalLevel;
        float max = generator.maxLearningGoalLevel;

        EditorGUI.BeginChangeCheck();
        EditorGUI.MinMaxSlider(new Rect(x + 30, y, w - 60, h), ref min, ref max, 0, generator.learningGoalLevels.Count - 1);
        if (EditorGUI.EndChangeCheck()) {
            generator.minLearningGoalLevel = Mathf.RoundToInt(min);
            generator.maxLearningGoalLevel = Mathf.RoundToInt(max);
        }
        GUILayout.EndVertical();
    }

    private void PropertyField(SerializedProperty parentProperty, string name, string displayName = "") {
        SerializedProperty property = parentProperty.FindPropertyRelative(name);
        if (displayName.Length == 0) EditorGUILayout.PropertyField(property);
        else EditorGUILayout.PropertyField(property, new GUIContent(displayName));
    }

    private void OnDisable() {
        listAlphabet.onAddCallback -= AddItemT;
        listAlphabet.drawElementCallback -= DrawElementT;
        listAlphabet.drawHeaderCallback -= DrawHeaderT;
        listRecipes.onAddCallback -= AddItemR;
        listRecipes.drawElementCallback -= DrawElementR;
        listRecipes.drawHeaderCallback -= DrawHeaderR;
    }

    //Alphabet
    private void AddItemT(ReorderableList list) {
        LevelGenerator generator = target as LevelGenerator;
        generator.alphabet.Add(new Symbol());
        EditorUtility.SetDirty(target);
    }

    private void DrawElementT(Rect rect, int index, bool active, bool focused) {
        LevelGenerator generator = target as LevelGenerator;
        string name = generator.alphabet[index].name;
        if (name.Length == 0) name = "{Empty}";
        GUI.Label(rect, name);
    }

    private void DrawHeaderT(Rect rect) {
        GUI.Label(rect, "Alphabet");
    }

    //Recipes
    private void AddItemR(ReorderableList list) {
        LevelGenerator generator = target as LevelGenerator;
        generator.recipes.Add(new Recipe());
        EditorUtility.SetDirty(target);
    }

    private void DrawElementR(Rect rect, int index, bool active, bool focused) {
        LevelGenerator generator = target as LevelGenerator;
        string name = generator.recipes[index].name;
        if (name.Length == 0) name = "{Empty}";
        GUI.Label(rect, $"{index} {name}");

        Rect rectNew = new Rect(rect.x + rect.width - 20, rect.y, 20, rect.height);
        SerializedProperty recipeProp = recipesProp.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rectNew, recipeProp.FindPropertyRelative("enabled"), new GUIContent());
    }

    private void DrawHeaderR(Rect rect) {
        GUI.Label(rect, "Recipes");

        Rect labelRect = new Rect(rect.x + rect.width - 55, rect.y, 50, rect.height);
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleRight;
        GUI.Label(labelRect, "Enabled", style);
    }
}


