using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SolvePuzzle : CustomPuzzle {
    public Tilemap layoutTilemap;
    public PuzzleLadder ladder;

    private List<SolvePuzzleCarryable> puzzleCarryables;

    public void Start() {
        puzzleCarryables = FindObjectsOfType<SolvePuzzleCarryable>().ToList();
        puzzleCarryables.OrderBy(x => x.answerIndex);
        puzzleCarryables.Reverse();
    }

    public void OnCarryablePlacement() {
        float xPos = float.MinValue;
        bool success = true;
        foreach (var carryable in puzzleCarryables) {
            if (!carryable.snappedToPuzzleFloor) success = false;
            if (xPos < carryable.transform.position.x) {
                xPos = carryable.transform.position.x;
            } else {
                success = false;
            }
        }

        if (success) {
            ladder.gameObject.SetActive(true);
        }
    }
}
