using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderViewerPFItem : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private GameObject _sepratorBar;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Images")]
    [SerializeField] private Image _itemVisualImg;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    #endregion

    public void Init(OrderItemInfoData orderItemInfoData, bool isLastItem)
    {
        _itemVisualImg.sprite = orderItemInfoData.Item.itemIcon;
        _itemDescriptionText.text = $"{orderItemInfoData.ItemCount}X {orderItemInfoData.Item.itemName}: ${orderItemInfoData.Item.itemCost * orderItemInfoData.ItemCount}";

        _sepratorBar.SetActive(!isLastItem);
    }

    public void MarkAsCollected()
    {
        _canvasGroup.enabled = true;

    }

    public void MarkAsUnCollected()
    {
        _canvasGroup.enabled = false;
    }
}
