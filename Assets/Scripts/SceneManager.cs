using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;

//Utility class for keeping track of our scenes and switching between them
public class SceneManager : MonoBehaviour {
    [Serializable]
    public class SceneDefinition {
        public string name;
        public SceneReference scene;

        public SceneDefinition() {
            name = "";
            scene = null;
        }
    }

    public static List<string> loadedScenes = new List<string>();
    public List<SceneDefinition> sceneDefinitions = new List<SceneDefinition>();
    private SceneDefinition activeScene;
    private bool isLoadingScene = false;

    private void Awake() {
        for (int i = 0; i < sceneDefinitions.Count; i++) {
            if (sceneDefinitions[i].scene.ScenePath.Contains(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".unity")) activeScene = sceneDefinitions[i];
        }

        if (activeScene == null) {
            Debug.LogError($"Current scene {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} not found in scene manager!");
        }
    }

    public bool IsLoadingScene() {
        return isLoadingScene;
    }

    public void SetScene(string sceneName) {
        var sceneDefinition = sceneDefinitions.Find(x => x.name == sceneName);

        if (sceneDefinition != null) {
            SetScene(sceneDefinition);
        } else Debug.LogError($"[SceneManager] scene has not been registered {sceneName}");
    }

    public void SetScene(SceneDefinition scene) {
        if (isLoadingScene) {
            Debug.LogError($"[SceneManager] scene can't be loaded because another scene is already being loaded");
            return;
        }
        Globals.UIManager.BlackScreenFadeIn(1.0f, true).OnComplete(() => StartCoroutine(LoadScene(scene))).SetUpdate(true);
    }

    private IEnumerator LoadScene(SceneDefinition scene) {
        //string oldSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        isLoadingScene = true;
        activeScene = scene;
        AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.scene, LoadSceneMode.Single);
        yield return loadOperation;

        //while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == oldSceneName) yield return null;
        isLoadingScene = false;
        print("Active scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        //TODO: This is dirty
        if (scene.name != "LevelGeneration")
            Globals.UIManager.BlackScreenFadeOut(1.0f);
    }

    public static void LoadSceneIfNotActive(string sceneName) {
        if (loadedScenes.Contains(sceneName)) return;
        loadedScenes.Add(sceneName);
        print("Loaded scene " + sceneName + " + " + loadedScenes.Count);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public List<SceneDefinition> GetAllLevelScenes() {
        return sceneDefinitions;
    }

    public SceneDefinition GetActiveScene() {
        return activeScene;
    }

    public void ReloadScene() {
        Globals.UIManager.BlackScreenFadeIn(1.0f, true).OnComplete(() => SetScene(activeScene));
    }
}
