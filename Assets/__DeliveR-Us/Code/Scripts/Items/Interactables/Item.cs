using UnityEngine;

public class Item : Interactable
{
    #region Setters/Private Variables

    [SerializeField] private ItemData _iData;

    private GameObject _pf;

    #endregion

    #region Getters/Public Variables

    public ItemData GetItemData => _iData;

    #endregion

    private void OnEnable()
    {
        MarketTutorialController.OnTutorialComplete += HideHighlightItem;
        MarketTutorialController.TutorialItemHighlight += HighlightItem;
    }
    private void OnDisable()
    {
        MarketTutorialController.OnTutorialComplete -= HideHighlightItem;
        MarketTutorialController.TutorialItemHighlight -= HighlightItem;
    }


    public override void Interact()
    {
        if (!IsInteractive) return;

        base.Interact();

        if (OrderManager.Instance.AddItemToCart(_iData))
        {
            AudioManager.Instance.PlaySFX(AudioId.CollectItem);

            IsInteractive = false;

            ItemDescriptionPrompt.HideItemDescriptionAction?.Invoke();

            LeanTween.delayedCall(1, () =>
            {
                IsInteractive = true;
            });
        }
    }
    public override void OnStartDetection(string itemTag)
    {
        if (!IsInteractive) return;

        base.OnStartDetection(_iData.itemName);

        ItemDescriptionPrompt.DisplayItemDescriptionAction?.Invoke(_iData);
    }
    public override void OnStopDetection()
    {
        base.OnStopDetection();

        ItemDescriptionPrompt.HideItemDescriptionAction?.Invoke();
    }

    private void HighlightItem(string itemTag, GameObject pf)
    {
        if(_iData.itemName != itemTag) return;

        _pf = Instantiate(pf, transform);
        _pf.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        _pf.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);
    }

    private void HideHighlightItem()
    {
        if (_pf == null) return;

        Destroy(_pf);
    }
}
