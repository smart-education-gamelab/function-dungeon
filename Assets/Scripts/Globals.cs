using System;
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
    private CameraController cameraController;
    private PlayerController playerController;
    private EndScreenUI endScreenUI;
    private DialogueManager dialogueManager;
    private MathManager mathManager;
    public Inventory_UI[] inventoryUI;
    private EndScreen endScreen;
    private RequestUIManager requestUIManager;

    //Level generation
    private GenerationVariables variables = null;

    [Header("Resources")]
    public Sprite handIcon;
    public GameObject holePrefab;
    public GameObject multipleChoicePuzzlePrefab;

    public static SceneManager SceneManager => _Instance.sceneManager;
    public static UIManager UIManager => _Instance.uiManager;
    public static RandomManager RandomManager => _Instance.randomManager;
    public static PlayerController PlayerController => _Instance.playerController;
    public static EndScreenUI EndScreenUI => _Instance.endScreenUI;
    public static MathManager MathManager => _Instance.mathManager;
    public static DialogueManager DialogueManager => _Instance.dialogueManager;
    public static CameraController CameraController => _Instance.cameraController;
    public static Inventory_UI[] InventoryUI => _Instance.inventoryUI;
    public static EndScreen EndScreen => _Instance.endScreen;
    public static RequestUIManager RequestUIManager => _Instance.requestUIManager;

    //Level generation
    public static GenerationVariables LevelGenerationVariables => _Instance.variables;

    public static Sprite HandIcon => _Instance.handIcon;
    public static GameObject HolePrefab => _Instance.holePrefab;
    public static GameObject MultipleChoicePuzzlePrefab => _Instance.multipleChoicePuzzlePrefab;

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
        Utils.FindUniqueObject(out cameraController);
        Utils.FindUniqueObject(out endScreenUI);
        Utils.FindUniqueObject(out dialogueManager);
        Utils.FindUniqueObject(out mathManager);
        Utils.FindUniqueObject(out endScreen);
        Utils.FindUniqueObject(out requestUIManager);
    }

    public static void SetLevelGenerationVariables(GenerationVariables variables) {
        _Instance.variables = variables;
    }

    public static Globals GetInstance() {
        return _Instance;
    }
}
