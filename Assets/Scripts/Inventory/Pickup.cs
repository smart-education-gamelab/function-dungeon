using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void Awake()
    {
        if (!gameObject.GetComponent<NoExercise>())
        {
            gameObject.AddComponent<NoExercise>();
        }
    }

    private void Start()
    {
        if (item == null)
        {
            Destroy(this);
        }
    }

    public Items item;
}
