using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SceneManager))]
public class SceneManagerEditor : Editor {
    private ReorderableList reorderableList;
    private SerializedProperty sceneDefinitionsProperty;
    private int globalIndex;

    private void OnEnable() {
        sceneDefinitionsProperty = serializedObject.FindProperty("sceneDefinitions");
        reorderableList = new ReorderableList(serializedObject, sceneDefinitionsProperty, true, true, true, true);
        reorderableList.onAddCallback += AddItem;
        reorderableList.drawElementCallback += DrawElement;
        reorderableList.drawHeaderCallback += DrawHeader;
    }

    public override void OnInspectorGUI() {
        SceneManager mgr = target as SceneManager;
        EditorGUI.BeginChangeCheck();
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        if (reorderableList.index != -1 && reorderableList.index < sceneDefinitionsProperty.arraySize) {
            EditorGUILayout.LabelField("Scene properties", EditorStyles.boldLabel);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            serializedObject.Update();
            SerializedProperty sceneDefinition = sceneDefinitionsProperty.GetArrayElementAtIndex(reorderableList.index);
            SerializedProperty name = sceneDefinition.FindPropertyRelative("name");
            SerializedProperty scene = sceneDefinition.FindPropertyRelative("scene");

            EditorGUILayout.PropertyField(name, new GUIContent("Name"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(scene);
            EditorGUILayout.Space(-28); //Fuck yeah, magic numbers

            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
            //Force override any changes to the prefab for persistence across different scenes.
            PrefabUtility.ApplyPrefabInstance(PrefabUtility.GetNearestPrefabInstanceRoot(mgr.gameObject), InteractionMode.AutomatedAction);
        }
    }

    private void OnDisable() {
        reorderableList.onAddCallback -= AddItem;
        reorderableList.drawElementCallback -= DrawElement;
        reorderableList.drawHeaderCallback -= DrawHeader;
    }

    private void AddItem(ReorderableList list) {
        SceneManager mgr = target as SceneManager;
        mgr.sceneDefinitions.Add(new SceneManager.SceneDefinition());
        EditorUtility.SetDirty(target);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused) {
        SceneManager mgr = target as SceneManager;
        string name = mgr.sceneDefinitions[index].name;
        if (name.Length == 0) name = "{Empty}";
        GUI.Label(rect, name);
    }
   

    private void DrawHeader(Rect rect) {
        GUI.Label(rect, "Scenes");
    }
}