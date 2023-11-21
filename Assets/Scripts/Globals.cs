using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour {
    private bool isInitialized = false;

    private static Globals _Instance;
    private SceneManager sceneManager;
    private UIManager uiManager;

    public static SceneManager SceneManager => _Instance.sceneManager;
    public static UIManager UIManager => _Instance.uiManager;

    private void Awake() {
        if (_Instance != null) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        } else {
            _Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public static bool IsInitialized() {
        return _Instance != null && _Instance.isInitialized;
    }

    public static void Initialize() {
        if (_Instance == null) Debug.LogError("No globals found in scene.");
        _Instance.GlobalInitialize();
        _Instance.isInitialized = true;
    }

    private void GlobalInitialize() {
        Utils.FindUniqueObject(out sceneManager);
        Utils.FindUniqueObject(out uiManager);
    }

    public static Globals GetInstance() {
        return _Instance;
    }
}
