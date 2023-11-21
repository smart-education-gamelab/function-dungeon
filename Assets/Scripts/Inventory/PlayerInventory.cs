using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : Inventory
{
    [Header("UI")]
    private Items[] items_;
    [SerializeField] private Inventory_UI[] UI;

    private void Start()
    {
        items_ = new Items[items.Length];
    }

    private void Update()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                UI[i].slots.SetActive(true);
                UI[i].icon.sprite = items[i].item_sprite;
            }
            else
            {
                UI[i].slots.SetActive(false);
            }
        }
    }

    public void Pickup(GameObject pickup)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = pickup.GetComponent<Pickup>().item;

                items_[i] = Instantiate(items[i]);          //A copy of the SO is made so changes like changing the original object can happen without changing the original SO.
                items_[i].original_object = pickup;
                items_[i].original_object.SetActive(false);
                break;
            }
        }
    }

    public void Remove(int index)
    {
        items_[index].original_object.SetActive(true);      //When the object is removed from Inventory it appears in its original location again.
        items[index] = null;
    }
}

[System.Serializable]
public class Inventory_UI
{
    [Header("Inventory")]
    public GameObject slots; //The slots and item icons of the UI
    public Image icon;
}
