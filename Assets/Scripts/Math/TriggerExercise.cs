using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerExercise : MonoBehaviour {
    private SetExercise setExercise;
    private SetDoors setDoors;

    private int index;

    [SerializeField] private GameObject particles;

    Color btnCol;

    private void Start() {
        setExercise = transform.parent.gameObject.GetComponent<SetExercise>();
        setDoors = transform.parent.gameObject.GetComponent<SetDoors>();

        particles = transform.Find("Particle System").gameObject;

        btnCol = Globals.MathManager.answers[0].transform.parent.gameObject.GetComponent<Image>().color;

        DOTween.Shake(() => transform.position, x => transform.position = x, 10, 0.01f, 5, 50, true, false).SetLoops(-1);
    }

    public void OnTrigger() {
        if (Globals.MathManager.displayExerciseUI || Globals.MathManager.customPuzzle) return;
        Question question = GetQuestion();
        Globals.MathManager.feedback = false;
        Globals.MathManager.questionOrigin = this;
        Globals.MathManager.activeQuestion = question;
        Globals.MathManager.setDoors = setDoors;
        Globals.MathManager.index = index;
        Globals.MathManager.torches = setExercise.Torches();

        UpdateUI();
        if (question.type == QuestionType.MULTIPLECHOICE) {
            Globals.MathManager.displayExerciseUI = true;
            Globals.DialogueManager.StartDialogue(setExercise.dialogue);
        } else { //If its a custom puzzle question
            if (question.type == QuestionType.MULTIPLECHOICEGAMIFIED) {
                Globals.MathManager.customPuzzle = Instantiate(Globals.MultipleChoicePuzzlePrefab).GetComponent<CustomPuzzle>();
            } else if (question.type == QuestionType.CUSTOM) {
                Globals.MathManager.customPuzzle = Instantiate(question.puzzlePrefab).GetComponent<CustomPuzzle>();
            }
            Globals.PlayerController.FallAndTeleport(Globals.MathManager.customPuzzle.playerSpawnPoint.position, Globals.MathManager.customPuzzle.OnPlayerEnter);
            Globals.MathManager.customPuzzle.question = question;
        }
    }

    private Question GetQuestion() {
        List<int> usedUsable = new List<int>();
        List<int> usable = new List<int>();

        foreach (Question question in Globals.MathManager.questionList.questions) {
            if (!question.enabled) continue;
            if (Constants.learningGoalLevels[question.learningGoalLevel] == transform.parent.GetComponent<SetExercise>().section) {
                usedUsable.Add(Globals.MathManager.questionList.questions.IndexOf(question));
                if (!question.used) {
                    usable.Add(Globals.MathManager.questionList.questions.IndexOf(question));
                }
            }
        }

        //Make sure we fail gracefully when there are no 'unused' questions left
        if (usable.Count == 0) {
            int random = Globals.RandomManager.Range(RandomState.EXERCISE, 0, usedUsable.Count);
            index = usedUsable[random];
            return Globals.MathManager.questionList.questions[index];
        } else {
            int random = Globals.RandomManager.Range(RandomState.EXERCISE, 0, usable.Count);
            index = usable[random];
            return Globals.MathManager.questionList.questions[index];
        }
    }

    public void UpdateUI() {
        Globals.MathManager.question.text = LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetQuestionLocalizationKey(), LocalizationTable.QUESTIONS);
        Globals.MathManager.image.sprite = Globals.MathManager.activeQuestion.image;

        Color c = Globals.MathManager.icon.color;
        c.a = 0f;
        Globals.MathManager.icon.color = c;

        ShuffleAnswers();

        if (Globals.MathManager.activeQuestion.image == null) {
            Globals.MathManager.image.gameObject.SetActive(false);
        } else {
            Globals.MathManager.image.gameObject.SetActive(true);
        }

        for (int i = 0; i < Globals.MathManager.answers.Length; i++) {
            GameObject btn = Globals.MathManager.answers[i].transform.parent.gameObject;
            btn.SetActive(true);
            btn.GetComponent<Button>().enabled = true;
            btn.GetComponent<Image>().color = btnCol;
        }
    }

    private void ShuffleAnswers() {
        List<string> shuffle = new List<string>() { Globals.MathManager.activeQuestion.GetCorrectLocalizationKey(), Globals.MathManager.activeQuestion.GetWrong1LocalizationKey(), Globals.MathManager.activeQuestion.GetWrong2LocalizationKey(), Globals.MathManager.activeQuestion.GetWrong3LocalizationKey() };

        for (int i = 0; i < Globals.MathManager.answers.Length; i++) {
            int random = UnityEngine.Random.Range(0, shuffle.Count);
            Globals.MathManager.answers[i].text = LocalizationManager.Localize(shuffle[random], LocalizationTable.QUESTIONS);
            Globals.MathManager.answers[i].enabled = true;
            if (DebugHelper.EasyQuestions() && Globals.MathManager.answers[i].text != LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetCorrectLocalizationKey(), LocalizationTable.QUESTIONS)) {
                Globals.MathManager.answers[i].enabled = false;
            }
            shuffle.Remove(shuffle[random]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            particles.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            particles.SetActive(false);
        }
    }
}
