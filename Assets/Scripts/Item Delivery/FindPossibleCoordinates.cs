using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPossibleCoordinates : MonoBehaviour
{
    [Header("Spawn System")]
    [SerializeField] private Tilemap tileMap;
    [HideInInspector] public List<Vector3> availablePlaces;

    public Vector2 GetCoordinate()
    {
        FindLocationsOfTiles();
        int randomIndex = Random.Range(0, availablePlaces.Count);
        return new Vector2(availablePlaces[randomIndex].x, availablePlaces[randomIndex].y);
    }

    private void FindLocationsOfTiles()
    {
        availablePlaces = new List<Vector3>(); // create a new list of vectors by doing...

        for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++) // scan from left to right for tiles

        {
            for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++) // scan from bottom to top for tiles
            {
                Vector3Int localPlace = new Vector3Int(n, p, (int)tileMap.transform.position.y); // if you find a tile, record its position on the tile map grid
                Vector3 place = tileMap.CellToWorld(localPlace); // convert this tile map grid coords to local space coords
                if (tileMap.HasTile(localPlace))
                {
                    //Tile at "place"
                    availablePlaces.Add(place);
                }
            }
        }
    }

    //private void Test()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Debug.Log(GetCoordinate(availableRooms));
    //    }
    //}

    //private void Update()
    //{
    //    Test();
    //}
}
