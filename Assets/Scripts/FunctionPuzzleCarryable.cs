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
        Vector3Int cellPosition = customPuzzle.layoutTilemap.WorldToCell(transform.position);
        TileBase tile = customPuzzle.layoutTilemap.GetTile(cellPosition);
        if (tile.name.Contains("Puzzlefloor")) {
            Vector3 centerPosition = customPuzzle.layoutTilemap.GetCellCenterWorld(cellPosition);
            transform.position = centerPosition + new Vector3(0f, 0.25f, 0f);
        }
        customPuzzle.UpdateLaser();
    }
}
