using UnityEngine;

public class ApartmentController : MonoBehaviour
{
    private OrderManager _orderManager;

    #region Unity Methods

    private void Start()
    {
        Init();
    }

    #endregion

    private void Init()
    {
        GameManager.Instance?.DoFadeIn(disableOnComplete: true);
        GameManager.Instance?.ActivateInteractionCrossHair();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _orderManager = OrderManager.Instance;

        if (_orderManager == null) return;

        if (_orderManager.PlayerBusy)
        {
            if (_orderManager.OrderCollected)
                _orderManager.GetDeliveryStatusUI.UpdateStatus(OrderStatus.Delivery);
            else
                _orderManager.GetDeliveryStatusUI.UpdateStatus(OrderStatus.OrderAccepted);
        }
        else
        {
            _orderManager.GetDeliveryStatusUI.UpdateStatus(OrderStatus.Idle);
        }
        
        GameManager.Instance.HideInteractionUI();

        //GameManager.Instance.CheckAndSchedulePayout();

        GameManager.Instance.GetThirdPersonControllerRef.gameObject.SetActive(true);
    }
}
