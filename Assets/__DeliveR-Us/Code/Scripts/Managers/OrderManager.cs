using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MHUtility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OrderManager : MHUtility.Singleton<OrderManager>
{
    #region Setters/Private Variables

    [Header("Orders Data")]
    [SerializeField]
    private ItemData[] _orderItems;

    [SerializeField]
    private AvatarData _avatarsData;

    [SerializeField]
    private CurrentOrderData _activeOrderData;

    [SerializeField]
    private ViolationData _violationData;

    [Space]
    [SerializeField]
    private float _minIntervalBetweenOrders = 10;

    [SerializeField]
    private float _maxIntervalBetweenOrders = 60;

    [SerializeField]
    private int _minItemInOrder = 1;

    [SerializeField]
    private int _maxItemInOrder = 6;

    [SerializeField]
    private int _maxCountOfItem = 7;

    [Header("Controls")]
    [SerializeField]
    private KeyCode _openOrdersKey;

    [SerializeField]
    private KeyCode _closeOrdersKey;

    [SerializeField]
    private KeyCode _viewOrderListKey;

    [Header("UI Resources")]
    [SerializeField]
    private TimerOverlayUI _timerOverlay;

    [SerializeField]
    private OrderViewerUI _orderViewer;

    [SerializeField]
    private CartViewerUI _cartViewer;

    [SerializeField]
    private DeliveryStatusUI _deliveryStatusUI;

    [SerializeField]
    private UpdateNameUI _updateNameUI;

    [SerializeField]
    private PhoneUiController _phoneUiController;

    [SerializeField]
    private GameObject _phoneOverlay;

    [SerializeField]
    private Button _closeBtn;

    [SerializeField] private GameObject targetObject;

    [SerializeField] private ViolationData violationData;

    [Header("Read-Only")]
    [SerializeField]
    private List<OrderItemInfoData> _cartItems = new List<OrderItemInfoData>();

    private Coroutine _ordersCrt;

    private bool _viewingOrderListThroughKey = false;

    public static Action EnableCheckoutsAction = delegate { };
    public static Action DisableCheckoutsAction = delegate { };
    public static Action UpdateTargetMarketAction = delegate { };
    public static Action OrderCleanupAction = delegate { };
    public static event Action OnOrderReceived;
    public static event Action OnCompleteShipping;
    public static event Action HidePopupCity;

    #endregion

    #region Getters/Public Variables

    public bool OrderManagerAvailable { private set; get; } = false;
    public bool PhoneUIOpen { private set; get; } = false;
    public bool OrderViewUIOpen { private set; get; } = false;
    public bool OrderCollected { private set; get; } = false;
    public bool PlayerBusy { private set; get; } = false;

    public TimerOverlayUI GetTimer => _timerOverlay;
    public DeliveryStatusUI GetDeliveryStatusUI => _deliveryStatusUI;
    public OrderData GetCurrentOrderData => _activeOrderData.orderData;
    public int GetDeliveryPointIndex { private set; get; }
    public int GetMarketPortalIndex { private set; get; }

    #endregion

    #region Unity Methods

    private void Start()
    {
        RegisterButtonEvents();

        if (_maxItemInOrder >= _orderItems.Length)
            _maxItemInOrder = _orderItems.Length - 1;

        _deliveryStatusUI.UpdateStatus(OrderStatus.Idle);
    }

    private void Update()
    {
        if (!OrderManagerAvailable)
            return;

        if (Input.GetKeyDown(_openOrdersKey))
        {
            GameManager.Instance.HideAlert();
            HideOrder();

            if (GameManager.Instance.ShopUIOpen) { }
            else if (PhoneUIOpen)
            {
                HidePhoneUI();
            }
            else
            {
                DisplayPhoneUI();
            }
        }

        if (Input.GetKeyDown(_closeOrdersKey))
        {
            if (GameManager.Instance.QuitUIOpen)
            {
                GameManager.Instance.HideQuitUI();
                Cursor.lockState = CursorLockMode.Locked; 
                Cursor.visible = false;
            }
            else if (GameManager.Instance.AlertUIOpen)
            {
                GameManager.Instance.HideAlert();
            }
            else if (GameManager.Instance.ShopUIOpen)
            {
                ShopController.HideShopAction?.Invoke();
            }
            else if (OrderViewUIOpen)
            {
                HideOrder();
            }
            else if (PhoneUIOpen)
            {
                HidePhoneUI();
            }
            else
            {
                GameManager.Instance.DisplayQuitUI();
            }
        }

        if (PlayerBusy && Input.GetKeyDown(_viewOrderListKey))
        {
            if (!PhoneUIOpen && !_viewingOrderListThroughKey)
            {
                _viewingOrderListThroughKey = true;
                ViewOrder(_activeOrderData.orderData, viewingOrderListThroughKey: true);
            }
            else if (_viewingOrderListThroughKey)
            {
                HideOrder();
                HidePhoneUI();
            }
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents()
    {
        _closeBtn.onClick.RemoveAllListeners();

        _closeBtn.onClick.AddListener(CloseBtn_fn);
    }

    public void CloseBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);
        HidePhoneUI();
    }
    #endregion

    public void ActivateOrderManager()
    {
        OrderManagerAvailable = true;

        _ordersCrt = StartCoroutine(StartGettingOrder());
    }
    
    public void DeactivateOrderManager()
    {
        OrderManagerAvailable = false;

        if (_ordersCrt != null)
        {
            StopCoroutine(_ordersCrt);
            _ordersCrt = null;
        }
    }

    public void ViewOrder(OrderData orderData, bool viewingOrderListThroughKey = false)
    {
        OrderViewUIOpen = true;

        _orderViewer.gameObject.SetActive(true);

        _orderViewer.DisplayOrder(orderData, _cartItems.ToArray());

        if (viewingOrderListThroughKey)
        {
            PhoneUIOpen = true;

            GameManager.Instance.StopControllers();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameManager.Instance.DeactivateInteractionCrossHair();
            GameManager.Instance.HideInteractionUI();
            GameManager.Instance.HideMsg();
        }
    }

    public void HideOrder()
    {
        OrderViewUIOpen = false;

        if (_viewingOrderListThroughKey)
        {
            _viewingOrderListThroughKey = false;

            HidePhoneUI();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        _deliveryStatusUI.gameObject.SetActive(false);
        _orderViewer.gameObject.SetActive(false);
    }

    public bool AcceptOrder(OrderPFItem orderPF)
    {
        if (PlayerBusy)
        {
            ShowOrderMessage(GameStrings.GetAlreadyInOrderMsg());

            return false;
        }

        if (GameManager.Instance.ActiveScene == Scenes.SuperMarketScene)
        {
            ShowOrderMessage(GameStrings.GetNoInternetInMarketMsg());

            return false;
        }

        PlayerBusy = true;
        GetDeliveryPointIndex = -1;

        UpdateTargetMarketAction?.Invoke();

        HidePhoneUI();

        _activeOrderData.orderData = orderPF.GetOrderData.Clone();
        //_phoneUiController.RemoveOrder(orderPF);
        _phoneUiController.OnAcceptOrder(orderPF);

        _timerOverlay.gameObject.SetActive(true);
        _timerOverlay.StartTimer(_activeOrderData.orderData.GetTargetShoppingTime);

        _deliveryStatusUI.OnOrderAccept(_activeOrderData.orderData);

        LeanTween.delayedCall(
            1,
            () =>
            {
                ShowOrderMessage(
                    GameStrings.GetOrderStartedMsg(
                        orderPF.GetOrderData.GetCustomerInfo.CustomerName,
                        orderPF.GetOrderData.GetTotalItemsInOrder
                    )
                );
            }
        );

        return true;
    }

    public void DeclineOrder(OrderPFItem orderPF)
    {
        _violationData.AddViolation();

        _phoneUiController.RemoveOrder(orderPF);

        ShowOrderMessage(GameStrings.GetOrderDeclinedMsg());
    }

    public bool AddItemToCart(ItemData item)
    {
        if (!PlayerBusy)
        {
            GameManager.Instance.DisplayAlert(GameStrings.GetNoActiveOrderAlert(), 3);
            return false;
        }

        OrderItemInfoData currentOrderItemInfo = Array.Find(
            _activeOrderData.orderData.GetOrderInfo,
            x => x.Item.itemName == item.itemName
        );

        if (currentOrderItemInfo == null)
        {
            GameManager.Instance.DisplayAlert(GameStrings.GetItemNotInOrderAlert(), 3);

            return false;
        }

        if (GameManager.Instance.InTutorial)
        {
            if (
                currentOrderItemInfo == null
                || currentOrderItemInfo.Item.itemName
                    != _activeOrderData.orderData.GetOrderInfo[0].Item.itemName
            )
            {
                GameManager.Instance.DisplayAlert(
                    GameStrings.GetWrongTargetItemAlert(
                        _activeOrderData.orderData.GetOrderInfo[0].Item.itemName
                    ),
                    3
                );

                return false;
            }
            else
            {
                MarketTutorialController.OnTutorialComplete?.Invoke();
            }
        }

        OrderItemInfoData cartItem = _cartItems.Find(x => x.Item.itemName == item.itemName);

        if (cartItem == null)
        {
            OrderItemInfoData newCartItem = new OrderItemInfoData { Item = item, ItemCount = 1 };

            _cartItems.Add(newCartItem);
        }
        else if (cartItem.ItemCount >= currentOrderItemInfo.ItemCount)
        {
            GameManager.Instance.DisplayAlert(GameStrings.GetOrderItemAlreadyCollectedAlert(), 3);

            return false;
        }
        else
        {
            cartItem.ItemCount++;
        }

        _cartViewer.UpdateCart(_cartItems);

        ValidateCart();

        return true;
    }

    public void ValidateCart()
    {
        if (_cartItems.Count == _activeOrderData.orderData.GetOrderInfo.Length)
        {
            int targetItemsCount = 0;
            int currentItemsCount = 0;

            OrderItemInfoData[] targetItems = _activeOrderData.orderData.GetOrderInfo;

            foreach (var item in _cartItems)
            {
                currentItemsCount += item.ItemCount;
            }

            foreach (var item in targetItems)
            {
                targetItemsCount += item.ItemCount;
            }

            if (targetItemsCount == currentItemsCount)
                EnableCheckoutsAction?.Invoke();
            else
                DisableCheckoutsAction?.Invoke();
        }
        else
        {
            DisableCheckoutsAction?.Invoke();
        }
    }

    public void OrderFailed()
    {
        _violationData.AddViolation();

        GetDeliveryPointIndex = -1;
        GetMarketPortalIndex = -1;

        PlayerBusy = false;
        OrderCollected = false;

        _timerOverlay.gameObject.SetActive(false);

        _deliveryStatusUI.UpdateStatus(OrderStatus.Idle);
        _phoneUiController.OnOrderEnd();

        OrderCleanupAction?.Invoke();
        _cartItems.Clear();

        GameManager.Instance.DisplayAlert(GameStrings.GetOrderFailedAlert(), 3);

        LeanTween.delayedCall(
            3,
            () =>
            {
                ShowOrderMessage(
                    GameStrings.GetOrderFailedMsg(
                        _activeOrderData.orderData.GetCustomerInfo.CustomerName
                    )
                );
            }
        );

        _activeOrderData.orderData.CleanCurrentOrder();

        DisableCheckoutsAction?.Invoke();
    }

    public void OnOrderCollected()
    {
        GetDeliveryPointIndex = -1;
        GetMarketPortalIndex = -1;

        OrderCollected = true;
        _deliveryStatusUI.UpdateStatus(OrderStatus.Delivery);
        _timerOverlay.StartTimer(_activeOrderData.orderData.GetTargetDeliverTime);
    }

    public void OnOrderDelivered()
    {
        GetDeliveryPointIndex = -1;

        PlayerBusy = false;
        OrderCollected = false;

        _timerOverlay.gameObject.SetActive(false);

        _deliveryStatusUI.UpdateStatus(OrderStatus.Idle);
        _phoneUiController.OnOrderEnd();

        OrderCleanupAction?.Invoke();
        _cartItems.Clear();

        GameManager.Instance.HideInteractionUI();

        LeanTween.delayedCall(
            1,
            () =>
            {
                GameManager.Instance.DisplayAlert(GameStrings.GetOrderSuccessAlert(), 2);
            }
        );

        CustomerInfoData customerInfoData = new CustomerInfoData();

        customerInfoData = _activeOrderData.orderData.GetCustomerInfo;
        float bill = _activeOrderData.orderData.GetBill;
        float tip = customerInfoData.TipAmount;
        string customerName = customerInfoData.CustomerName;

        GameManager.Instance.GetPlayerDataSo.AddCoinsInWallet(bill + tip);

        LeanTween.delayedCall(
            2,
            () =>
            {
                ShowOrderMessage(GameStrings.GetOrderSuccessMsg(customerName, bill, tip));
            }
        );

        _activeOrderData.orderData.CleanCurrentOrder();

        if (PlayerPrefs.GetInt("CompleteShipping", 0) == 0)
        {
            LeanTween.delayedCall(4, () =>
            {
                if (HungerSystem.Instance.currentHunger >= 100)
                {
                    HungerSystem.Instance.ReduceHunger(50f);
                }
                else if (HungerSystem.Instance.currentHunger <= 50)
                {
                    HungerSystem.Instance.ReduceHunger(25f);
                }
                //OnCompleteShipping?.Invoke();
                //OnToEat?.Invoke();

                HungerSystem.Instance.GotoDonutShop();
                DonutShopEvents.Show();
                MissionManager.Instance.HideMarketPortalIcon();

                PlayerPrefs.SetInt("CompleteShipping", 1);
                PlayerPrefs.SetInt("HasReceivedOrderToday", 1);
                PlayerPrefs.Save();
            });
        }
    }

    public void UpdateDeliveryPoint(int deliveryPointIndex) =>
        GetDeliveryPointIndex = deliveryPointIndex;

    public void UpdateTargetMarketPortal(int marketPortalIndex) =>
        GetMarketPortalIndex = marketPortalIndex;

    private void DisplayPhoneUI()
    {
        _phoneOverlay.SetActive(true);

        GameManager.Instance.StopControllers();

        PhoneUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.DeactivateInteractionCrossHair();
        GameManager.Instance.HideInteractionUI();
        GameManager.Instance.HideMsg();
    }

    private void HidePhoneUI()
    {
        if (_updateNameUI.gameObject.activeInHierarchy)
        {
            _updateNameUI.gameObject.SetActive(false);

            return;
        }

        _phoneOverlay.SetActive(false);
        _deliveryStatusUI.gameObject.SetActive(false);

        GameManager.Instance.ActivateInteractionCrossHair();

        PhoneUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.ResumeControllers();
    }

    IEnumerator StartGettingOrder()
    {
        while (OrderManagerAvailable)
        {
            float waitTime = Random.Range(_minIntervalBetweenOrders, _maxIntervalBetweenOrders);

            yield return new WaitForSeconds(waitTime);

            TriggerNewOrder();
        }
    }

    private void TriggerNewOrder()
    {
        if (GameManager.Instance.IsRestingTime || GameManager.Instance.isShippingForBoss || GameOver.Instance.isShowing 
            || GameManager.Instance.IsTutorialActive() 
            || PlayerPrefs.GetInt("TutorialDay1") == 1
            || PlayerPrefs.GetInt("CityScenePopup") == 1 
            || PlayerPrefs.GetInt("HasReceivedOrderToday", 0) == 1
            || PlayerPrefs.GetInt("HasOrder") == 1
            )
        {
            Debug.Log("[OrderManager] Cannot receive orders during RESTING TIME.");
            return;
        }

        if (PlayerBusy)
            return;

        #region Total Main Item Type Count - Random

        int totalItemsInOrder = Random.Range(_minItemInOrder, _maxItemInOrder);

        #endregion

        #region Order Items - Random

        List<int> allIndexes = Enumerable.Range(0, _orderItems.Length).ToList();

        for (int i = 0; i < allIndexes.Count; i++)
        {
            int rand = Random.Range(i, allIndexes.Count);
            (allIndexes[i], allIndexes[rand]) = (allIndexes[rand], allIndexes[i]);
        }

        List<int> selectedIndexes = allIndexes.Take(totalItemsInOrder).ToList();

        #endregion

        List<int> distributedCounts = OrderUtils.DistributeTotalItemCount(
            _maxCountOfItem,
            totalItemsInOrder
        );

        List<OrderItemInfoData> newOrderData = new List<OrderItemInfoData>();

        for (int i = 0; i < selectedIndexes.Count; i++)
        {
            OrderItemInfoData orderItemInfo = new OrderItemInfoData
            {
                Item = _orderItems[selectedIndexes[i]],
                ItemCount = distributedCounts[i],
            };

            newOrderData.Add(orderItemInfo);
        }

        CustomerInfoData newCustomerData = new CustomerInfoData
        {
            CustomerAvatar = _avatarsData.GetRandomData,
            CustomerName = OrderUtils.GetRandomName,
            DeliveryAddress = OrderUtils.GetRandomAddress(),
            OrderId = OrderUtils.GetRandomOrderID(),
            TipAmount = OrderUtils.GetRandomTip(),
        };

        (float targetShoppingTimer, float targetDeliveryTimer) = OrderUtils.GetTimers(
            totalItemsInOrder
        );

        OrderData newData = new OrderData();

        newData.RegisterOrder(
            newOrderData.ToArray(),
            targetShoppingTimer,
            targetDeliveryTimer,
            newCustomerData
        );

        _phoneUiController.AddInOrder(newData.Clone());

        ShowOrderMessage(
            GameStrings.OrderNotificationMsg(newCustomerData.CustomerName, newOrderData)
        );

        CheckOrder();

        if (PlayerPrefs.GetInt("Tutorial1") == 1)
        {
            PlayerPrefs.SetInt("CityScenePopup", 0);
            PlayerPrefs.SetInt("HasOrder", 1);
            PlayerPrefs.Save();
        }
    }

    void CheckOrder()
    {
        if (
            PlayerPrefs.GetInt("TutorialDay1", 0) == 0 &&
            PlayerPrefs.GetInt("CityScenePopup", 0) == 0 &&
            PlayerPrefs.GetInt("HasShownOrderReceived", 0) == 0)
        {
            HidePopupCity?.Invoke();
            //OnOrderReceived?.Invoke();
            GasSystem.Instance.StartCoroutine(GasSystem.Instance.ShowPopupAccepted());
            PlayerPrefs.SetInt("Pickup", 1);
            PlayerPrefs.SetInt("OrderOne", 1);
            PlayerPrefs.SetInt("HasShownOrderReceived", 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowOrderMessage(string message)
    {
        GameManager.Instance.DisplayMsg(message, 5);
    }

    public void ClearAllOrders()
    {
        Debug.Log("[OrderManager] Clearing only idle orders due to sleep.");

        if (_phoneUiController.HasAcceptedOrder())
        {
            violationData.AddViolation();
        }

        if (_activeOrderData.orderData != null)
        {
            if (_activeOrderData.orderData.GetOrderStatus == OrderStatus.Idle)
            {
                _activeOrderData.orderData?.CleanCurrentOrder();
                _activeOrderData.orderData = new OrderData();

                PlayerBusy = false;
                OrderCollected = false;
                OrderViewUIOpen = false;

                _cartItems.Clear();
                _cartViewer.UpdateCart(_cartItems);

                _orderViewer.gameObject.SetActive(false);
                _deliveryStatusUI.gameObject.SetActive(false);
                _timerOverlay.gameObject.SetActive(false);

                _deliveryStatusUI.UpdateStatus(OrderStatus.Idle);

                _phoneUiController.OnOrderEnd();
                _phoneUiController.ClearIdleOrdersUI();

                OrderCleanupAction?.Invoke();
                DisableCheckoutsAction?.Invoke();
            }
        }
        else
        {
            Debug.Log("[OrderManager] Skipping clear - order is in progress.");
        }

        ClearIdleOrderPFItems();
    }

    private void ClearIdleOrderPFItems()
    {
        Transform orderListParent = _orderViewer.transform.parent;
        foreach (Transform child in orderListParent)
        {
            OrderPFItem item = child.GetComponent<OrderPFItem>();
            if (item != null && !item.IsAccepted) 
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log("[OrderManager] Cleared only Idle OrderPFItem prefabs.");
    }

    public void ForceClearAllOrders()
    {
        Debug.Log("[OrderManager] Force clearing ALL orders (any status).");

        if (_activeOrderData.orderData != null)
        {
            _activeOrderData.orderData?.CleanCurrentOrder();
            _activeOrderData.orderData = new OrderData();
        }

        PlayerBusy = false;
        OrderCollected = false;
        OrderViewUIOpen = false;

        _cartItems.Clear();
        _cartViewer.UpdateCart(_cartItems);

        _orderViewer.gameObject.SetActive(false);
        _deliveryStatusUI.gameObject.SetActive(false);
        _timerOverlay.gameObject.SetActive(false);

        _deliveryStatusUI.UpdateStatus(OrderStatus.Idle);

        _phoneUiController.OnOrderEnd();
        _phoneUiController.ClearIdleOrdersUI();

        OrderCleanupAction?.Invoke();
        DisableCheckoutsAction?.Invoke();

        ClearAllOrderPFItems();
    }

    private void ClearAllOrderPFItems()
    {
        Transform orderListParent = _orderViewer.transform.parent;
        foreach (Transform child in orderListParent)
        {
            OrderPFItem item = child.GetComponent<OrderPFItem>();
            if (item != null)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log("[OrderManager] Cleared all OrderPFItem prefabs.");
    }
}
