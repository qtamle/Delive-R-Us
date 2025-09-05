using System;
using UnityEngine;

public class SuperMarketTutorial : MonoBehaviour
{
    public static event Action EnterSuperMarket;
    public GameObject areaBlue;
    private void Start()
    {
        ScooterManager.Instance.SetScooterState(false);

        if (PlayerPrefs.GetInt("EnterSuperMarket", 0) == 0)
        {
            //EnterSuperMarket?.Invoke();
            PlayerPrefs.SetInt("EnterSuperMarket", 1);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetInt("Pickup") == 1)
        {
            areaBlue.SetActive(false);
        }
    }

}
