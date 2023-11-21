using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class DoorPair {
    public int levelToOpen;
    public TileDefinition doorOne, doorTwo;

    public DoorPair(int levelToOpen, TileDefinition doorOne, TileDefinition doorTwo) {
        this.levelToOpen = levelToOpen;
        this.doorOne = doorOne;
        this.doorTwo = doorTwo;
    }

    public Vector2 GetPosition() {
        return new Vector2((doorOne.x + doorTwo.x + 1.0f) / 2, (doorOne.y + doorTwo.y + 1.0f) / 2);
    }
}

[Serializable]
public class RoomData {
    public int level;
    public string learningGoalSection;
    public GameObject roomObj;
    public TileDefinition roomTile;
    public List<TileDefinition> doorTiles;
    public List<DoorPair> doorPairs;

    public RoomData(int level, string learningGoalSection, GameObject roomObj, TileDefinition roomTile, List<TileDefinition> doorTiles, List<DoorPair> doorPairs) {
        this.level = level;
        this.learningGoalSection = learningGoalSection;
        this.roomObj = roomObj;
        this.roomTile = roomTile;
        this.doorTiles = doorTiles;
        this.doorPairs = doorPairs;
    }
}

public class RoomsManager : MonoBehaviour {
    public GameObject roomPrefab;
    public GameObject objectPrefab;
    public GameObject level;
    public GameObject ladder;
    public GameObject player;
    public GameObject npc1, npc2;
    public GameObject vase1, vase2;
    public GameObject[] decorativeObjects;

    [SerializeField, HideInInspector] private Vector3 npcOneRequestLocation, npcTwoRequestLocation;
    [SerializeField, HideInInspector] private int roomCount = 0;
    [SerializeField, HideInInspector] private List<DoorPair> doorPairs = new List<DoorPair>();
    [SerializeField, HideInInspector] private List<RoomData> rooms = new List<RoomData>();

    Dictionary<int, TileDefinition> roomsDict = new Dictionary<int, TileDefinition>();
    Dictionary<int, GameObject> roomGameObjectsDict = new Dictionary<int, GameObject>();
    Dictionary<int, List<TileDefinition>> localDoorsDict = new Dictionary<int, List<TileDefinition>>();
    Dictionary<int, List<GameObject>> roomObjectsDict = new Dictionary<int, List<GameObject>>();

    public void Reset(LevelGenerator levelGenerator) {
        npc1.transform.position = Vector3.zero;
        npc2.transform.position = Vector3.zero;
        vase1.transform.position = Vector3.zero;
        vase2.transform.position = Vector3.zero;
        npcOneRequestLocation = Vector3.zero;
        npcTwoRequestLocation = Vector3.zero;
        roomCount = 0;
        roomsDict = new Dictionary<int, TileDefinition>();
        roomGameObjectsDict = new Dictionary<int, GameObject>();
        localDoorsDict = new Dictionary<int, List<TileDefinition>>();
        roomObjectsDict = new Dictionary<int, List<GameObject>>();
        rooms = new List<RoomData>();
        doorPairs = new List<DoorPair>();

        GameObject[] roomsToDestroy = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in roomsToDestroy)
            DestroyImmediate(room);
    }

    public void IncrementRoomCount(LevelGenerator levelGenerator) {
        roomCount++;
    }

    public void RoomGrammarCallback(LevelGenerator levelGenerator, GrammarPlacedEvent e) {
        levelGenerator.GetTile(e.tile.x, e.tile.y).extraData.roomIndex = roomCount;
        if (levelGenerator.GetTile(e.tile.x, e.tile.y).character == 'i') {
        }
        if (levelGenerator.GetTile(e.tile.x, e.tile.y).character == 'd') {
            levelGenerator.GetTile(e.tile.x, e.tile.y).extraData.roomIndex = roomCount - 1;
        }
    }

    public void MakeRoomConnectionsCallback(LevelGenerator levelGenerator) {
        for (int x = 0; x < levelGenerator.lastGenerationWidth; x++) {
            for (int y = 0; y < levelGenerator.lastGenerationHeight; y++) {
                var tile = levelGenerator.GetTile(x, y);
                int roomIndex = tile.extraData.roomIndex;
                if (roomIndex != -1) {
                    if (tile.character == 'i' || tile.character == 'f') {
                        roomsDict.Add(roomIndex, tile);
                    }
                }
            }
        }
    }
    public void CollectDoorsCallback(LevelGenerator levelGenerator) {
        for (int x = 0; x < levelGenerator.lastGenerationWidth; x++) {
            for (int y = 0; y < levelGenerator.lastGenerationHeight; y++) {
                var tile = levelGenerator.GetTile(x, y);
                int roomIndex = tile.extraData.roomIndex;
                if (tile.character == 'd') {
                    if (localDoorsDict.TryGetValue(roomIndex, out List<TileDefinition> tileList)) {
                        tileList.Add(tile);
                    } else {
                        List<TileDefinition> newTileList = new List<TileDefinition> { tile };
                        localDoorsDict.Add(roomIndex, newTileList);
                    }
                }
            }
        }
    }

    public void PlaceNPC1(LevelGenerator levelGenerator, GrammarPlacedEvent e) {
        if (e.newCharacter == 'r') {
            e.newCharacter = 'b';
            npc1.transform.position = new Vector3(e.tile.x + 0.5f, e.tile.y + 0.5f, 0);
        }
        if (e.newCharacter == 't') {
            e.newCharacter = 'b';
            vase1.transform.position = new Vector3(e.tile.x + 0.5f, e.tile.y + 0.5f, 0);
        }
    }

    public void PlaceNPC2(LevelGenerator levelGenerator, GrammarPlacedEvent e) {
        if (e.newCharacter == 'r') {
            e.newCharacter = 'b';
            npc2.transform.position = new Vector3(e.tile.x + 0.5f, e.tile.y + 0.5f, 0);
        }
        if (e.newCharacter == 't') {
            e.newCharacter = 'b';
            vase2.transform.position = new Vector3(e.tile.x + 0.5f, e.tile.y + 0.5f, 0);
        }
    }

    public void FindNPCOneItemTileCustomGrammar(LevelGenerator levelGenerator, GrammarPlacedEvent e) {
        if (e.newCharacter == 'b') {
            levelGenerator.SetNPCOneRequestTile(e.tile.x, e.tile.y);
            npcOneRequestLocation = new Vector3(e.tile.x, e.tile.y, 0);
            e.newCharacter = 'e';
        }
    }

    public void FindNPCTwoItemTileCustomGrammar(LevelGenerator levelGenerator, GrammarPlacedEvent e) {
        if (e.newCharacter == 'b') {
            levelGenerator.SetNPCTwoRequestTile(e.tile.x, e.tile.y);
            npcTwoRequestLocation = new Vector3(e.tile.x, e.tile.y, 0);
            e.newCharacter = 'e';
        }
    }

    public void PlaceRoomLayout(LevelGenerator levelGenerator, GrammarPlacedEvent e) {
        try {
            int index = int.Parse(e.newCharacter.ToString());
            GameObject obj = Instantiate(decorativeObjects[index], new Vector3(e.tile.x + 0.5f, e.tile.y + 0.5f, 0), Quaternion.identity);

            if (roomObjectsDict.TryGetValue(e.tile.extraData.roomIndex, out List<GameObject> objectsList)) {
                objectsList.Add(obj);
            } else {
                List<GameObject> newObjectsList = new List<GameObject> { obj };
                roomObjectsDict.Add(e.tile.extraData.roomIndex, newObjectsList);
            }
            e.newCharacter = 'e';
        } catch {}
    }

    public void SetLadder(LevelGenerator levelGenerator) {
        ladder.transform.position = new Vector3(roomsDict[roomCount].x + 0.5f, roomsDict[roomCount].y + 0.5f);
    }

    public void GeneratePlayableRooms(LevelGenerator levelGenerator) {
        for (int i = 0; i < roomCount; i++) {
            GameObject roomObj = Instantiate(roomPrefab, level.transform);
            string learningGoalSection = GetLearningGoalSectionForRoom(levelGenerator, i, roomCount);
            roomObj.GetComponent<SetExercise>().section = learningGoalSection;
            //GameObject obj = Instantiate(objectPrefab, new Vector3(roomsDict[i].x + 0.5f, roomsDict[i].y + 0.5f), Quaternion.identity);
            //obj.transform.parent = roomObj.transform;

            if (roomObjectsDict.ContainsKey(i)) {
                foreach (GameObject roomObject in roomObjectsDict[i]) {
                    roomObject.transform.parent = roomObj.transform;
                }
            }

            List<DoorPair> localDoorPairs = new List<DoorPair>();
            //if (localDoorsDict.ContainsKey(i)) {
                foreach (var localDoor in localDoorsDict[i]) {
                    int x = localDoor.x;
                    int y = localDoor.y;
                    TileDefinition left = levelGenerator.GetTile(x - 2, y);
                    TileDefinition right = levelGenerator.GetTile(x + 2, y);
                    TileDefinition down = levelGenerator.GetTile(x, y - 2);
                    TileDefinition up = levelGenerator.GetTile(x, y + 2);

                    if (left != null && left.character == 'd') localDoorPairs.Add(new DoorPair(Mathf.Max(localDoor.extraData.roomIndex, left.extraData.roomIndex), localDoor, left));
                    if (right != null && right.character == 'd') localDoorPairs.Add(new DoorPair(Mathf.Max(localDoor.extraData.roomIndex, right.extraData.roomIndex), localDoor, right));
                    if (up != null && up.character == 'd') localDoorPairs.Add(new DoorPair(Mathf.Max(localDoor.extraData.roomIndex, up.extraData.roomIndex), localDoor, up));
                    if (down != null && down.character == 'd') localDoorPairs.Add(new DoorPair(Mathf.Max(localDoor.extraData.roomIndex, down.extraData.roomIndex), localDoor, down));
                }
            //}
            doorPairs.AddRange(localDoorPairs);
            roomGameObjectsDict.Add(i, roomObj);

            RoomData rData = new RoomData(i, learningGoalSection, roomObj, roomsDict[i], localDoorsDict[i], localDoorPairs);
            rooms.Add(rData);
        }

        for (int j = 0; j < roomCount; j++) {
            SetDoors setDoors = roomGameObjectsDict[j].GetComponent<SetDoors>();
            IEnumerable<DoorPair> pairs = doorPairs.Where(x => x.levelToOpen == j);

            foreach (DoorPair pair in pairs) {
                setDoors.doors.Add(new Doors(new Vector2(pair.doorOne.x, pair.doorOne.y)));
                setDoors.doors.Add(new Doors(new Vector2(pair.doorTwo.x, pair.doorTwo.y)));
            }
        }
        player.transform.position = new Vector2(levelGenerator.lastGenerationWidth / 2 + 0.5f, levelGenerator.lastGenerationHeight / 2 + 0.5f);
    }

    //This function determines what learning goal is selected for room i when there are roomCount rooms.
    private string GetLearningGoalSectionForRoom(LevelGenerator levelGenerator, int i, int roomCount) {
        float stepSize = (levelGenerator.maxLearningGoalLevel - levelGenerator.minLearningGoalLevel) / (float)(roomCount - 1);
        int index = levelGenerator.minLearningGoalLevel + Mathf.RoundToInt(i * stepSize);
        return levelGenerator.learningGoalLevels[index];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        int index = 0;
        GUIStyle style = GUI.skin.textArea;
        style.alignment = TextAnchor.MiddleCenter;

        foreach (var room in rooms) {
            foreach (var doorPair in room.doorPairs) {
                Vector2 doorPosition = doorPair.GetPosition();
                DrawLine(new Vector2(room.roomTile.x + 0.5f, room.roomTile.y + 0.5f), doorPosition, index);
                Handles.Label(new Vector3(doorPosition.x, doorPosition.y), $"{room.level}", style);
            }
            Handles.Label(new Vector3(room.roomTile.x + 0.5f, room.roomTile.y + 0.5f), $"Room {room.level}\n{room.learningGoalSection}", style);
            index++;
        }

        if (npcOneRequestLocation != Vector3.zero) {
            Handles.Label(new Vector3(npcOneRequestLocation.x + 0.5f, npcOneRequestLocation.y + 0.5f), $"NPC 1\nRequest Location", style);
        }

        if (npcTwoRequestLocation != Vector3.zero) {
            Handles.Label(new Vector3(npcTwoRequestLocation.x + 0.5f, npcTwoRequestLocation.y + 0.5f), $"NPC 2\nRequest Location", style);
        }

        style.alignment = TextAnchor.UpperLeft;
        Handles.color = Color.white;
    }

    private void DrawLine(Vector2 pos1, Vector2 pos2, int index) {
        const float fixedLength = 0.5f;
        Color startColor = Color.red;
        Color endColor = Color.green;

        Vector3 position1 = new Vector3(pos1.x, pos1.y);
        Vector3 position2 = new Vector3(pos2.x, pos2.y);

        Vector3 direction = (position2 - position1).normalized;
        Vector3 start = position1 + direction * fixedLength;
        Vector3 end = position2 - direction * fixedLength;

        float t = (float)index / (roomCount - 1);
        Color lerpedColor = Color.Lerp(startColor, endColor, t);
        Color.RGBToHSV(lerpedColor, out float h, out float _, out float _);
        Color visibleColor = Color.HSVToRGB(h, 1, 1);
        Handles.color = visibleColor;
        Handles.DrawLine(start, end);
    }
#endif
}
