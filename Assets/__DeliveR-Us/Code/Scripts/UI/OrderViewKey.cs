using UnityEngine;

public class OrderViewKey : MonoBehaviour
{
    [SerializeField] private GameObject _contentHolder;

    private void OnEnable()
    {
        OrderManager.UpdateTargetMarketAction += DisplayUI;
        OrderManager.OrderCleanupAction += HideUI;
    }
    private void OnDisable()
    {
        OrderManager.UpdateTargetMarketAction -= DisplayUI;
        OrderManager.OrderCleanupAction -= HideUI;
    }

    private void Start()
    {
        if (OrderManager.Instance.PlayerBusy)
            DisplayUI();
    }

    private void DisplayUI()
    {
        _contentHolder.SetActive(true);
    }
    private void HideUI()
    {
        _contentHolder.SetActive(false);
    }


}
