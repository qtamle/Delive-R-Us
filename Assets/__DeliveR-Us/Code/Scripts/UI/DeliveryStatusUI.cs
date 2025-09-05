using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryStatusUI : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _onScreenNotificationText;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _etaText;
    [SerializeField] private TextMeshProUGUI _addressText;
    [SerializeField] private TextMeshProUGUI _paymentText;
    [SerializeField] private TextMeshProUGUI _fillerCountDownText;

    [Header("Images")]
    [SerializeField] private Image _timerFillerImg;

    [Header("Objects")]
    [SerializeField] private GameObject _onScreenNotificationObj;

    private OrderData _iOrderData;
    private OrderStatus _iOrderStatus;
    private TimerOverlayUI _timerOverlay;

    #endregion

    public string GetPaymentStr()
    {
        string paymentStr= $"${_iOrderData.GetBill}";

        if (_iOrderData.GetCustomerInfo.TipAmount > 0)
            paymentStr += " + Tip";

        return paymentStr;
    }

    private void Update()
    {
        if (_iOrderStatus == OrderStatus.Idle) return;
        if (_timerOverlay == null) return;

        _etaText.text = _timerOverlay.GetFormattedTime;
        _timerFillerImg.fillAmount = _timerOverlay.GetTimerFiller;
        _fillerCountDownText.text = _timerOverlay.GetTimerSeconds.ToString();
    }


    public void OnOrderAccept(OrderData orderData)
    {
        _iOrderData = orderData;

        if (GameManager.Instance.ActiveScene == Scenes.SuperMarketScene)
        {
            UpdateStatus(OrderStatus.Shopping);
        }
        else
        {
            UpdateStatus(OrderStatus.OrderAccepted);
        }
    }
    public void UpdateStatus(OrderStatus status)
    {
        if(_iOrderData == null && status != OrderStatus.Idle)
        {
            Debug.Log("Order Data not found, please accept order before updating Status");
            return;
        }

        if(_timerOverlay == null)
            _timerOverlay = OrderManager.Instance.GetTimer;

        _iOrderStatus = status;

        if (status == OrderStatus.Idle)
        {
            _iOrderData = null;

            _onScreenNotificationObj.SetActive(false);
            _statusText.text = "No active order";
            _addressText.text = "NA";
            _paymentText.text = "NA";

            _etaText.text = "NA";
            _timerFillerImg.fillAmount = 0;
            _fillerCountDownText.text = "0";
        }
        else if (status == OrderStatus.OrderAccepted)
        {
            _onScreenNotificationObj.SetActive(true);
            _onScreenNotificationText.text = "Head to market to collect items";

            _statusText.text = "Head to market to collect items";
            _addressText.text = "Reveals after pickup";
            _paymentText.text = GetPaymentStr();

        }
        else if (status == OrderStatus.Shopping)
        {
            _onScreenNotificationObj.SetActive(true);
            _onScreenNotificationText.text = "Gather the order items now.";

            _statusText.text = "Collect all listed items from the market.";
            _addressText.text = "Unlocks after checkout";
            _paymentText.text = GetPaymentStr();

        }
        else if (status == OrderStatus.Delivery)
        {
            _onScreenNotificationObj.SetActive(true);
            _onScreenNotificationText.text = "Items collected! Delivering now.";

            _statusText.text = "Items collected! Delivering now.";
            _addressText.text = _iOrderData.GetCustomerInfo.DeliveryAddress;
            _paymentText.text = GetPaymentStr();
        }

    }
}