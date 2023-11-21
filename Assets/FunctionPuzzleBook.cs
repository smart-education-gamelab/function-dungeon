using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionPuzzleBook : MonoBehaviour {
    public FunctionPuzzle functionPuzzle;
    public Transform cameraFocus;
    public float cameraFocusSize = 3.5f;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Globals.PlayerController.FocusCamera(cameraFocus, cameraFocusSize);
            if(functionPuzzle.answeredCorrectly) Utils.DelayedAction(1.0f, () => functionPuzzle.ladder.gameObject.SetActive(true));
        }
    }
}
