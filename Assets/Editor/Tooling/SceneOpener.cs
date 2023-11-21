using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneOpener : EditorWindow {
    [MenuItem("Scene/Main")]
    public static void Main() {
        if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
    }

    [MenuItem("Scene/Persistent")]
    public static void Persistent() {
        if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Persistent.unity");
    }

    [MenuItem("Scene/Level Generation")]
    public static void LevelGeneration() {
        if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/level generation.unity");
    }

    [MenuItem("Scene/Level 1")]
    public static void Level1() {
        if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/level 1.unity");
    }

    [MenuItem("Scene/Level 2")]
    public static void Level2() {
        if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/level 2.unity");
    }

    [MenuItem("Scene/Tutorial")]
    public static void Tutorial() {
        if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Tutorial.unity");
    }
}