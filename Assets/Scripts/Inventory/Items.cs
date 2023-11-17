using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Items : ScriptableObject
{
    public string item_name;
    public Sprite item_sprite;
    public GameObject item_GO;

    public GameObject original_object;
}
