using System;
using UnityEngine;

public class DonutShopController : MonoBehaviour
{
    [Header("Donut Shop Objects")]
    [SerializeField] private GameObject[] donutShopObjects;

    private void OnEnable()
    {
        DonutShopEvents.OnShowDonutShop += ShowDonutShop;
        DonutShopEvents.OnHideDonutShop += HideDonutShop;
        DonutShopEvents.OnToggleDonutShop += ToggleDonutShop;
    }

    private void OnDisable()
    {
        DonutShopEvents.OnShowDonutShop -= ShowDonutShop;
        DonutShopEvents.OnHideDonutShop -= HideDonutShop;
        DonutShopEvents.OnToggleDonutShop -= ToggleDonutShop;
    }

    public void ShowDonutShop()
    {
        foreach (var obj in donutShopObjects)
        {
            if (obj != null) obj.SetActive(true);
        }
    }

    public void HideDonutShop()
    {
        foreach (var obj in donutShopObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void ToggleDonutShop()
    {
        bool isActive = donutShopObjects.Length > 0 && donutShopObjects[0].activeSelf;

        foreach (var obj in donutShopObjects)
        {
            if (obj != null) obj.SetActive(!isActive);
        }
    }
}


public static class DonutShopEvents
{
    public static Action OnShowDonutShop;
    public static Action OnHideDonutShop;
    public static Action OnToggleDonutShop;

    public static void Show() => OnShowDonutShop?.Invoke();
    public static void Hide() => OnHideDonutShop?.Invoke();
    public static void Toggle() => OnToggleDonutShop?.Invoke();
}