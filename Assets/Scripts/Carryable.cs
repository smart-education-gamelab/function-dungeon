using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CarryableType {
    MovableStone,
    UnmovableStone,
    Ball,
}

[RequireComponent(typeof(Rigidbody2D))]
public class Carryable : MonoBehaviour {
    public Collider2D objectCollider;
    public CarryableType type;

    private SpriteRenderer spriteRenderer;
    private Color interactableColor = Color.yellow;
    public float maxSnapDistance = 1.25f;

    private Color originalColor;
    private Rigidbody2D rb;
    [NonSerialized] public Transform parent; //Stores the parent of the object while its being picked up by the player

    private bool isInteractable = false;
    private GameObject interactionIcon;
    private static Vector3 interactableIconScale = new Vector3(0.25f, 0.25f, 0f);

    public virtual void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;

        interactionIcon = new GameObject("InteractionIcon");
        SpriteRenderer interactionSpriteRenderer = interactionIcon.AddComponent<SpriteRenderer>();
        interactionSpriteRenderer.sprite = Globals.InteractionIcon;
        interactionSpriteRenderer.sortingOrder = 3;
        interactionSpriteRenderer.sortingLayerName = "Foreground";
        interactionIcon.transform.SetParent(transform);
        interactionIcon.transform.localPosition = new Vector3(0f, 0.25f, 0f); //Hardcoded offset for current hand icon
        interactionIcon.transform.localScale = interactableIconScale;
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
        interactionIcon.SetActive(false);
        objectCollider.gameObject.SetActive(false);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public virtual void OnDrop() {
        objectCollider.gameObject.SetActive(true);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public bool TrySnapToPuzzleFloor(Tilemap tileMap) {
        Vector3Int centerCellPosition = tileMap.WorldToCell(transform.position);

        Vector3 snapPosition = Vector3.zero;
        float closestDistance = float.MaxValue;

        // Loop through a 3x3 area around the object.
        for (int xOffset = -1; xOffset <= 1; xOffset++) {
            for (int yOffset = -1; yOffset <= 1; yOffset++) {
                Vector3Int currentCellPosition = centerCellPosition + new Vector3Int(xOffset, yOffset, 0);
                TileBase tile = tileMap.GetTile(currentCellPosition);

                if (tile != null && tile.name.Contains("Puzzlefloor")) {
                    Vector3 centerPosition = tileMap.GetCellCenterWorld(currentCellPosition);
                    float distance = Vector3.Distance(transform.position, centerPosition);

                    if (distance < maxSnapDistance && distance < closestDistance) {
                        closestDistance = distance;
                        snapPosition = centerPosition + new Vector3(0f, 0.25f, 0f);
                    }
                }
            }
        }

        // Check if a valid snapping position was found.
        if (snapPosition != Vector3.zero) {
            transform.position = snapPosition;
            return true;
        }
        return false;
    }
}