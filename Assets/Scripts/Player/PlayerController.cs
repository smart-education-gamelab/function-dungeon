using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class PlayerController : Movement {
    [Header("Game Managers")]
    private Animator anim;

    [HideInInspector] public bool allowHorizontalInput;
    [HideInInspector] public bool allowVerticalInput;
    [HideInInspector] public bool allowSpaceInput;
    [HideInInspector] public bool falling;

    public Camera cam;
    public float carriableSearchRadius = 0.5f;

    public Carryable currentHeldItem; // The current item above the player's head.
    public Transform itemHoldPosition; // The position where the item will be displayed above the player's head.

    private GameObject interactableObject;
    private Carryable closestCarryable;
    [HideInInspector] public int dialogueState;

    private List<Carryable> nearbyCarriables = new List<Carryable>();
    private Vector3 lookDirection;
    private bool isCameraDetached = false;
    private Tween cameraTween1, cameraTween2;

    public virtual void Start() {
        anim = gameObject.GetComponent<Animator>();

        dialogueState = -1;

        allowVerticalInput = true;
        allowHorizontalInput = true;
        allowSpaceInput = true;
    }

    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveInTargetLocalSpace(Transform transform, Transform target, Vector3 targetLocalEndPosition, float duration) {
        var t = DOTween.To(
            () => transform.position - target.transform.position, // Value getter
            x => transform.position = x + target.transform.position, // Value setter
            targetLocalEndPosition,
            duration);
        t.SetTarget(transform);
        return t;
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

        if (falling || Globals.DialogueManager.displayDialogueUI || Globals.MathManager.displayExerciseUI) {
            allowHorizontalInput = false;
            allowVerticalInput = false;
        } else {
            allowHorizontalInput = true;
            allowVerticalInput = true;
        }

        if (allowHorizontalInput) {
            directionX = Input.GetAxisRaw("Horizontal");
        } else {
            directionX = 0; //Makes it so the player stops walking if input is turned off.
        }

        if (allowVerticalInput) {
            directionY = Input.GetAxisRaw("Vertical");
        } else {
            directionY = 0; //Makes it so the player stops walking if input is turned off.
        }

        if (isCameraDetached && (directionX != 0 || directionY != 0)) {
            isCameraDetached = false;
            cameraTween1?.Kill();
            cameraTween2?.Kill();
            DOMoveInTargetLocalSpace(cam.transform, Globals.PlayerController.transform, new Vector3(0, 0, -10), 0.5f).OnComplete(() => {
                cam.transform.SetParent(Globals.PlayerController.transform);

            }).SetEase(Ease.OutCubic);
            cameraTween2 = cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        }

       
        if (!falling) {
            switch (dialogueState) {
                case -1:
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
                        dialogueState = 1;
                    }

                    break;
                case 0:
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        dialogueState += 1;
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
                case 1:
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        Globals.DialogueManager.DisplayNextSentence();
                    }
                    //After the conversation has ended the dialogueState will be reset in the EndDialogue function of DialogueManager
                    break;
            }
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
        if (dialogueState == -1) {
            if (other.gameObject.CompareTag("Interactable") || other.gameObject.CompareTag("NPC")) {
                interactableObject = other.gameObject;
                dialogueState = 0;
            }
        }
    }

    public bool IsHoldingItem() {
        return currentHeldItem != null;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Interactable") && Globals.DialogueManager.sentences.Count == 0 || other.gameObject.CompareTag("NPC") && Globals.DialogueManager.sentences.Count == 0) {
            dialogueState = -1;
        }
    }

    public void FallAndTeleport(Vector2 position) {
        StartCoroutine(StartPlayerFall(position));
    }

    IEnumerator StartPlayerFall(Vector2 position) {
        GameObject hole = Instantiate(Globals.HolePrefab, transform.position, transform.rotation);
        falling = true;
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
        falling = false;
        transform.position = position;
    }

    public void FocusCamera(Transform cameraFocus, float cameraFocusSize) {
        cam.transform.SetParent(null);
        cameraTween1?.Kill();
        cameraTween2?.Kill();
        cameraTween1 = cam.transform.DOMove(new Vector3(cameraFocus.position.x, cameraFocus.position.y, -10f), 0.5f).OnComplete(() => {
            isCameraDetached = true;
        }).SetEase(Ease.OutCubic);
        cameraTween2 = cam.DOOrthoSize(cameraFocusSize, 0.5f).SetEase(Ease.OutCubic);
    }
}
