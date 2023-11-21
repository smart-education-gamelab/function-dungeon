using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour {
    private bool isInitialized = false;

    private static Globals _Instance;
    private SceneManager sceneManager;
    private UIManager uiManager;
    private RandomManager randomManager;
    public PlayerController playerController;
    public EndScreenUI endScreenUI;
    public DialogueManager dialogueManager;
    public MathManager mathManager;

    [Header("Resources")]
    public Sprite handIcon;
    public GameObject holePrefab;

    public static SceneManager SceneManager => _Instance?.sceneManager;
    public static UIManager UIManager => _Instance?.uiManager;
    public static RandomManager RandomManager => _Instance?.randomManager;
    public static PlayerController PlayerController => _Instance?.playerController;
    public static EndScreenUI EndScreenUI => _Instance?.endScreenUI;
    public static MathManager MathManager => _Instance?.mathManager;
    public static DialogueManager DialogueManager => _Instance?.dialogueManager;

    public static Sprite HandIcon => _Instance?.handIcon;
    public static GameObject HolePrefab=> _Instance?.holePrefab;

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
        Utils.FindUniqueObject(out randomManager);
        Utils.FindUniqueObject(out playerController);
    }

    public static Globals GetInstance() {
        return _Instance;
    }
}
