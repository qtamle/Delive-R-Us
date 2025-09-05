using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPFItem : MonoBehaviour
{
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _iTitleText;
    [SerializeField] private TextMeshProUGUI _iDescriptionText;
    [SerializeField] private TextMeshProUGUI _iPriceText;
    
    [SerializeField] private Button _buyBtn;

    private ShopItemData _iShopData;

    public void Init(ShopItemData shopItemData)
    {
        _iShopData = shopItemData;

        _iconImg.sprite = _iShopData._iData.ItemIcon;
        _iTitleText.text = _iShopData._iData.ItemName;
        _iDescriptionText.text = _iShopData._iData.ItemDescription;
        _iPriceText.text = $"Price: ${_iShopData._iData.ItemPrice}";

        _buyBtn.onClick.RemoveAllListeners();
        _buyBtn.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        if (_iShopData._iData == null || _iShopData._iData.isBought)
        {
            Destroy(gameObject);
            return;
        }

        AudioManager.Instance.PlaySFX(AudioId.BtnClick);


        PlayerDataSO playerDataSO = GameManager.Instance.GetPlayerDataSo;

        string itemName = _iShopData._iData.ItemName;
        float itemPrice = _iShopData._iData.ItemPrice;

        if (playerDataSO.CanBuyItem(itemPrice))
        {
            playerDataSO.RemovePlayerCoins(itemPrice, saveData: false);
            playerDataSO.OnItemBought(itemName, saveData: true);

            Instantiate(_iShopData.IPrefab);

            Destroy(gameObject);

            GameManager.Instance.DisplayAlert(GameStrings.GetPurchaseSuccessAlert(itemName, itemPrice), 3);

            if (PlayerPrefs.GetInt("OpenLaptop") == 1)
            {
                Day3Tutorial.Instance.tutorialTargetObject.SetActive(false);
                Day3Tutorial.Instance.areaBlue.SetActive(true);
                PlayerPrefs.SetInt("BuySomething", 0);
                PlayerPrefs.SetInt("OpenLaptop", 0);
                PlayerPrefs.Save();

                Bed bed = FindFirstObjectByType<Bed>();
                if (bed != null)
                {
                    bed.EnableInteraction();
                }

                GameManager.Instance.ShowFloatingMessage("All done, let’s head out and get to work!");
            }
        }
        else
        {
            GameManager.Instance.DisplayAlert(GameStrings.GetPurchaseFailedAlert(itemName, itemPrice), 3);

        }
    }
}
