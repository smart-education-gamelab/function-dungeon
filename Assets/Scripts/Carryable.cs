using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarryableType {
    MovableStone,
    UnmovableStone,
    Ball,
}

[RequireComponent(typeof(Rigidbody2D))]
public class Carryable : MonoBehaviour {
    public Collider2D objectCollider;
    private SpriteRenderer spriteRenderer;
    private Color interactableColor = Color.yellow;
    public CarryableType type;
    private Color originalColor;
    private Rigidbody2D rb;

    private bool isInteractable = false;
    private GameObject interactionIcon;

    public virtual void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;

        interactionIcon = new GameObject("InteractionIcon");
        SpriteRenderer interactionSpriteRenderer = interactionIcon.AddComponent<SpriteRenderer>();
        interactionSpriteRenderer.sprite = Globals.HandIcon;
        interactionSpriteRenderer.sortingOrder = 3;
        interactionSpriteRenderer.sortingLayerName = "Foreground";
        interactionIcon.transform.SetParent(transform);
        interactionIcon.transform.localPosition = new Vector3(-0.01f, 0.17f, 0f); //Hardcoded offset for current hand icon
        interactionIcon.transform.localScale = new Vector3(0.1f, 0.1f, 0f);
        interactionIcon.SetActive(false);
        InitializePhysicsVariables();
    }

    public Collider2D GetObjectCollider() {
        return objectCollider;
    }

    private void InitializePhysicsVariables() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.angularDrag = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        switch (type) {
            case CarryableType.MovableStone: {
                    rb.mass = 2;
                    rb.drag = 100;
                }
                break;
            case CarryableType.UnmovableStone: {
                    rb.mass = 10000;
                    rb.drag = 10000;
                }
                break;
            case CarryableType.Ball: {
                    rb.mass = 2;
                    rb.drag = 5;
                }
                break;
            default:
                break;

        }
    }

    public void SetInteractable(bool state) {
        isInteractable = state;
        if (state) {
            spriteRenderer.color = interactableColor;
            interactionIcon.SetActive(true);
        } else {
            spriteRenderer.color = originalColor;
            interactionIcon.SetActive(false);
        }
    }

    public virtual bool IsInteractable() {
        return isInteractable;
    }

    //Only needed if object can rotate
    //public void Update() {
    //interactionIcon.transform.localRotation = Quaternion.Inverse(transform.localRotation);
    //interactionIcon.transform.position = transform.position + new Vector3(-0.01f, 0.17f, 0f);
    //}

    public virtual void OnPickup() {
        objectCollider.gameObject.SetActive(false);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public virtual void OnDrop() {
        objectCollider.gameObject.SetActive(true);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}