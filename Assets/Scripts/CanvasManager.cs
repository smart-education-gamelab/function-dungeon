using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour {
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
        call = FindObjectOfType<Call>();
    }

    private void Start() {
        canvas.SetActive(true);
    }

    private void Update() {
        if (Globals.DialogueManager.displayDialogueUI) {
            dialogueUI.SetActive(true);
            inventoryUI.SetActive(false);
        } else {
            inventoryUI.SetActive(true);
        }
        if (Globals.MathManager.displayExerciseUI && Globals.DialogueManager.sentences.Count == 0) {
            dialogueUI.SetActive(true);
            exerciseUI.SetActive(true);
        }

        if (!Globals.DialogueManager.displayDialogueUI && !Globals.MathManager.displayExerciseUI) {
            dialogueUI.SetActive(false);
            exerciseUI.SetActive(false);
        }

        if (Globals.EndScreen.displayEndScreen) {
            endScreenUI.SetActive(true);
            canvas.SetActive(false);
        } else {
            endScreenUI.SetActive(false);
            canvas.SetActive(true);
        }

        if (!Globals.DialogueManager.displayDialogueUI && Globals.RequestUIManager.displayRequestUI) {
            requestUI.SetActive(true);
        } else {
            requestUI.SetActive(false);
        }

        if (Globals.MathManager.displayExerciseUI && call.calls > 0) {
            phoneBtn.SetActive(true);
        } else {
            phoneBtn.SetActive(false);
            callsLeft.text = call.calls.ToString();
        }
    }
}
