using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour {
    private DialogueManager dialogueManager;
    private MathManager mathManager;
    private EndScreen endScreen;
    private RequestUIManager requestManager;
    private Call call;
    [SerializeField] private TextMeshProUGUI callsLeft;

    [Header("UI Objects")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject exerciseUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject endScreenUI;
    [SerializeField] private GameObject requestUI;
    [SerializeField] private GameObject phoneBtn;

    private void Awake() {
        dialogueManager = GameObject.Find("Manager").GetComponent<DialogueManager>();
        mathManager = GameObject.Find("Manager").GetComponent<MathManager>();
        endScreen = FindObjectOfType<EndScreen>();
        requestManager = FindObjectOfType<RequestUIManager>();
        call = FindObjectOfType<Call>();
    }

    private void Start() {
        canvas.SetActive(true);
    }

    private void Update() {
        if (dialogueManager.displayDialogueUI) {
            dialogueUI.SetActive(true);
            inventoryUI.SetActive(false);
        } else {
            inventoryUI.SetActive(true);
        }
        if (mathManager.displayExerciseUI && dialogueManager.sentences.Count == 0) {
            dialogueUI.SetActive(true);
            exerciseUI.SetActive(true);
        }

        if (!dialogueManager.displayDialogueUI && !mathManager.displayExerciseUI) {
            dialogueUI.SetActive(false);
            exerciseUI.SetActive(false);
        }

        if (endScreen.displayEndScreen) {
            endScreenUI.SetActive(true);
            canvas.SetActive(false);
        } else {
            endScreenUI.SetActive(false);
            canvas.SetActive(true);
        }

        if (!dialogueManager.displayDialogueUI && requestManager.displayRequestUI) {
            requestUI.SetActive(true);
        } else {
            requestUI.SetActive(false);
        }

        if (mathManager.displayExerciseUI && call.calls > 0) {
            phoneBtn.SetActive(true);
        } else {
            phoneBtn.SetActive(false);
            callsLeft.text = call.calls.ToString();
        }
    }
}
