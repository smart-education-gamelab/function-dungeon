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
        scoreTxt.text = mathManager.questionsAnswered.ToString() + " / " + (FindObjectsOfType<SetExercise>().Length + " kysymykseen vastattu");
        helpedTxt.text = call.score.ToString() + " / " + GameObject.FindGameObjectsWithTag("NPC").Length + " henkilöä autettu";
        wrongTxt.text = mathManager.questionsWrong.ToString() + " kysymykseen vastasit väärin";
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
