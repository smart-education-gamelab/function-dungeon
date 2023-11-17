using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class CustomPuzzle : MonoBehaviour {
    [HideInInspector] public Question question;
    public Transform playerSpawnPoint;

    private Vector2 originalPlayerLocation;

    public virtual void Awake() {
        originalPlayerLocation = Globals.PlayerController.transform.position;
        transform.position = new Vector3(-100, -100, 0);
    }

    //Called when the player exits the puzzle
    public virtual void OnLadderExit() {
        Globals.MathManager.OnQuestionComplete();
        Globals.PlayerController.transform.position = originalPlayerLocation;
        Globals.MathManager.customPuzzle = null;
        Globals.CameraController.ReattachCamera(0);
        Destroy(gameObject);
    }

    public virtual void OnPlayerEnter() {
    }
}
