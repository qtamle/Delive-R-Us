using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CartViewerUI : MonoBehaviour
{
    [SerializeField] private CartViewerPFItem[] _cartViewerPFItems;
    [SerializeField] private TextMeshProUGUI[] _cartItemsText;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _totalBillText;
    //[SerializeField] private TextMeshProUGUI _checkoutBtn;


    [Header("Read-Only")]
    [SerializeField] private List<OrderItemInfoData> _cartItems = new List<OrderItemInfoData>();

    private void OnEnable()
    {
        ValidateCartUI();
    }

    public void UpdateCart(List<OrderItemInfoData> cartData)
    {
        _cartItems = cartData;
    }

    public void DeleteCartItem(OrderItemInfoData cartItemInfo)
    {
        _cartItems.Remove(cartItemInfo);

        ValidateCartUI();
    }

    public void ValidateCartUI()
    {
        int totalItemsInCart = _cartItems.Count;
        float totalBill = 0;

        for (int i = 0; i < _cartViewerPFItems.Length; i++)
        {
            if (i < totalItemsInCart)
            {
                _cartItemsText[i].gameObject.SetActive(true);
                _cartViewerPFItems[i].gameObject.SetActive(true);

                OrderItemInfoData cartItemInfo = _cartItems[i];

                float ItemCost = cartItemInfo.Item.itemCost * cartItemInfo.ItemCount;
                totalBill += ItemCost;

                _cartViewerPFItems[i].Init(cartItemInfo, this);
                _cartItemsText[i].text = $"{cartItemInfo.ItemCount}X {cartItemInfo.Item.itemName}: ${ItemCost}";
            }
            else
            {
                _cartItemsText[i].gameObject.SetActive(false);
                _cartViewerPFItems[i].gameObject.SetActive(false);
            }
        }

        _totalBillText.text = $"Total Bill: ${totalBill}";
    }

}
