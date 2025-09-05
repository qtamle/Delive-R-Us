using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderPFItem : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private Image _avatarImg;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _headerTitleText;
    [SerializeField] private TextMeshProUGUI _totalItemsText;
    [SerializeField] private TextMeshProUGUI _targetTimerText;
    [SerializeField] private TextMeshProUGUI _timeLimited;

    [Header("Buttons")]
    [SerializeField] private Button _viewOrderBtn;
    [SerializeField] private Button _acceptOrderBtn;
    [SerializeField] private Button _declineOrderBtn;

    [Header("Read-Only")]
    [SerializeField] private OrderData _iOrderData;
    private float _timer; 
    private bool _isCounting = false;
    private PhoneUiController _phoneUI;
    public float RemainingTime = -1f;

    private bool _updateTimerText = false;
    private TimerOverlayUI _timerOverlay;

    LTDescr delLT;

    public bool IsAccepted { get; private set; }

    #endregion

    #region Getters/Public Variables

    public OrderData GetOrderData => _iOrderData;
    public bool OrderInProgress { private set; get; }

    #endregion


    private void Update()
    {
        //if (!_updateTimerText) return;

        //_targetTimerText.text = _timerOverlay.GetFormattedTime;

        if (_iOrderData == null) return;
        UpdateTimerUI();
    }

    private void OnDestroy()
    {
        if (_iOrderData != null)
            OrderTimerManager.Instance.RemoveOrder(_iOrderData);
    }

    public void Init(OrderData newOrderData, PhoneUiController phoneUI)
    {
        if (_timerOverlay == null)
            _timerOverlay = OrderManager.Instance.GetTimer;

        //_updateTimerText = false;

        transform.SetAsFirstSibling();

        _iOrderData = newOrderData;
        _phoneUI = phoneUI;

        _avatarImg.sprite = newOrderData.GetCustomerInfo.CustomerAvatar.icon;
        _headerTitleText.text = $"{newOrderData.GetCustomerInfo.CustomerName} JUST ORDERD";
        _totalItemsText.text = $"{newOrderData.GetTotalItemsInOrder:00}";
        _targetTimerText.text = $"{newOrderData.GetTotalOrderTime_MS}";

        _viewOrderBtn.onClick.AddListener(ViewOrderBtn_fn);
        _acceptOrderBtn.onClick.AddListener(AcceptOrderBtn_fn);
        _declineOrderBtn.onClick.AddListener(DeclineOrderBtn_fn);

        OrderTimerManager.Instance.AddOrder(newOrderData, OnOrderTimeout);

        if (PlayerPrefs.GetInt("OrderOne", 1) == 1)
        {
            _declineOrderBtn.interactable = false;
        }

        delLT = LeanTween.delayedCall(newOrderData.GetTotalOrderTime, () =>
        {
            if (!OrderInProgress && gameObject != null)
            {
                if (gameObject != null)
                    Destroy(gameObject);
            }
        });

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (_timeLimited == null || _iOrderData == null) return;

        if (IsAccepted)
        {
            _timeLimited.text = "ACCEPTED";
        }
        else
        {
            if (PlayerPrefs.GetInt("Tutorial1") == 1)
            {
                _timeLimited.text = "TIME LEFT: ∞";
            }
            else
            {
                float remaining = OrderTimerManager.Instance.GetRemainingTime(_iOrderData);
                int minutes = Mathf.FloorToInt(remaining / 60f);
                int seconds = Mathf.FloorToInt(remaining % 60f);
                _timeLimited.text = $"TIME LEFT: {minutes:00}:{seconds:00}";
            }
        }
    }

    private void ViewOrderBtn_fn()
    {
        OrderManager.Instance.ViewOrder(_iOrderData);
    }

    private void AcceptOrderBtn_fn() 
    {
        if (OrderManager.Instance.AcceptOrder(this))
        {
            if (_iOrderData != null)
                OrderTimerManager.Instance.RemoveOrder(_iOrderData);

            if (delLT != null)
                LeanTween.cancel(delLT.id);

            OrderInProgress = true;
            _isCounting = false;
            SetAcceptedState();

            _acceptOrderBtn.gameObject.SetActive(false);
            _declineOrderBtn.gameObject.SetActive(false);

            //_updateTimerText = true;
        }

        if (PlayerPrefs.GetInt("Tutorial1") == 1)
        {
            GasSystem.Instance.HidePopupAccepted();
            MissionManager.Instance.OpenMarketPortalIcon();
        }
    }

    private void DeclineOrderBtn_fn()
    {
        if (delLT != null)
            LeanTween.cancel(delLT.id);

        OrderManager.Instance.DeclineOrder(this);
    }

    public void SetAcceptedState()
    {
        OrderInProgress = true;
        IsAccepted = true;

        if (_iOrderData != null)
        {
            _iOrderData.SetOrderState(OrderStatus.OrderAccepted);
        }

        _acceptOrderBtn.interactable = false;
        _acceptOrderBtn.gameObject.SetActive(false);
        _declineOrderBtn.gameObject.SetActive(false);

        UpdateTimerUI();
    }

    public void SetOtherOrderState()
    {
        IsAccepted = false;
        _acceptOrderBtn.interactable = false;
    }

    public void ResetState()
    {
        OrderInProgress = false;
        IsAccepted = false;

        if (_iOrderData != null)
        {
            _iOrderData.SetOrderState(OrderStatus.Idle);
        }

        _acceptOrderBtn.gameObject.SetActive(true);
        _acceptOrderBtn.interactable = true;
        _declineOrderBtn.gameObject.SetActive(true);

        UpdateTimerUI();
    }

    private void OnOrderTimeout(OrderData order)
    {
        if (_phoneUI != null)
        {
            _phoneUI.RemoveOrder(this);
        }
    }
}
