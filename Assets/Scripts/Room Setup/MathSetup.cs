using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathSetup : MonoBehaviour {
    private GameObject[] rooms;
    private List<GameObject> objects = new List<GameObject>();
    public GameObject failRoom;

    public void Start() {
        Globals.MathManager.ResetQuestions();
        InitializeObjects();
    }

    public void InitializeObjects() {
        rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms) {
            for (int i = 0; i < room.transform.childCount; i++) {
                Transform child = room.transform.GetChild(i);
                if (child.gameObject.CompareTag("Interactable") && !child.gameObject.GetComponent<NoExercise>()) {
                    objects.Add(child.gameObject);  //Makes list of items that can potentially receive math exercise
                }
            }

            GameObject exerciseObject = null;

            if (objects.Count > 0) {
                int random = Random.Range(0, objects.Count);
                exerciseObject = objects[random];
            }
            if (exerciseObject != null) {
                exerciseObject.AddComponent<TriggerExercise>();
            }
            objects.Clear();
        }
    }
}


