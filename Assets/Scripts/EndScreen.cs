using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class EndScreen : MonoBehaviour {
    internal bool displayEndScreen;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Trigger_EndScreen();
        }
    }

    private void Trigger_EndScreen() {
        displayEndScreen = true;
        Globals.EndScreenUI.scoreTxt.text = Globals.MathManager.questionsAnswered.ToString() + " out of " + (FindObjectsOfType<SetExercise>().Length + " questions answered");
        string score = FindObjectOfType<Call>(true).score.ToString();
        int actualNPCCount = 0;
        foreach(GameObject npc in GameObject.FindGameObjectsWithTag("NPC")) {
            if (npc.activeSelf) {
                actualNPCCount++;
            }
        }
        Globals.EndScreenUI.helpedTxt.text = score + " out of " + actualNPCCount + " people helped";
        Globals.EndScreenUI.wrongTxt.text = Globals.MathManager.questionsWrong.ToString() + " questions answered incorrectly";
    }
}
