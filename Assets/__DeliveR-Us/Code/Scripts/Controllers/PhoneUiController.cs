using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUiController : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private OrderPFItem _orderPF;
    [SerializeField] private RectTransform _ordersPFHolder;

    [Header("Buttons")]
    [SerializeField] private Button _ordersBtn;
    [SerializeField] private Button _cartBtn;
    [SerializeField] private Button _profileBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _deliverySubBtn;
    [SerializeField] private Button _orderSubBtn;
    [SerializeField] private Button _updateNameUIBtn;
    [SerializeField] private Button _bankUIBtn;

    [Header("Panels")]
    [SerializeField] private GameObject _ordersUI;
    [SerializeField] private GameObject _cartUI;
    [SerializeField] private GameObject _profileUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _deliverySubUI;
    [SerializeField] private GameObject _updateNameUI;
    [SerializeField] private GameObject _bankUI;

    [Header("LT FX")]
    [SerializeField] private LeanTweenUIItem _ordersLTItem;
    [SerializeField] private LeanTweenUIItem _cartLTItem;
    [SerializeField] private LeanTweenUIItem _profileLTItem;
    [SerializeField] private LeanTweenUIItem _settingsLTItem;
    [SerializeField] private LeanTweenUIItem _bankLTItem;

    [Header("GameObject")]
    public GameObject warningObj;

    private List<OrderPFItem> _instantiatedOrderPF = new List<OrderPFItem>();
    #endregion

    #region Unity Events

    private void OnEnable()
    {
        DefaultUI();
    }
    private void Start()
    {
        RegisterButtonEvents();
        ActiveWarning();
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents()
    {
        _ordersBtn.onClick.RemoveAllListeners();
        _cartBtn.onClick.RemoveAllListeners();
        _profileBtn.onClick.RemoveAllListeners();
        _settingsBtn.onClick.RemoveAllListeners();
        _deliverySubBtn.onClick.RemoveAllListeners();
        _orderSubBtn.onClick.RemoveAllListeners();
        _updateNameUIBtn.onClick.RemoveAllListeners();
        _bankUIBtn.onClick.RemoveAllListeners();

        _ordersBtn.onClick.AddListener(OrdersBtn_fn);
        _cartBtn.onClick.AddListener(CartBtn_fn);
        _profileBtn.onClick.AddListener(ProfileBtn_fn);
        _settingsBtn.onClick.AddListener(SettingsBtn_fn);
        _deliverySubBtn.onClick.RemoveAllListeners();
        _orderSubBtn.onClick.RemoveAllListeners();
        _deliverySubBtn.onClick.AddListener(DeliverySubBtn_fn);
        _orderSubBtn.onClick.AddListener(OrderSubBtn_fn);
        _updateNameUIBtn.onClick.AddListener(UpdateNameUIBtn_fn);
        _bankUIBtn.onClick.AddListener(BankBtn_fn);
    }

    private void ProfileBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _ordersUI.SetActive(false);
        _cartUI.SetActive(false);
        _settingsUI.SetActive(false);
        _deliverySubUI.SetActive(false);
        _updateNameUI.SetActive(false);
        _bankUI.SetActive(false);

        _ordersLTItem.enabled = false;
        _cartLTItem.enabled = false;
        _settingsLTItem.enabled = false;
        _bankLTItem.enabled = false;

        _profileUI.SetActive(true);
        _profileLTItem.enabled = true;
    }
    private void OrdersBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _profileUI.SetActive(false);
        _cartUI.SetActive(false);
        _settingsUI.SetActive(false);
        _deliverySubUI.SetActive(false);
        _updateNameUI.SetActive(false);
        _bankUI.SetActive(false);

        _profileLTItem.enabled = false;
        _cartLTItem.enabled = false;
        _settingsLTItem.enabled = false;
        _bankLTItem.enabled = false;

        _ordersUI.SetActive(true);
        _ordersLTItem.enabled = true;
    }
    private void CartBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _profileUI.SetActive(false);
        _ordersUI.SetActive(false);
        _settingsUI.SetActive(false);
        _deliverySubUI.SetActive(false);
        _updateNameUI.SetActive(false);
        _bankUI.SetActive(false);

        _profileLTItem.enabled = false;
        _ordersLTItem.enabled = false;
        _settingsLTItem.enabled = false;
        _bankLTItem.enabled = false;

        _cartUI.SetActive(true);
        _cartLTItem.enabled = true;
    }
    private void SettingsBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _profileUI.SetActive(false);
        _ordersUI.SetActive(false);
        _cartUI.SetActive(false);
        _deliverySubUI.SetActive(false);
        _updateNameUI.SetActive(false);
        _bankUI.SetActive(false);

        _profileLTItem.enabled = false;
        _ordersLTItem.enabled = false;
        _cartLTItem.enabled = false;
        _bankLTItem.enabled = false;

        _settingsUI.SetActive(true);
        _settingsLTItem.enabled = true;
    }
    private void DeliverySubBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _ordersUI.SetActive(false);
        _deliverySubUI.SetActive(true);
    }
    private void OrderSubBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _deliverySubUI.SetActive(false);
        _ordersUI.SetActive(true);
    }

    private void UpdateNameUIBtn_fn()
    {
        _updateNameUI.SetActive(true);
    }

    private void BankBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        _profileUI.SetActive(false);
        _ordersUI.SetActive(false);
        _cartUI.SetActive(false);
        _settingsUI.SetActive(false);
        _deliverySubUI.SetActive(false);
        _updateNameUI.SetActive(false);

        _profileLTItem.enabled = false;
        _ordersLTItem.enabled = false;
        _cartLTItem.enabled = false;
        _settingsLTItem.enabled = false;

        _bankUI.SetActive(true);
        _bankLTItem.enabled = true;
    }
    #endregion

    private void DefaultUI()
    {
        _ordersUI.SetActive(false);
        _cartUI.SetActive(false);
        _settingsUI.SetActive(false);
        _deliverySubUI.SetActive(false);
        _updateNameUI.SetActive(false);
        _bankUI.SetActive(false);

        _ordersLTItem.enabled = false;
        _cartLTItem.enabled = false;
        _settingsLTItem.enabled = false;
        _bankLTItem.enabled = false;

        _profileUI.SetActive(true);
        _profileLTItem.enabled = true;
    }

    public void AddInOrder(OrderData newOrderData)
    {
        OrderPFItem order = Instantiate(_orderPF, _ordersPFHolder);
        order.Init(newOrderData, this);

        _instantiatedOrderPF.Add(order);

        if (_instantiatedOrderPF.Count > 10)
        {
            Destroy(_instantiatedOrderPF[0].gameObject);
            _instantiatedOrderPF.RemoveAt(0);
        }

        ActiveWarning();
    }
    public void RemoveOrder(OrderPFItem orderPFItem)
    {
        if (_instantiatedOrderPF.Contains(orderPFItem))
        {
            _instantiatedOrderPF.Remove(orderPFItem);
        }

        Destroy(orderPFItem.gameObject);

        ActiveWarning();
    }
    public void OnAcceptOrder(OrderPFItem orderPFItem)
    {
        //foreach (var item in _instantiatedOrderPF)
        //{
        //    if (!item.Equals(orderPFItem))
        //        Destroy(item.gameObject);
        //    else
        //        continue;
        //}

        foreach (var item in _instantiatedOrderPF)
        {
            if (item != null)
            {
                if (item == orderPFItem)
                {
                    item.SetAcceptedState();
                }
                else
                {
                    item.SetOtherOrderState();
                }
            }
        }
    }
    public void OnOrderEnd()
    {
        if (GameManager.Instance.isSleeping)
        {
            List<OrderPFItem> toRemove = new List<OrderPFItem>();

            foreach (var item in _instantiatedOrderPF)
            {
                if (item != null && !item.IsAccepted) 
                {
                    Destroy(item.gameObject);
                    toRemove.Add(item);
                }
            }

            foreach (var item in toRemove)
            {
                _instantiatedOrderPF.Remove(item);
            }

            ActiveWarning();
            return; 
        }

        OrderPFItem completedOrder = null;

        foreach (var item in _instantiatedOrderPF)
        {
            if (item != null && item.IsAccepted)
            {
                completedOrder = item;
                break;
            }
        }

        if (completedOrder != null)
        {
            _instantiatedOrderPF.Remove(completedOrder);
            Destroy(completedOrder.gameObject);
        }

        List<OrderPFItem> toReset = new List<OrderPFItem>();
        foreach (var item in _instantiatedOrderPF)
        {
            if (item != null && !item.IsAccepted)
            {
                item.ResetState();
                toReset.Add(item);
            }
        }

        ActiveWarning();
    }

    public void ClearIdleOrdersUI()
    {
        List<OrderPFItem> toRemove = new List<OrderPFItem>();

        foreach (var item in _instantiatedOrderPF)
        {
            if (item != null && !item.IsAccepted) 
            {
                Destroy(item.gameObject);
                toRemove.Add(item);
            }
        }

        foreach (var item in toRemove)
        {
            _instantiatedOrderPF.Remove(item);
        }

        ActiveWarning(); 
    }

    public void ActiveWarning()
    {
        if (warningObj == null) return;

        LeanTween.cancel(warningObj);

        if (HasOrders())
        {
            if (!warningObj.activeSelf)
            {
                warningObj.SetActive(true);
            }

            warningObj.transform.localScale = Vector3.one;

            LeanTween.scale(warningObj, Vector3.one * 1.2f, 0.8f)
                .setEaseInOutSine()
                .setLoopPingPong();
        }
        else
        {
            if (warningObj.activeSelf)
            {
                LeanTween.cancel(warningObj);
                LeanTween.scale(warningObj, Vector3.zero, 0.3f)
                    .setEaseInBack()
                    .setOnComplete(() => warningObj.SetActive(false));
            }
        }
    }

    public bool HasOrders()
    {
        return _instantiatedOrderPF != null && _instantiatedOrderPF.Count > 0;
    }

    public bool HasAcceptedOrder()
    {
        foreach (var item in _instantiatedOrderPF)
        {
            if (item != null && item.IsAccepted)
                return true;
        }
        return false;
    }
}
