using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopItemData[] _shopData;

    [Space]
    [SerializeField] private Laptop _laptop;
    [SerializeField] private ShopPFItem _shopPfItem;

    [Space]
    [SerializeField] private RectTransform _shopPFContainer;
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private Button _closeShopBtn;


    public static Action OpenShopAction = delegate { };
    public static Action HideShopAction = delegate { };

    #region Unity Methods

    private void OnEnable()
    {
        OpenShopAction += OpenShop;
        HideShopAction += CloseShop;
    }
    private void OnDisable()
    {
        OpenShopAction -= OpenShop;
        HideShopAction -= CloseShop;
    }

    private void Start()
    {
        RegisterBtnEvents();

        InitShop();
    }

    #endregion

    #region Register Button Events

    private void RegisterBtnEvents()
    {
        _closeShopBtn.onClick.RemoveAllListeners();

        _closeShopBtn.onClick.AddListener(CloseShopBtn_fn);
    }
    private void CloseShopBtn_fn()
    {
        CloseShop();
    }

    #endregion

    private void InitShop()
    {
        foreach(Transform T in _shopPFContainer.transform)
        {
            Destroy(T.gameObject);
        }

        List<string> boughtItems = GameManager.Instance.GetPlayerDataSo.GetBoughtItems;

        _shopData = _shopData.OrderBy(x => UnityEngine.Random.value).ToArray();

        foreach (ShopItemData shopItem in _shopData)
        {
            if (boughtItems.Contains(shopItem._iData.ItemName))
            {
                shopItem._iData.isBought = true;

                Instantiate(shopItem.IPrefab);
            }
            else
            {
                shopItem._iData.isBought = false;

                Instantiate(_shopPfItem, _shopPFContainer).Init(shopItem);
            }
        }
    }

    private void OpenShop()
    {
        _laptop.gameObject.SetActive(false);

        GameManager.Instance.UpdateOpenShopState(true);
        GameManager.Instance.StopControllers();

        _shopUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.DeactivateInteractionCrossHair();
        GameManager.Instance.HideInteractionUI();
        GameManager.Instance.HideMsg();
    }
    private void CloseShop()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        GameManager.Instance.UpdateOpenShopState(false);

        GameManager.Instance.ActivateInteractionCrossHair();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.ResumeControllers();

        _laptop.gameObject.SetActive(true);

        _shopUI.SetActive(false);
    }
}

