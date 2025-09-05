using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button continueButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Popup")]
    [SerializeField] private GameObject confirmPopup;
    [SerializeField] private float tweenDuration = 0.2f;

    [Header("ScriptableObject")]
    [SerializeField] private ViolationData violationData;
    [SerializeField] private BankDebtData bankDebtData;

    private bool _isStarting = false;

    private void Start()
    {
        _isStarting = false;

        if (PlayerPrefs.GetInt("DoneTutorial") == 0)
        {
            continueButton.interactable = false;
        }
    }

    #region Start Game
    public void OnStartButton()
    {
        if (_isStarting) return;

        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        if (PlayerPrefs.GetInt("DoneTutorial") == 1 && !GameManager.Instance.demoVersion)
        {
            ShowPopup();
        }
        else
        {
            StartGame();
        }

        _isStarting = true;
    }

    private void StartGame()
    {
        _isStarting = true;
        GameManager.Instance.DeleteKeys();
        violationData.ResetViolations();
        bankDebtData.ResetDebt();
        DeleteKeyPlayerPref();
        GameManager.Instance.StartNewGame();
    }

    private void ShowPopup()
    {
        if (confirmPopup == null) return;

        confirmPopup.SetActive(true);
        confirmPopup.transform.localScale = Vector3.zero;
        LeanTween.scale(confirmPopup, Vector3.one, tweenDuration).setEaseOutBack();
    }

    private void HidePopup()
    {
        if (confirmPopup == null) return;

        LeanTween.scale(confirmPopup, Vector3.zero, tweenDuration * 0.7f).setEaseInBack()
            .setOnComplete(() =>
            {
                confirmPopup.SetActive(false);
            });
    }

    public void OnConfirmYes()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        HidePopup();
        StartGame();
    }

    public void OnConfirmNo()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        HidePopup();
        _isStarting = false;
    }

    #endregion

    #region Settings

    public void OnSettingsButton()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive); 
        }
        else
        {
            Debug.LogWarning("[MenuController] Settings panel is not assigned!");
        }
    }

    #endregion

    #region Continue

    public void OnContinueButton()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        GameManager.Instance.ContinueGame();
    }

    #endregion

    #region Quit

    public void OnQuitButton()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        Debug.Log("Quit Game"); 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    void DeleteKeyPlayerPref()
    {
        PlayerPrefs.DeleteKey("TutorialDay1");
        PlayerPrefs.DeleteKey("CityScenePopup");
        PlayerPrefs.DeleteKey("Tutorial1");
        PlayerPrefs.DeleteKey("OrderSuccessful");
        PlayerPrefs.DeleteKey("HasShownOrderReceived");
        PlayerPrefs.DeleteKey("EnterSuperMarket");
        PlayerPrefs.DeleteKey("CompleteShipping");
        PlayerPrefs.DeleteKey("AteComplete");
        PlayerPrefs.DeleteKey("GasComplete");
        PlayerPrefs.DeleteKey("HasReceivedOrderToday");
        PlayerPrefs.DeleteKey("SkipSleep");
        PlayerPrefs.DeleteKey("TutorialDay2");
        PlayerPrefs.DeleteKey("DoneTutorial");
        PlayerPrefs.DeleteKey("TutorialDay3");
        PlayerPrefs.DeleteKey("OrderOne");
        PlayerPrefs.DeleteKey("NotFoundOrderDay3");
        PlayerPrefs.DeleteKey("BuySomething");
        PlayerPrefs.DeleteKey("OpenLaptop");
        PlayerPrefs.DeleteKey("GoHomeDay1");
        PlayerPrefs.DeleteKey("HasOrder");
        PlayerPrefs.DeleteKey("HidePortal");
        PlayerPrefs.DeleteKey("DoneSkipTutorial");
        PlayerPrefs.DeleteKey("SleepSkipDay1Tutorial");
        PlayerPrefs.DeleteKey("GoHomeDay2");
        PlayerPrefs.DeleteKey("Pickup");
        PlayerPrefs.DeleteKey("ShippingComplete");
        PlayerPrefs.Save();
    }
}
