using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathSetup : MonoBehaviour
{
    public GameObject[] rooms;
    private List<GameObject> objects = new List<GameObject>();

    public void Awake()
    {
        rooms = GameObject.FindGameObjectsWithTag("Room");
       
        foreach (GameObject room in rooms)
        {
            foreach (Transform child in room.transform)
            {
                if (child.gameObject.CompareTag("Interactable") && !child.gameObject.GetComponent<NoExercise>())
                {
                    objects.Add(child.gameObject);  //Makes list of items that can potentially receive math exercise
                }
            }

            GameObject exerciseObject = null;
            if (objects.Count > 0)
            {
                int random = Random.Range(0, objects.Count);
                exerciseObject = objects[random];
            }
            if (exerciseObject != null)
            {
                exerciseObject.AddComponent<TriggerExercise>();
            }
            objects.Clear();
        }
    }
}


