using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ShowFeedback : MonoBehaviour
{
    private MathManager mathManager;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        mathManager = FindObjectOfType<MathManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void Feedback()
    {
        mathManager.feedback = true;
        mathManager.displayExerciseUI = true;

        for (int i = 0; i < mathManager.answers.Length; i++)
        {
            GameObject btn = mathManager.answers[i].transform.parent.gameObject;
            btn.SetActive(true);
            if (mathManager.answers[i].text != LocalizationManager.Localize(mathManager.math.GetCorrectLocalizationKey(), LocalizationTable.QUESTIONS))
            {
                btn.GetComponent<Button>().enabled = false;
            }
            else
            {
                btn.GetComponent<Image>().color = Color.green;
            }

            if (i == mathManager.wrongAnsw)
            {
                btn.GetComponent<Image>().color = Color.red;
            }
            

            
        }

        LocalizedString localized = new LocalizedString();
        localized.TableReference = "Table";
        localized.TableEntryReference = mathManager.math.id + "_FEEDBACK";

        //mathManager.math.dialogue.content[0].localizationKey = mathManager.math.feedback;
        mathManager.math.dialogue.content[0].localizationKey = localized;
        dialogueManager.AddDialogue(mathManager.math.dialogue);
    }
}
