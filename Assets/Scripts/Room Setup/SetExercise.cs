using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetExercise : MonoBehaviour
{       
    [Header("Set Dialogue")]
    [SerializeField] internal Dialogue dialogue;
    [Header("Set Section")]
    [SerializeField] internal string section;
    internal Math exercise;

    private List<Animator> torches;

    private void Awake()
    {
       torches = new List<Animator>();
    }

    public List<Animator> Torches()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Torch"))
            {
                torches.Add(child.gameObject.GetComponent<Animator>());
            }
        }
        return torches;
    }
}
