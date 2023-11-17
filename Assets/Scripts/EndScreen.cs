using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    internal bool displayEndScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Trigger_EndScreen();
        }
    }

    private void Trigger_EndScreen()
    {
        displayEndScreen = true;
        Globals.EndScreenUI.scoreTxt.text = Globals.MathManager.questionsAnswered.ToString() + " out of " + (FindObjectsOfType<SetExercise>().Length + " questions answered");
        Globals.EndScreenUI.helpedTxt.text = FindObjectOfType<Call>().score.ToString() + " out of " + GameObject.FindGameObjectsWithTag("NPC").Length + " people helped";
        Globals.EndScreenUI.wrongTxt.text = Globals.MathManager.questionsWrong.ToString() + " questions answered incorrectly";
    }

    public void OnPress_ToTitleScreen()
    {
        //SceneManager.LoadScene(0);
    }

    public void OnPress_Continue()
    {
        displayEndScreen = false;
    }
}
