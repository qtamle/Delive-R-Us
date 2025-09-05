using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public GameObject IPrefab;

    public ShopItem _iData;
}


[System.Serializable]
public class ShopItem
{
    public string ItemName;
    [Multiline] public string ItemDescription;
    public Sprite ItemIcon;
    public float ItemPrice = 0;

    [Space]
    public bool isBought = false;
}