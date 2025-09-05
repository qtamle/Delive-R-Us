using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CartViewerPFItem : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private GameObject _interactableBtnsObj;

    [Header("Images")]
    [SerializeField] private Image _itemVisualImg;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private TextMeshProUGUI _quantityText;

    [Header("Buttons")]
    [SerializeField] private Button _deleteBtn;
    [SerializeField] private Button _incrCountBtn;
    [SerializeField] private Button _decrCountBtn;

    [SerializeField] private OrderItemInfoData _iCartItemInfoData;

    private CartViewerUI _iCartViewer;

    #endregion

    public void Init(OrderItemInfoData cartItemInfoData, CartViewerUI cartViewer)
    {
        _iCartItemInfoData = cartItemInfoData;
        _iCartViewer = cartViewer;

        _itemVisualImg.sprite = cartItemInfoData.Item.itemIcon;
        _itemNameText.text = cartItemInfoData.Item.itemName;
        _itemPriceText.text = $"${cartItemInfoData.Item.itemCost}";
        _quantityText.text = $"{cartItemInfoData.ItemCount:00}";

        _interactableBtnsObj.SetActive(GameManager.Instance.ActiveScene == Scenes.SuperMarketScene);

        RegisterBtnEvents();
    }

    private void RegisterBtnEvents()
    {
        _deleteBtn.onClick.RemoveAllListeners();
        _incrCountBtn.onClick.RemoveAllListeners();
        _decrCountBtn.onClick.RemoveAllListeners();

        _deleteBtn.onClick.AddListener(DeleteBtn_fn);
        _incrCountBtn.onClick.AddListener(IncrBtn_fn);
        _decrCountBtn.onClick.AddListener(DecrBtn_fn);
    }

    private void DeleteBtn_fn()
    {
        _iCartViewer.DeleteCartItem(_iCartItemInfoData);

        OrderManager.Instance.ValidateCart();
    }

    private void IncrBtn_fn()
    {
        _iCartItemInfoData.ItemCount++;

        _iCartViewer.ValidateCartUI();

        OrderManager.Instance.ValidateCart();
    }

    private void DecrBtn_fn()
    {
        if (_iCartItemInfoData.ItemCount < 1) return;

        if(_iCartItemInfoData.ItemCount == 1)
        {
            _iCartViewer.DeleteCartItem(_iCartItemInfoData);

            OrderManager.Instance.ValidateCart();

            return;
        }

        _iCartItemInfoData.ItemCount--;

        _iCartViewer.ValidateCartUI();

        OrderManager.Instance.ValidateCart();
    }
}
