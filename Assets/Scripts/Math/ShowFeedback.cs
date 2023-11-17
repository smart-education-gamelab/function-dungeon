using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if (mathManager.answers[i].text != mathManager.math.correct)
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

        mathManager.math.dialogue.content[0].sentences = mathManager.math.feedback;
        dialogueManager.AddDialogue(mathManager.math.dialogue);
    }
}
