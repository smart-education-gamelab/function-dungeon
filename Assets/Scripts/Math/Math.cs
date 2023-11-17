using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Math", menuName = "Math")]
public class Math : ScriptableObject
{
    public string id;
    public string question;
    public Sprite image;

    public string correct;
    public string wrong1;
    public string wrong2;
    public string wrong3;

    public string section;
    public string variation;
    public string learningGoal;

    public string feedback;
    public Dialogue dialogue;

    public bool used;
}
