using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    internal bool displayEndScreen;
    private MathManager mathManager;
    [SerializeField] private Call call;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI helpedTxt;
    [SerializeField] private TextMeshProUGUI wrongTxt;

    private void Awake()
    {
        mathManager = FindObjectOfType<MathManager>();
    }

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
        scoreTxt.text = mathManager.questionsAnswered.ToString() + " out of " + (FindObjectsOfType<SetExercise>().Length + " questions answered");
        helpedTxt.text = call.score.ToString() + " out of " + GameObject.FindGameObjectsWithTag("NPC").Length + " people helped";
        wrongTxt.text = mathManager.questionsWrong.ToString() + " questions answered incorrectly";
    }

    public void OnPress_ToTitleScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void OnPress_Continue()
    {
        displayEndScreen = false;
    }
}
