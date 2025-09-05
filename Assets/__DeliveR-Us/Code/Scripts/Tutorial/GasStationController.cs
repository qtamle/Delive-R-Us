using System;
using UnityEngine;

public class GasStationController : MonoBehaviour
{
    [Header("Gas Station Objects")]
    [SerializeField] private GameObject[] gasStationObjects;

    private void OnEnable()
    {
        GasStationEvents.OnShowGasStation += ShowGasStation;
        GasStationEvents.OnHideGasStation += HideGasStation;
        GasStationEvents.OnToggleGasStation += ToggleGasStation;
    }

    private void OnDisable()
    {
        GasStationEvents.OnShowGasStation -= ShowGasStation;
        GasStationEvents.OnHideGasStation -= HideGasStation;
        GasStationEvents.OnToggleGasStation -= ToggleGasStation;
    }

    public void ShowGasStation()
    {
        foreach (var obj in gasStationObjects)
        {
            if (obj != null) obj.SetActive(true);
        }
    }

    public void HideGasStation()
    {
        foreach (var obj in gasStationObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void ToggleGasStation()
    {
        bool isActive = gasStationObjects.Length > 0 && gasStationObjects[0].activeSelf;

        foreach (var obj in gasStationObjects)
        {
            if (obj != null) obj.SetActive(!isActive);
        }
    }
}


public static class GasStationEvents
{
    public static Action OnShowGasStation;
    public static Action OnHideGasStation;
    public static Action OnToggleGasStation;

    public static void Show() => OnShowGasStation?.Invoke();
    public static void Hide() => OnHideGasStation?.Invoke();
    public static void Toggle() => OnToggleGasStation?.Invoke();
}
