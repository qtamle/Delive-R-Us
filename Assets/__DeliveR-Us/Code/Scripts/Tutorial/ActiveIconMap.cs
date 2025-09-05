using UnityEngine;

public class ActiveIconMap : MonoBehaviour
{
    private MarketPortalsController marketPortalsController;
    void Start()
    {
        marketPortalsController = FindFirstObjectByType<MarketPortalsController>();

        if (!PlayerPrefs.HasKey("HasShownOrderReceived"))
        {
            if (marketPortalsController == null)
            {
                Debug.Log("Errorrrrrrr");
            }
            else
            {
                marketPortalsController.DisableAllPortals();
                DonutShopEvents.Hide();
                GasStationEvents.Hide();
            }
        }

        if (GameManager.Instance.GetCurrentDay() >= 2)
        {
            marketPortalsController.EnableAllPortals();
            DonutShopEvents.Show();
            GasStationEvents.Show();
        }
    }

}
