using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemCheck
{
    public static bool CheckItems(Inventory target, Items requestedItem)
    {
        for (int i = 0; i < target.items.Length; i++)
        {
            if (target.items[i] != null)
            {
                if (requestedItem.item_name == target.items[i].name)        //Checks if items in inventory align with target objects requested items
                {
                    target.items[i] = null;                        //Matching item is removed from inventory
                    return true;
                }
            }
        }
        return false;
    }
}
