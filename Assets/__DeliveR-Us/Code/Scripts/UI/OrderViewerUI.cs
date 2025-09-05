using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderViewerUI : MonoBehaviour
{
    [SerializeField] private OrderViewerPFItem[] orderViewerPFItems;
    [SerializeField] private TextMeshProUGUI _billText;

    [SerializeField] private Button _closeOrderViewerBtn;

    private void Start()
    {
        _closeOrderViewerBtn.onClick.RemoveAllListeners();
        _closeOrderViewerBtn.onClick.AddListener(CloseOrderViewerBtn_fn);
    }

    public void DisplayOrder(OrderData orderData, OrderItemInfoData[] cartData)
    {
        int totalItemsInOrder = orderData.GetOrderInfo.Length;
        float totalBill = orderData.GetBill;

        for (int i = 0; i < orderViewerPFItems.Length; i++)
        {
            if( i <  totalItemsInOrder)
            {
                OrderViewerPFItem orderViewerPFItem = orderViewerPFItems[i];

                orderViewerPFItem.Init(orderData.GetOrderInfo[i], i == totalItemsInOrder - 1);
                orderViewerPFItem.gameObject.SetActive(true);

                OrderItemInfoData cartItem = Array.Find(cartData, x=>x.Item.itemName == orderData.GetOrderInfo[i].Item.itemName);
                int targetCount = orderData.GetOrderInfo[i].ItemCount;

                if (cartItem != null && cartItem.ItemCount == targetCount)
                {
                    orderViewerPFItem.MarkAsCollected();
                }
                else
                {
                    orderViewerPFItem.MarkAsUnCollected();
                }
            }
            else
            {
                orderViewerPFItems[i].gameObject.SetActive(false);
            }

        }
        _billText.text = $"Total Bill: ${totalBill}";

    }
    private void CloseOrderViewerBtn_fn()
    {
        OrderManager.Instance.HideOrder();
    }
}
