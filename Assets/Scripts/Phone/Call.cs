using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Call : MonoBehaviour {
    internal int calls;
    internal int score;

    [SerializeField] private Dialogue dialogue;
    private Dialogue dialogueInst;

    private void Awake() {
        dialogueInst = Instantiate(dialogue);
    }

    public void OnButtonPress() {
        string[] customSentences = new string[dialogue.content.Length];
        Sprite[] customSprites = new Sprite[dialogue.content.Length];

        for (int i = 0; i < dialogue.content.Length; i++) {
            customSprites[i] = dialogue.content[i].sprite;
            customSentences[i] = dialogue.content[i].localizationKey.GetLocalizedString();
            customSentences[i] = customSentences[i].Replace("[WRONG1]", LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetWrong1LocalizationKey(), LocalizationTable.QUESTIONS));
            customSentences[i] = customSentences[i].Replace("[WRONG2]", LocalizationManager.Localize(Globals.MathManager.activeQuestion.GetWrong2LocalizationKey(), LocalizationTable.QUESTIONS));
        }
        Globals.DialogueManager.StartCustomDialogue(customSprites, customSentences);
        calls--;
    }
}
