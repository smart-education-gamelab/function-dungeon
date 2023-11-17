using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    private DialogueManager dialogueManager;
    [SerializeField] private Dialogue dialogue;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(dialogue);
            Destroy(gameObject);
        }
    }
}
