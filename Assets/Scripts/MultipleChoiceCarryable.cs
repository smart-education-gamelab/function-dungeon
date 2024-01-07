using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultipleChoiceCarryable : Carryable {
    public MultipleChoicePuzzle customPuzzle;
    public override void OnDrop() {
        base.OnDrop();
        TrySnapToPuzzleFloor(customPuzzle.layoutTilemap);
    }
}
