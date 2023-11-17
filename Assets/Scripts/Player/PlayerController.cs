using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : Movement {
    [SerializeField] private Animator anim;
    [SerializeField] private Transform itemHoldPosition; // The position where the item will be displayed above the player's head.

    public float carriableSearchRadius = 0.5f;

    [HideInInspector] public Carryable currentHeldItem; // The current item above the player's head.
    private GameObject interactableObject;
    private Carryable closestCarryable;
    private List<Carryable> nearbyCarriables = new List<Carryable>();
    private Vector3 lookDirection;

    public delegate void OnFinishedFalling();
    private OnFinishedFalling onFinishedFalling;

    [HideInInspector] public bool allowHorizontalInput;
    [HideInInspector] public bool allowVerticalInput;
    [HideInInspector] public bool allowSpaceInput;

    public PlayerState state = PlayerState.Idle;

    public enum PlayerState {
        Idle,
        Interacting,
        InDialogue,
        Falling
    }


    public virtual void Start() {
        anim = gameObject.GetComponent<Animator>();

        allowVerticalInput = true;
        allowHorizontalInput = true;
        allowSpaceInput = true;
    }

    public bool IsPlayerMoving() {
        return directionX != 0 || directionY != 0;
    }

    public override void Update() {
        if (Time.timeScale == 0.0f) return;
        base.Update();

        UpdateLookDirection();

        if (closestCarryable != null) closestCarryable.SetInteractable(false);
        closestCarryable = GetClosestCarryable();
        closestCarryable?.SetInteractable(true);

        if (anim != null) {
            anim.SetInteger("Horizontal", (int)directionX);
            anim.SetInteger("Vertical", (int)directionY);
        }

        if (state == PlayerState.Falling || Globals.DialogueManager.displayDialogueUI || Globals.MathManager.displayExerciseUI) {
            allowHorizontalInput = false;
            allowVerticalInput = false;
        } else {
            allowHorizontalInput = true;
            allowVerticalInput = true;
        }

        if (allowHorizontalInput && state != PlayerState.Falling) {
            directionX = Input.GetAxisRaw("Horizontal");
        } else {
            directionX = 0; //Makes it so the player stops walking if input is turned off.
        }

        if (allowVerticalInput && state != PlayerState.Falling) {
            directionY = Input.GetAxisRaw("Vertical");
        } else {
            directionY = 0; //Makes it so the player stops walking if input is turned off.
        }

        switch (state) {
            case PlayerState.Idle:
                if (Input.GetKeyDown(KeyCode.E)) {
                    if (currentHeldItem != null) {
                        Vector3 targetPosition = transform.position + lookDirection.normalized * 0.6f; //Distance from player
                        if (!IsPlaceOccupied(targetPosition)) {
                            currentHeldItem.transform.SetParent(null);
                            currentHeldItem.transform.position = targetPosition;
                            currentHeldItem.OnDrop();
                            currentHeldItem = null;
                        }
                    } else if (closestCarryable != null && closestCarryable.IsInteractable()) {
                        currentHeldItem = closestCarryable;
                        currentHeldItem.transform.SetParent(itemHoldPosition);
                        currentHeldItem.transform.localPosition = Vector3.zero;
                        currentHeldItem.OnPickup();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    RequestedItems requests = gameObject.GetComponent<RequestedItems>();
                    foreach (Requests request in requests.requests) {
                        if (request != null) {
                            if (Mathf.Round(transform.position.x) == request.coordinates.x && Mathf.Round(transform.position.y) == request.coordinates.y) {
                                gameObject.GetComponent<PlayerInventory>().Pickup(request.item.item_GO);
                                Destroy(request.giver);
                                requests.requests.Remove(request);
                                break;
                            }
                        }
                    }
                }

                if (Globals.DialogueManager.displayDialogueUI) {
                    state = PlayerState.InDialogue;
                }

                break;
            case PlayerState.Interacting:
                if (Input.GetKeyDown(KeyCode.Space)) {
                    state = PlayerState.InDialogue;
                    if (interactableObject.GetComponent<DialogueTrigger>() != null) {
                        interactableObject.GetComponent<DialogueTrigger>().TriggerDialogue();
                    }

                    if (interactableObject.GetComponent<TriggerExercise>() != null) {
                        interactableObject.GetComponent<TriggerExercise>().OnTrigger();
                    }

                    if (interactableObject.GetComponent<Pickup>() != null) {
                        gameObject.GetComponent<PlayerInventory>().Pickup(interactableObject);
                    }

                    if (interactableObject.GetComponent<ItemRequest>() != null) {
                        interactableObject.GetComponent<ItemRequest>().OnInteraction(gameObject.GetComponent<PlayerInventory>());
                    }

                    if (interactableObject.GetComponent<SetRequest>() != null) {
                        var a = interactableObject.GetComponent<SetRequest>().Set();
                        gameObject.GetComponent<RequestedItems>().requests.Add(a);
                    }

                    if (interactableObject.GetComponent<SetRequestUI>() != null) {
                        interactableObject.GetComponent<SetRequestUI>().SetUI();
                    }

                    if (interactableObject.GetComponent<ShowFeedback>() != null) {
                        interactableObject.GetComponent<ShowFeedback>().Feedback();
                    }

                    //dialogueState += 1;
                }
                break;
            case PlayerState.InDialogue:
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Globals.DialogueManager.DisplayNextSentence();
                }
                //After the conversation has ended the dialogueState will be reset in the EndDialogue function of DialogueManager
                break;
        }
        nearbyCarriables.Clear();
    }

    private Carryable GetClosestCarryable() {
        if (currentHeldItem != null) return null;
        nearbyCarriables.Clear();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, carriableSearchRadius);

        foreach (Collider2D collider in hitColliders) {
            Carryable carryableComponent = collider.GetComponent<Carryable>();
            if (carryableComponent != null) {
                nearbyCarriables.Add(carryableComponent);
            }
        }

        if (nearbyCarriables.Count == 0) return null;
        Carryable closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = Globals.PlayerController.transform.position;

        foreach (Carryable obj in nearbyCarriables) {
            Vector3 directionToTarget = obj.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                closest = obj;
            }
        }

        return closest;
    }

    private bool IsPlaceOccupied(Vector3 position) {
        Collider2D[] hitColliders = null;
        if (currentHeldItem.GetObjectCollider() is CircleCollider2D circleCollider) {
            hitColliders = Physics2D.OverlapCircleAll(position, circleCollider.radius);
        } else if (currentHeldItem.GetObjectCollider() is BoxCollider2D boxCollider) {
            hitColliders = Physics2D.OverlapBoxAll(position, boxCollider.bounds.size, 0);
        } else Debug.LogError("Unsupported collider used!");
        if (hitColliders != null) {
            foreach (var collider in hitColliders) {
                if (collider.gameObject != gameObject && collider.gameObject != currentHeldItem.gameObject) {
                    return true;
                }
            }
        }
        return false;
    }

    //Very dirty way to get player looking rotation from animation
    private void UpdateLookDirection() {
        var animatorinfo = anim.GetCurrentAnimatorClipInfo(0);
        if (animatorinfo.Length == 0) return;
        var animation = animatorinfo[0].clip.name;
        bool up = animation.Contains("up");
        bool down = animation.Contains("down") || animation.Equals("boy_neutral");
        bool left = animation.Contains("left");
        bool right = animation.Contains("right");

        lookDirection = new Vector3(right ? 1 : left ? -1 : 0, up ? 1 : down ? -1 : 0, 0);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (state == PlayerState.Idle) {
            if (other.gameObject.CompareTag("Interactable") || other.gameObject.CompareTag("NPC")) {
                interactableObject = other.gameObject;
                state = PlayerState.Interacting;
            }
        }
    }

    public bool IsHoldingItem() {
        return currentHeldItem != null;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (state != PlayerState.Falling && other.gameObject.CompareTag("Interactable") && Globals.DialogueManager.sentences.Count == 0 || other.gameObject.CompareTag("NPC") && Globals.DialogueManager.sentences.Count == 0) {
            state = PlayerState.Idle;
        }
    }

    public void FallAndTeleport(Vector2 position, OnFinishedFalling onFinishedFalling) {
        this.onFinishedFalling = onFinishedFalling;
        StartCoroutine(StartPlayerFall(position));
    }

    IEnumerator StartPlayerFall(Vector2 position) {
        GameObject hole = Instantiate(Globals.HolePrefab, transform.position, transform.rotation);
        state = PlayerState.Falling;
        GetComponent<Collider2D>().enabled = false;
        anim.SetBool("Falling", true);
        anim.SetTrigger("Fall");
        yield return new WaitForSeconds(0.01f);
        Destroy(hole, 0.75f);
        StartCoroutine(EndPlayerFall(position));
    }

    IEnumerator EndPlayerFall(Vector2 position) {
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("Falling", false);
        anim.ResetTrigger("Fall");
        GetComponent<Collider2D>().enabled = true;
        state = PlayerState.Idle;
        if (onFinishedFalling != null) onFinishedFalling();
        transform.position = position;
    }
}
