using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : Movement
{
    [Header("Game Managers")]
    private DialogueManager dialogue;
    private Animator anim;

    [HideInInspector] public bool allowHorizontalInput;
    [HideInInspector] public bool allowVerticalInput;
    [HideInInspector] public bool allowSpaceInput;
    [HideInInspector] public bool falling;

    private GameObject interactableObject;
    [HideInInspector] public int dialogueState;
    private DialogueManager dialogueManager;

    private MathManager mathManager;

    public virtual void Start()
    {
        dialogue = GameObject.FindObjectOfType<DialogueManager>();
        mathManager = GameObject.FindObjectOfType<MathManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        anim = gameObject.GetComponent<Animator>();

        dialogueState = -1;

        allowVerticalInput = true;
        allowHorizontalInput = true;
        allowSpaceInput = true;
    }

    public override void Update()
    {
        base.Update();

        if (anim != null)
        {
            anim.SetInteger("Horizontal", (int)directionX);
            anim.SetInteger("Vertical", (int)directionY);
            anim.SetBool("wrong", falling);
        }

        if (falling || dialogue.displayDialogueUI || mathManager.displayExerciseUI)
        {
            allowHorizontalInput = false;
            allowVerticalInput = false;
        }
        else
        {
            allowHorizontalInput = true;
            allowVerticalInput = true;
        }

        if (allowHorizontalInput)
        {
            directionX = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            directionX = 0; //Makes it so the player stops walking if input is turned off.
        }

        if (allowVerticalInput)
        {
            directionY = Input.GetAxisRaw("Vertical");
        }
        else
        {
            directionY = 0; //Makes it so the player stops walking if input is turned off.
        }

        switch (dialogueState)
        {
            case -1:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RequestedItems requests = gameObject.GetComponent<RequestedItems>();
                    foreach (Requests request in requests.requests)
                    {
                        if (request != null)
                        {
                            if (Mathf.Round(transform.position.x) == request.coordinates.x && Mathf.Round(transform.position.y) == request.coordinates.y)
                            {
                                gameObject.GetComponent<PlayerInventory>().Pickup(request.item.item_GO);
                                Destroy(request.giver);
                                requests.requests.Remove(request);
                                break;
                            }
                        }
                    }
                }

                if (dialogueManager.displayDialogueUI)
                {
                    dialogueState = 1;
                }

                break;
            case 0:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    dialogueState += 1;
                    if (interactableObject.GetComponent<DialogueTrigger>() != null)
                    {
                        interactableObject.GetComponent<DialogueTrigger>().TriggerDialogue();
                    }

                    if (interactableObject.GetComponent<TriggerExercise>() != null)
                    {
                        interactableObject.GetComponent<TriggerExercise>().OnTrigger();
                    }

                    if (interactableObject.GetComponent<Pickup>() != null)
                    {
                        gameObject.GetComponent<PlayerInventory>().Pickup(interactableObject);
                    }

                    if (interactableObject.GetComponent<ItemRequest>() != null)
                    {
                        interactableObject.GetComponent<ItemRequest>().OnInteraction(gameObject.GetComponent<PlayerInventory>());
                    }

                    if (interactableObject.GetComponent<SetRequest>() != null)
                    {
                        gameObject.GetComponent<RequestedItems>().requests.Add(interactableObject.GetComponent<SetRequest>().Set());
                    }

                    if (interactableObject.GetComponent<SetRequestUI>() != null)
                    {
                        interactableObject.GetComponent<SetRequestUI>().SetUI();
                    }

                    if (interactableObject.GetComponent<ShowFeedback>() != null)
                    {
                        interactableObject.GetComponent<ShowFeedback>().Feedback();
                    }

                    //dialogueState += 1;
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    dialogue.DisplayNextSentence();
                }
                //After the conversation has ended the dialogueState will be reset in the EndDialogue function of DialogueManager
                break;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (dialogueState == -1)
        {
            if (other.gameObject.CompareTag("Interactable") || other.gameObject.CompareTag("NPC"))
            {
                interactableObject = other.gameObject;
                dialogueState = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable") && dialogue.sentences.Count == 0 || other.gameObject.CompareTag("NPC") && dialogue.sentences.Count == 0)
        {
            dialogueState = -1;
        }
    }
}
