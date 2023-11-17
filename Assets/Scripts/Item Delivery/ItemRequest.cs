using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequest : SetRequest
{
    [SerializeField] GameObject targetObject;

    private DialogueManager dialogueManager;
    [SerializeField] private Sprite updatedSprite;

    [Header("Dialogues")]
    [SerializeField] private Dialogue successDialogue;
    [SerializeField] private Dialogue failureDialogue;

    private Call call;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        call = FindObjectOfType<Call>();
    }

    public void OnInteraction(Inventory target)
    {
        if (ItemCheck.CheckItems(target, this.request.item))
        {
            dialogueManager.StartDialogue(successDialogue);
            call.calls++;
            call.score++;
            targetObject.GetComponent<SpriteRenderer>().sprite = updatedSprite;
            Destroy(this);
        }
        else
        {
            dialogueManager.StartDialogue(failureDialogue);
        }
    }
}
