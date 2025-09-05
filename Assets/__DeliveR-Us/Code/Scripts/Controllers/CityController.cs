using Unity.VisualScripting;
using UnityEngine;

public class CityController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _scooterDummyTransform;

    [Space]
    [SerializeField] private Transform _playerOutsideMarket1;
    [SerializeField] private Transform _playerOutsideMarket2;
    [SerializeField] private Transform _playerOutsideMarket3;
    [SerializeField] private Transform _scooterOutsideMarket1;
    [SerializeField] private Transform _scooterOutsideMarket2;
    [SerializeField] private Transform _scooterOutsideMarket3;
    
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

        ValidatePositions();
    }

    private void ValidatePositions()
    {
        if (GameManager.Instance.PreviousScene == Scenes.SuperMarketScene)
        {
            MarketID targetMarket = GameManager.Instance.MarketEntranceID;

            if (targetMarket == MarketID.Market1)
            {
                if (_scooterDummyTransform != null)
                {
                    _scooterDummyTransform.position = _scooterOutsideMarket1.position;
                    _scooterDummyTransform.rotation = _scooterOutsideMarket1.rotation;
                }

                if (_playerTransform != null)
                {
                    _playerTransform.position = _playerOutsideMarket1.position;
                    _playerTransform.rotation = _playerOutsideMarket1.rotation;
                }
            }
            else if (targetMarket == MarketID.Market2)
            {
                if (_scooterDummyTransform != null)
                {
                    _scooterDummyTransform.position = _scooterOutsideMarket2.position;
                    _scooterDummyTransform.rotation = _scooterOutsideMarket2.rotation;
                }

                if (_playerTransform != null)
                {
                    _playerTransform.position = _playerOutsideMarket2.position;
                    _playerTransform.rotation = _playerOutsideMarket2.rotation;
                }
            }
            else
            {
                if (_scooterDummyTransform != null)
                {
                    _scooterDummyTransform.position = _scooterOutsideMarket3.position;
                    _scooterDummyTransform.rotation = _scooterOutsideMarket3.rotation;
                }

                if (_playerTransform != null)
                {
                    _playerTransform.position = _playerOutsideMarket3.position;
                    _playerTransform.rotation = _playerOutsideMarket3.rotation;
                }
            }

        }
     
        GameManager.Instance.GetThirdPersonControllerRef.gameObject.SetActive(true);
    }
}
