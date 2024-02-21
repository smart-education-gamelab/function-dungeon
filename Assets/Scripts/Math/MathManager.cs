﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;

public class MathManager : MonoBehaviour {
    [NonSerialized] public CustomPuzzle customPuzzle;
    internal bool feedback;

    internal int questionsAnswered;
    internal int questionsWrong;
    public QuestionList questionList;
    internal int index;

    private DialogueManager dialogueManager;
    [HideInInspector] public Question activeQuestion;
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
    internal string wrongAnsw;

    private void Start() {
        //Ensure we can generate consistent levels with the same questions.
        Globals.RandomManager.InitializeStateRandomly(RandomState.EXERCISE);
        LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator != null) {
            GenerationVariables variables = levelGenerator.GetGenerationVariables();
            if (variables != null && !variables.randomizedQuestions) {
                Globals.RandomManager.InitializeState(RandomState.EXERCISE, variables.seed);
            }
        }

        dialogueManager = gameObject.GetComponent<DialogueManager>();
        audioManager = gameObject.GetComponent<AudioManager>();
    }

    private void ExportQuestions() {
        string str = "";
        foreach (var question in questionList.questions) {
            if (!question.enabled || (question.type != QuestionType.MULTIPLECHOICE && question.type != QuestionType.MULTIPLECHOICEGAMIFIED)) continue;
            str += question.name + "\n";
            str += "Question: " + question.text[1].question + "\n";
            str += "Correct answer: " + question.text[1].correct + "\n";
            str += "Wrong answer 1: " + question.text[1].wrong1 + "\n";
            str += "Wrong answer 2: " + question.text[1].wrong2 + "\n";
            str += "Wrong answer 3: " + question.text[1].wrong3 + "\n";
            str += "Feedback: " + question.text[1].feedback + "\n";
            if (question.image)
                str += "Image: " + question.image.name + "\n\n";
            else str += "\n";
        }

        GUIUtility.systemCopyBuffer = str;
    }

    public void ResetQuestions() {
        foreach (Question question in questionList.questions) {
            if (question.used) {
                question.used = false;
            }
        }
    }

    private void CheckAnswer(int number) {
        foreach (TextMeshProUGUI ans in answers) {
            ans.transform.parent.gameObject.SetActive(false);       //Deactivates the buttons on the canvas
        }
        if (answers[number].text == LocalizationManager.Localize(activeQuestion.GetCorrectLocalizationKey(), LocalizationTable.QUESTIONS))                   //Checks the answer.
        {
            audioManager.Play("Correct");
            Color c = icon.color;
            c.a = 0.3f;
            icon.color = c;
            icon.sprite = correct;
            if (!feedback)                                          //If the player is not being given feedback, doors will unlock, torches will be lit.
            {
                OnQuestionComplete();
            }
        } else {
            WrongAnswer();
            wrongAnsw = answers[number].text;
        }

        StartCoroutine(closeUI(1));
        dialogueManager.EndDialogue();
    }

    //Triggers when the player has succesfully answered a question or executed a puzzle
    public void OnQuestionComplete() {
        foreach (Animator torch in torches) {
            TriggerAnimation.TriggerAnim(torch, "Trigger");
        }
        setDoors.UnlockAllDoors();
        questionsAnswered++;
        questionList.questions[index].used = true;
        Destroy(questionOrigin.gameObject.transform.GetChild(0).gameObject);
        Destroy(questionOrigin);
    }

    public void OnPress_Answer1() {
        CheckAnswer(0);
    }

    public void OnPress_Answer2() {
        CheckAnswer(1);
    }

    public void OnPress_Answer3() {
        CheckAnswer(2);
    }

    public void OnPress_Answer4() {
        CheckAnswer(3);
    }

    private void WrongAnswer() {
        questionsWrong++;
        FindObjectOfType<Teleport>().destination = Globals.PlayerController.transform.position;
        Color c = icon.color;
        c.a = 0.3f;
        icon.color = c;
        icon.sprite = incorrect;
        audioManager.Play("Wrong");
        FailRoom failRoom = FindObjectOfType<FailRoom>();
        Globals.PlayerController.FallAndTeleport(new Vector2(failRoom.spawnPos.position.x, failRoom.spawnPos.position.y), null);
    }

    private IEnumerator closeUI(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        displayExerciseUI = false;
    }

    public QuestionList GetQuestionList() {
        return questionList;
    }
}
