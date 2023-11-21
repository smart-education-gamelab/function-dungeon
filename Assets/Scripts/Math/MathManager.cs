using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MathManager : MonoBehaviour
{
    internal bool feedback;

    internal int questionsAnswered;
    internal int questionsWrong;
    public ExerciseList questionList;
    internal int index;

    private PlayerController player;
    private DialogueManager dialogueManager;
    [HideInInspector] public Math math;
    internal TriggerExercise questionOrigin;
    internal List<GameObject> doors;
    private AudioManager audioManager;
    internal SetDoors setDoors; 

    [HideInInspector] public List<Animator> torches;

    internal bool displayExerciseUI;
    [SerializeField] private Sprite correct;
    [SerializeField] private Sprite incorrect;

    [Header("Question UI")]
    [SerializeField] internal TextMeshProUGUI question;
    [SerializeField] internal Image image;
    [SerializeField] internal Image icon;
    [SerializeField] internal TextMeshProUGUI[] answers;
    [SerializeField] internal Text difficulty;

    [Header("After Failure")]
    [SerializeField] private GameObject failRoom;
    [SerializeField] private GameObject holePrefab;
    private Teleport ladder;
    internal int wrongAnsw;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        dialogueManager = gameObject.GetComponent<DialogueManager>();
        audioManager = gameObject.GetComponent<AudioManager>();
        ladder = FindObjectOfType<Teleport>();

        foreach (Math question in questionList.list)
        {
            if (question.used)
            {
                question.used = false;
            }
        }
    }

    private void CheckAnswer(int number)
    {
        foreach (TextMeshProUGUI ans in answers)
        {
            ans.transform.parent.gameObject.SetActive(false);       //Deactivates the buttons on the canvas
        }
        if (answers[number].text == LocalizationManager.Localize(math.GetCorrectLocalizationKey(), LocalizationTable.QUESTIONS))                   //Checks the answer.
        {
            audioManager.Play("Correct");
            Color c = icon.color;
            c.a = 0.3f;
            icon.color = c;
            icon.sprite = correct;
            if (!feedback)                                          //If the player is not being given feedback, doors will unlock, torches will be lit.
            {
                foreach (Animator torch in torches)
                {
                    TriggerAnimation.TriggerAnim(torch, "Trigger");
                }
                setDoors.UnlockAllDoors();
                questionsAnswered++;
                questionList.list[index].used = true;
                Destroy(questionOrigin.gameObject.transform.GetChild(0).gameObject);
                Destroy(questionOrigin);
            }
        }
        else
        {
            WrongAnswer();
            wrongAnsw = number;
        }

        StartCoroutine(closeUI(1));
        dialogueManager.EndDialogue();
    }

    public void OnPress_Answer1()
    {
        CheckAnswer(0);
    }

    public void OnPress_Answer2()
    {
        CheckAnswer(1);
    }

    public void OnPress_Answer3()
    {
        CheckAnswer(2);
    }

    public void OnPress_Answer4()
    {
        CheckAnswer(3);
    }

    private void WrongAnswer()
    {
        questionsWrong++;
        ladder.destination = player.transform.position;
        Color c = icon.color;
        c.a = 0.3f;
        icon.color = c;
        icon.sprite = incorrect;
        audioManager.Play("Wrong");
        StartCoroutine(StartPlayerFall(1));
    }

    IEnumerator StartPlayerFall(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        player.falling = true;
        GameObject hole = Instantiate(holePrefab, player.transform.position, player.transform.rotation);
        Destroy(hole, 1f);
        StartCoroutine(EndPlayerFall());
    }

    IEnumerator EndPlayerFall()
    {
        yield return new WaitForSeconds(0.7f);
        player.GetComponent<PlayerController>().falling = false;
        player.transform.position = new Vector2(failRoom.transform.position.x, failRoom.transform.position.y);
    }

    private IEnumerator closeUI(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        displayExerciseUI = false;
    }
}
