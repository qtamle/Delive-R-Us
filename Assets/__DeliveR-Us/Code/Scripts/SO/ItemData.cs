
using UnityEngine;

[CreateAssetMenu(menuName = "Delivery/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public float itemCost;


    public ItemData Clone()
    {
        ItemData clonedItem = new ItemData();

        clonedItem.itemName = itemName;
        clonedItem.itemCost = itemCost;
        clonedItem.itemIcon = itemIcon;

        return clonedItem;
    }
}