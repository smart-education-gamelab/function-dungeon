using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FunctionPuzzleCarryable : Carryable {
    //The number this carryable represents in the function puzzle
    public float number;

    private FunctionPuzzle customPuzzle;

    public override void Start() {
        base.Start();
        customPuzzle = FindObjectOfType<FunctionPuzzle>();
    }

    public override void OnDrop() {
        base.OnDrop();
        TrySnapToPuzzleFloor(customPuzzle.layoutTilemap);
        customPuzzle.UpdateLaser();
    }
}
