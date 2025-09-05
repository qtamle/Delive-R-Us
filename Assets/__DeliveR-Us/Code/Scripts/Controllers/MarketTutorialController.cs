using System;
using UnityEngine;

public class MarketTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject _preFab;
    private bool IsTutorialDone => PlayerPrefs.GetInt("Tutorial", 0) == 1;
    private bool _hintSectionShown = false;


    public static Action OnTutorialComplete = delegate { };
    public static Action<string, GameObject> TutorialItemHighlight = delegate { };

    #region Unity Methods

    private void OnEnable()
    {
        OnTutorialComplete += StopTutorial;
    }
    private void OnDisable()
    {
        OnTutorialComplete -= StopTutorial;

        GameManager.Instance.StopTutorial();
    }

    private void Start()
    {
        if (IsTutorialDone) return;

        StartTutorial();
    }

    private void Update()
    {
        if (GameManager.Instance.InTutorial && _hintSectionShown)
        {

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _hintSectionShown = false;

                GameManager.Instance.DisplayAlert(GameStrings.GetFirstItemReminderAlert(), -1);

            }
        }
    }

    #endregion

    public void StartTutorial()
    {
        if (!OrderManager.Instance.PlayerBusy) return;

        OrderManager.Instance.GetTimer.Pause();

        GameManager.Instance.StartTutorial();

        string targetItem = OrderManager.Instance.GetCurrentOrderData.GetOrderInfo[0].Item.itemName;
        TutorialItemHighlight?.Invoke(targetItem, _preFab);

        GameManager.Instance.DisplayAlert(GameStrings.GetItemSectionHintAlert(targetItem), -1);

        _hintSectionShown = true;
    }

    public void StopTutorial()
    {
        GameManager.Instance.StopTutorial();

        PlayerPrefs.SetInt("Tutorial", 1);

        GameManager.Instance.DisplayAlert(GameStrings.GetTutorialCompleteAlert(), 3);

        LeanTween.delayedCall(1, () =>
        {
            OrderManager.Instance.GetTimer.Resume();
        });

    }
}
