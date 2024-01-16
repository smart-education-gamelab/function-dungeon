using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolvePuzzleCarryable : Carryable {
    private SolvePuzzle customPuzzle;
    public int answerIndex;

    [HideInInspector]
    public bool snappedToPuzzleFloor = false;

    public override void Start() {
        base.Start();
        customPuzzle = FindObjectOfType<SolvePuzzle>();
    }

    public override void OnDrop() {
        base.OnDrop();
        snappedToPuzzleFloor = TrySnapToPuzzleFloor(customPuzzle.layoutTilemap);
        customPuzzle.OnCarryablePlacement();
    }
}
