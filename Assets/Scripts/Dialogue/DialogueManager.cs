using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using static PlayerController;

public class DialogueManager : MonoBehaviour
{
    internal bool displayDialogueUI;

    [Header("UI")]
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed;

    public Queue<string> sentences;
    private Queue<Sprite> sprites;

    private void Awake()
    {
        sentences = new Queue<string>();
        sprites = new Queue<Sprite>();
    }

    public void AddDialogue(Dialogue dialogue)
    {
        for (int i = 0; i < dialogue.content.Length; i++)
        {
            sprites.Enqueue(dialogue.content[i].sprite);

            if (dialogue.content[i].localizationOverride.Length != 0) {
                sentences.Enqueue(LocalizationManager.Localize(dialogue.content[i].localizationOverride, LocalizationTable.QUESTIONS));
            } else sentences.Enqueue(dialogue.content[i].localizationKey.GetLocalizedString());
        }
    }

    public void StartCustomDialogue(Sprite[] customSprites, string[] customSentences) {
        displayDialogueUI = true;

        foreach (Sprite sprite in customSprites) sprites.Enqueue(sprite);
        foreach (string sentence in customSentences) sentences.Enqueue(sentence);

        if (sentences.Count == customSentences.Length) {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        displayDialogueUI = true;
        AddDialogue(dialogue);

        if (sentences.Count == dialogue.content.Length)
        {
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Sprite sprite = sprites.Dequeue();
        if (sprite != null)
        {
            characterSprite.sprite = sprite;
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void EndDialogue()
    {
        displayDialogueUI = false;
        Globals.PlayerController.state = PlayerState.Idle;
    }
}

