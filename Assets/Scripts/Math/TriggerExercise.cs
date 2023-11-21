using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerExercise : MonoBehaviour
{
    [SerializeField] internal MathManager mathManager;
    private DialogueManager dialogueManager;
    private SetExercise setExercise;
    private SetDoors setDoors;

    private int index;

    [SerializeField] private GameObject particles;

    Color btnCol;

    private void Awake()
    {
        mathManager = FindObjectOfType<MathManager>();
        dialogueManager = GameObject.Find("Manager").GetComponent<DialogueManager>();
        setExercise = transform.parent.gameObject.GetComponent<SetExercise>();
        setDoors = transform.parent.gameObject.GetComponent<SetDoors>();

        particles = transform.Find("Particle System").gameObject;

        btnCol = mathManager.answers[0].transform.parent.gameObject.GetComponent<Image>().color;

        DOTween.Shake(() => transform.position, x => transform.position = x, 10, 0.01f, 5, 50, true, false).SetLoops(-1);
    }

    public void OnTrigger()
    {
        mathManager.feedback = false;
        mathManager.displayExerciseUI = true;
        mathManager.questionOrigin = this;
        dialogueManager.StartDialogue(setExercise.dialogue);
        mathManager.math = GetExercise();
        mathManager.setDoors = setDoors;
        mathManager.index = index;
        UpdateUI();
        mathManager.torches = setExercise.Torches();
    }

    private Math GetExercise()
    {
        List<int> usable = new List<int>();

        foreach (Math question in mathManager.questionList.list)
        {
            if (question.section == transform.parent.GetComponent<SetExercise>().section && !question.used)
            {
                usable.Add(System.Array.IndexOf(mathManager.questionList.list, question));
            }
        }

        int random = Random.Range(0, usable.Count);
        index = usable[random];

        return mathManager.questionList.list[index];
    }

    public void UpdateUI()
    {
        mathManager.question.text = mathManager.math.question;
        mathManager.image.sprite = mathManager.math.image;

        Color c = mathManager.icon.color;
        c.a = 0f;
        mathManager.icon.color = c;

        ShuffleAnswers();

        if (mathManager.math.image == null)
        {
            mathManager.image.gameObject.SetActive(false);
        }
        else
        {
            mathManager.image.gameObject.SetActive(true);
        }

        for (int i = 0; i < mathManager.answers.Length; i++)
        {
            GameObject btn = mathManager.answers[i].transform.parent.gameObject;
            btn.SetActive(true);
            btn.GetComponent<Button>().enabled = true;
            btn.GetComponent<Image>().color = btnCol;

        }
    }

    private void ShuffleAnswers()
    {
        List<string> shuffle = new List<string>() { mathManager.math.correct, mathManager.math.wrong1, mathManager.math.wrong2, mathManager.math.wrong3 };

        for (int i = 0; i < mathManager.answers.Length; i++)
        {
            int random = Random.Range(0, shuffle.Count);
            mathManager.answers[i].text = shuffle[random];
            mathManager.answers[i].enabled = true;
            if (DebugHelper.EasyQuestions() && mathManager.answers[i].text != mathManager.math.correct) {
                mathManager.answers[i].enabled = false;
            }
            shuffle.Remove(shuffle[random]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            particles.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            particles.SetActive(false);
        }
    }
}
