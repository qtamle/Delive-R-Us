using UnityEngine;

[CreateAssetMenu(menuName = "Delivery/Current Order")]
public class CurrentOrderData : ScriptableObject
{
    [Header("Read-Only")]
    public OrderData orderData;
}


