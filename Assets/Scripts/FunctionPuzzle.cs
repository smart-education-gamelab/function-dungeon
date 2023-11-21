using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FunctionPuzzle : CustomPuzzle {
    [HideInInspector] public bool answeredCorrectly = false;
    public LineRenderer laser;
    public ParticleSystem laserBeamEnd;
    public PuzzleLadder ladder;
    public Transform answerPosition;
    public Tilemap layoutTilemap;
    public List<FunctionPuzzleCarryable> functionPuzzleCarryables = new List<FunctionPuzzleCarryable>();
    public FunctionPuzzleCarryable correctAnswer;

    public void UpdateLaser() {
        if (answeredCorrectly) return;

        FunctionPuzzleCarryable closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        //Find closest
        foreach (FunctionPuzzleCarryable obj in functionPuzzleCarryables) {
            Vector3 directionToTarget = obj.transform.position - answerPosition.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                closest = obj;
            }
        }

        if (closestDistanceSqr < 0.5f) {
            laser.gameObject.SetActive(true);
            Vector3 laserEndPos = new Vector3(10f, 10f * closest.number, 0f);
            laser.SetPosition(1, laserEndPos);
            laserBeamEnd.gameObject.transform.position = laser.transform.position + laserEndPos;
            if (closest == correctAnswer) answeredCorrectly = true;
        } else {
            laser.gameObject.SetActive(false);
        }
    }
}
