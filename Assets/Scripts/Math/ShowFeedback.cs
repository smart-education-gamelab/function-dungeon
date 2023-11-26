using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ShowFeedback : MonoBehaviour {
    public void Feedback() {
        Globals.MathManager.feedback = true;
        Globals.MathManager.displayExerciseUI = true;

        for (int i = 0; i < Globals.MathManager.answers.Length; i++) {
            GameObject btn = Globals.MathManager.answers[i].transform.parent.gameObject;
            btn.SetActive(true);
            if (Globals.MathManager.answers[i].text != LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetCorrectLocalizationKey(), LocalizationTable.QUESTIONS)) {
                btn.GetComponent<Button>().enabled = false;
                btn.GetComponent<Image>().color = Color.white;
            } else {
                btn.GetComponent<Image>().color = Color.green;
            }

            if (Globals.MathManager.answers[i].text.Equals(Globals.MathManager.wrongAnsw)) {
                btn.GetComponent<Image>().color = Color.red;
            }
        }

        LocalizedString localized = new LocalizedString();
        localized.TableReference = "Questions";
        localized.TableEntryReference = Globals.MathManager.activeQuestion.GetFeedbackLocalizationKey();

        //mathManager.math.dialogue.content[0].localizationKey = mathManager.math.feedback;
        Globals.MathManager.activeQuestion.dialogue.content[0].localizationKey = localized;
        Globals.DialogueManager.AddDialogue(Globals.MathManager.activeQuestion.dialogue);
    }
}
