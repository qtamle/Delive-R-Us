using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionPrompt : MonoBehaviour
{
    [SerializeField] private RectTransform _contentHolder;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private string _endingStr = "\n<color=#1775F8><size=35>Add <b><color=#FEC601>1</color></b> to Cart</size></color>";

    public static Action<ItemData> DisplayItemDescriptionAction = delegate { };
    public static Action HideItemDescriptionAction = delegate { };

    private void OnEnable()
    {
        DisplayItemDescriptionAction += DisplayItemDescription;
        HideItemDescriptionAction += HideItemDescription;
    }
    private void OnDisable()
    {
        DisplayItemDescriptionAction -= DisplayItemDescription;
        HideItemDescriptionAction -= HideItemDescription;
    }

    private void DisplayItemDescription(ItemData _itemData)
    {
        _itemIcon.sprite = _itemData.itemIcon;

        _contentHolder.gameObject.SetActive(true);

        if (_itemDescriptionText != null)
        {
            _itemDescriptionText.text = _itemData.itemName + _endingStr;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentHolder);
        }
    }

    private void HideItemDescription()
    {
        _contentHolder.gameObject.SetActive(false);
    }
}
