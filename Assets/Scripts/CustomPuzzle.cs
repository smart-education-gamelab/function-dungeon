using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomPuzzle : MonoBehaviour {
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
        Destroy(gameObject);
    }
}
