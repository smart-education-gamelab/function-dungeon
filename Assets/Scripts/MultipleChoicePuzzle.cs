using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MultipleChoicePuzzle;

public class MultipleChoicePuzzle : CustomPuzzle {
    [Serializable]
    public class AnswerTextPair {
        public TMPro.TextMeshPro text;
        public Transform pos;
    }

    public Tilemap layoutTilemap;
    public Transform cameraFocus;
    public float cameraFocusSize = 6;
    public PuzzleSprite puzzleSprite;

    public TMPro.TextMeshPro questionText;
    public Carryable answerStone;
    public PuzzleLadder ladder;

    public List<AnswerTextPair> answerTextPairs = new List<AnswerTextPair>();

    public override void OnPlayerEnter() {
        Globals.CameraController.DetachCamera(cameraFocus, cameraFocusSize, false, 0);
    }

    public void Start() {
        //Shuffle answers
        answerTextPairs = answerTextPairs.OrderBy(x => UnityEngine.Random.value).ToList();
        questionText.text = LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetQuestionLocalizationKey(), LocalizationTable.QUESTIONS);
        answerTextPairs[0].text.text = LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetCorrectLocalizationKey(), LocalizationTable.QUESTIONS);
        answerTextPairs[1].text.text = LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetWrong1LocalizationKey(), LocalizationTable.QUESTIONS);
        answerTextPairs[2].text.text = LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetWrong2LocalizationKey(), LocalizationTable.QUESTIONS);
        answerTextPairs[3].text.text = LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetWrong3LocalizationKey(), LocalizationTable.QUESTIONS);

        puzzleSprite.SetSprite(question.image);
    }

    private Color backupColor;
    private AnswerTextPair previousAnswer;
    public void Update() {
        //Find currently selected answer
        AnswerTextPair closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        if (Globals.PlayerController.currentHeldItem != answerStone) {
            foreach (AnswerTextPair answerTextPair in answerTextPairs) {

                Vector3 directionToTarget = answerTextPair.pos.position - answerStone.transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr) {
                    closestDistanceSqr = dSqrToTarget;
                    closest = answerTextPair;
                }
            }
        }

        if (closestDistanceSqr < 0.5f && previousAnswer != closest) {
            previousAnswer = closest;
            if (previousAnswer != null) previousAnswer.text.color = backupColor;
            backupColor = closest.text.color;
            if (closest == answerTextPairs[0]) {
                closest.text.color = Color.green;
                ladder.gameObject.SetActive(true);
            } else {
                closest.text.color = Color.red;
                MathManager mathManager = FindObjectOfType<MathManager>();
                Globals.MathManager.wrongAnsw = answerTextPairs.IndexOf(closest);
                mathManager.questionsWrong++;
                FindObjectOfType<Teleport>().destination = Globals.PlayerController.transform.position;
                FindObjectOfType<AudioManager>().Play("Wrong");
                FailRoom failRoom = FindObjectOfType<FailRoom>();
                Globals.PlayerController.FallAndTeleport(new Vector2(failRoom.spawnPos.position.x, failRoom.spawnPos.position.y), OnWrongAnswerFall);
            }
        }
    }

    private void OnWrongAnswerFall() {
        Globals.CameraController.ReattachCamera(0);
    }
}
