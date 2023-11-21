using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetDoors : MonoBehaviour {
    [SerializeField] public List<Doors> doors = new List<Doors>();
    private Vector3Int location;

    [Header("Replacement tile for open door:")]
    [SerializeField] private RuleTile newTileUp;
    [SerializeField] private RuleTile newTileLow;

    public void Awake() {
        Tilemap ceiling = gameObject.transform.parent.Find("Grid").Find("Ceiling").GetComponent<Tilemap>();
        Tilemap layout = gameObject.transform.parent.Find("Grid").Find("Layout").GetComponent<Tilemap>();
        foreach (Doors door in doors) {
            door.ceilingMap = ceiling;
            door.doorMap = layout;
        }
    }

    public void UnlockAllDoors() {
        for (int i = 0; i < doors.Count; i++) {
            location = doors[i].doorMap.WorldToCell(doors[i].doorPos);
            doors[i].doorMap.SetTile(location, newTileLow);
            doors[i].ceilingMap.SetTile(location, newTileUp);
            doors[i].doorMap.GetComponent<CompositeCollider2D>().GenerateGeometry();
        }
    }
}

[System.Serializable]
public class Doors {
    internal Tilemap doorMap;
    internal Tilemap ceilingMap;
    [Header("Door tile coordinates:")]
    public Vector2 doorPos;

    public Doors() {
        doorPos = Vector2.zero;
    }

    public Doors(Vector2 doorPos) {
        this.doorPos = doorPos;
    }
}
