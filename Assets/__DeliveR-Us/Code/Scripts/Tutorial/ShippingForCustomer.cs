using System;
using UnityEngine;

public class ShippingForCustomer : MonoBehaviour
{
    public static event Action OnShipping;
    private void Start()
    {
        if (PlayerPrefs.GetInt("EnterSuperMarket", 0) == 1 && PlayerPrefs.GetInt("Tutorial1", 1) == 1)
        {
            //OnShipping?.Invoke();
            OrderManager.Instance.GetTimer.Pause();
            PlayerPrefs.SetInt("EnterSuperMarket", 1);
            PlayerPrefs.Save();
        }
    }
}
