using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement Stats")]
    [SerializeField] private float speedX;
    [SerializeField] private float speedY;

    [HideInInspector] public float directionX;
    [HideInInspector] public float directionY;
    private float movementX;
    private float movementY;
    private Vector2 moveVelocity;

    public virtual void Update()
    {
        movementX = directionX * speedX;
        movementY = directionY * speedY;

        moveVelocity = new Vector2(movementX, movementY);
        moveVelocity = Vector2.ClampMagnitude(moveVelocity, 10);
    }

    private void FixedUpdate()
    {
        rb.velocity = moveVelocity;
    }
}
