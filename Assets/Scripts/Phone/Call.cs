using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Call : MonoBehaviour
{
    private MathManager mathManager;
    private DialogueManager dialogueManager;
    internal int calls;
    internal int score;

    [SerializeField] private Dialogue dialogue;
    private Dialogue dialogueInst;

    private void Awake()
    {
        dialogueInst = Instantiate(dialogue);
        mathManager = FindObjectOfType<MathManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void OnButtonPress()
    {
        for (int i = 0; i < dialogue.content.Length; i++)
        {
            dialogueInst.content[i].sentences = dialogue.content[i].sentences.Replace("[WRONG1]", mathManager.math.wrong1);
            dialogueInst.content[i].sentences = dialogueInst.content[i].sentences.Replace("[WRONG2]", mathManager.math.wrong2);
        }

        dialogueManager.StartDialogue(dialogueInst);
        calls--;
    }
}
