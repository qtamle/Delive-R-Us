using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Scriptable Objects/MissionData")]
public class MissionData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;

    public GameObject DeliveryPrefab;

}
