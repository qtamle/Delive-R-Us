using MHUtility;
using SMPScripts;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    #region Setters/Private Variables

    [SerializeField]
    private PlayerDataSO _playerDataSo;

    [Header("Resources")]
    [SerializeField]
    private GameObject _fadeInOverlay;

    [SerializeField]
    private GameObject _fadeOutOverlay;

    [SerializeField]
    private GameObject _interactionOverlay;

    [Space]
    [SerializeField]
    private SceneTransitionOverlayUI _sceneTransitionOverlay;

    [SerializeField]
    private MsgOverlayUI _msgOverlay;

    [SerializeField]
    private AlertOverlayUI _alertOverlay;

    [SerializeField]
    private QuitOverlayUI _quitOverlay;

    [Space]
    [SerializeField]
    private TextMeshProUGUI _interactionText;

    [Space]
    [SerializeField]
    private Image _interactionCrossHair;

    public PhoneUiController phoneUIController;

    private Color _defaultCrossHairColor;
    private ThirdPersonController _playerController;
    private MotoController _motoController;
    private ObjectInteractions _playerInteractionController;
    private PlayerFollow_MiniMap _playerFollowMiniMap;

    public static Action OnUserDataUpdate = delegate { };

    [Header("Time")]
    public GameObject timeObject;
    public TextMeshProUGUI timeDisplayText;
    public TextMeshProUGUI dayDisplayText;
    public TextMeshProUGUI timeDisplayText2;
    public TextMeshProUGUI dayDisplayText2;
    public CanvasGroup fadeCanvasGroup;
    public int currentDay = 1;
    private const int maxDay = 30;
    private float dayDurationRealSeconds = 1200f;
    private float elapsedTime = 0f;
    private TimeSpan currentGameTime;
    public bool payoutDoneToday = false;
    private TimeSpan previousGameTime;
    public bool isTimePaused = false;

    [Header("Sleep")]
    public TextMeshProUGUI messageText;
    private float nextRestReminderTime = 0f;
    private bool hasShownRestMessage = false;
    public bool isSleeping = false;
    private Bed bed;

    [Header("Rent")]
    [SerializeField]
    private CanvasGroup billPopupCanvasGroup;
    private bool isRentDue = false;
    bool isRentPaid = false;
    private bool hasShownRentMessage = false;
    public bool isShippingForBoss = false;

    [Header("Day/Night")]
    public float minRotation = -30f;
    public float maxRotation = 210f;
    private Color morningColor = Color.white;
    private Color noonColor = new Color(1f, 0.95f, 0.8f);
    private Color eveningColor = new Color(1f, 0.7f, 0.5f);
    private Color nightColorSky = new Color(0.45f, 0.45f, 0.55f);
    private Light directionalLight;
    private string currentSceneName;
    public Material morningSkybox;
    public Material noonSkybox;
    public Material eveningSkybox;
    public Material nightSkybox;

    [Header("Hunger")]
    public HungerSystem hungerSystem;
    public GameObject sceneTransitionOverlay;

    [Header("Debt")]
    public BankDebtData bankDebtData;
    public GameOver gameOver;
    public TextMeshProUGUI debtCountdownText;
    private int previousDay;

    [Header("Violation")]
    public ViolationData violationData;

    [Header("Show text")]
    private Queue<(string text, float duration, Action onComplete)> messageQueue = new Queue<(string, float, Action)>();
    private HashSet<string> activeMessages = new HashSet<string>();
    private bool isShowingMessage = false;

    [Header("Demo")]
    public bool demoVersion = false;
    private ITutorial activeTutorial;
    [SerializeField] private GameObject[] objectsToDisable;
    [SerializeField] private GameObject PopupDemo;
    //private bool isShowDay3 = false;

    public event Action showTutorialDay2;
    public event Action showTutorialDay3;
    //public static event Action OnToEat;

    public void UpdateOpenShopState(bool state) => ShopUIOpen = state;

    #endregion

    #region Getters/Public Variables

    public bool InTutorial { get; private set; } = false;
    public bool AlertUIOpen { private set; get; } = false;
    public bool ShopUIOpen { private set; get; } = false;
    public bool QuitUIOpen { private set; get; } = false;
    public bool IsRestingTime { get; private set; } = false;

    public Scenes ActiveScene { private set; get; } = Scenes.LoadingScene;
    public Scenes PreviousScene { private set; get; } = Scenes.LoadingScene;
    public MarketID MarketEntranceID { private set; get; } = MarketID.Market1;

    public PlayerDataSO GetPlayerDataSo => _playerDataSo;
    public ThirdPersonController GetThirdPersonControllerRef
    {
        get
        {
            if (_playerController == null)
                _playerController = FindFirstObjectByType<ThirdPersonController>(
                    FindObjectsInactive.Include
                );

            return _playerController;
        }
    }
    public MotoController GetMotoControllerRef
    {
        get
        {
            if (_motoController == null)
                _motoController = FindFirstObjectByType<MotoController>(
                    FindObjectsInactive.Include
                );

            return _motoController;
        }
    }
    public ObjectInteractions GetPlayerInteractionControllerRef
    {
        get
        {
            if (_playerInteractionController == null)
                _playerInteractionController = FindFirstObjectByType<ObjectInteractions>(
                    FindObjectsInactive.Include
                );

            return _playerInteractionController;
        }
    }
    public PlayerFollow_MiniMap GetPlayerFollowMiniMapRef
    {
        get
        {
            if (_playerFollowMiniMap == null)
                _playerFollowMiniMap = FindFirstObjectByType<PlayerFollow_MiniMap>(
                    FindObjectsInactive.Include
                );

            return _playerFollowMiniMap;
        }
    }
    public Transform GetActivePlayerTransform { private set; get; }

    #endregion

    #region Mission
    private bool _waitingMissionAfterCityLoad = false;

    #endregion

    #region Unity Methods

    private void Start()
    {
        fadeCanvasGroup.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);

        //Init();

        if (currentDay < 1 || currentDay > 30)
        { 
            currentDay = 1;
            violationData.ResetViolations();
        }

        payoutDoneToday = false;
        previousGameTime = TimeSpan.Zero;

        isRentPaid = PlayerPrefs.GetInt("RentPaid", 0) == 1;

        currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (currentSceneName == "CityScene")
            FindDirectionalLight();

        if (PlayerPrefs.GetInt("isShippingForBoss", 0) == 1)
        {
            isShippingForBoss = true;
        }
        else
        {
            isShippingForBoss = false;
        }

        if (isShippingForBoss)
        {
            StartCoroutine(ShippingWarningRoutine());
        }

        if (!PlayerPrefs.HasKey("SkipSleep"))
        {
            PlayerPrefs.SetInt("SkipSleep", 0);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("DoneTutorial"))
        {
            PlayerPrefs.SetInt("DoneTutorial", 0);
            PlayerPrefs.Save();
        }

        InvokeRepeating(nameof(AutoSave), 10f, 10f);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "LoadingScene" || sceneTransitionOverlay.activeInHierarchy || SceneManager.GetActiveScene().name == "MenuScene")
            return;

        if (isTimePaused || GameOver.Instance.isShowing)
            return;

        if (PlayerPrefs.GetInt("Tutorial1", 0) == 1)
        {
            elapsedTime = GetElapsedTime(7, 0);
            timeDisplayText.text = ""; 
            timeDisplayText2.text = "";
            timeObject.SetActive(false);

            dayDisplayText.text = $"Day {currentDay}";
            dayDisplayText2.text = $"Day {currentDay}";

            return; 
        }

        if (PlayerPrefs.GetInt("Tutorial1", 0) == 0)
        {
            timeObject.SetActive(true);
        }


        if (PlayerPrefs.GetInt("TutorialDay3", 1) == 1 && GetCurrentDay() == 3 && GetCurrentHour() >= 6 && SceneManager.GetActiveScene().name == "ApartmentScene")
        {
            Bed bed = FindFirstObjectByType<Bed>();
            if (bed != null)
            {
                bed.DisableInteraction();
            }
            Day3Tutorial.Instance.areaBlue.SetActive(false);
            ShowFloatingMessage("I should buy a decoration for my house. I can use the laptop in my room to do that.", 4f);
            _playerDataSo.AddCoinsInWallet(50f);
            _playerDataSo.DoPayout();

            PlayerPrefs.SetInt("OpenLaptop", 1);
            PlayerPrefs.SetInt("BuySomething", 1);
            PlayerPrefs.SetInt("TutorialDay3", 0);
            PlayerPrefs.SetInt("NotFoundOrderDay3", 0);
            PlayerPrefs.SetInt("DoneTutorial", 1);
            PlayerPrefs.Save();

            Day3Tutorial.Instance.tutorialTargetObject.SetActive(true);
        }

        elapsedTime += Time.deltaTime;

        float percentOfDay = (elapsedTime % dayDurationRealSeconds) / dayDurationRealSeconds;

        currentGameTime = TimeSpan.FromHours(6 + 24 * percentOfDay);
        if (currentGameTime.TotalHours >= 30)
            currentGameTime = TimeSpan.FromHours(currentGameTime.TotalHours - 24);

        timeDisplayText.text = currentGameTime.ToString(@"hh\:mm");
        dayDisplayText.text = $"Day {currentDay}";

        timeDisplayText2.text = currentGameTime.ToString(@"hh\:mm");
        dayDisplayText2.text = $"Day {currentDay}";

        //// DEMO =========================
        if (GetCurrentDay() > 3 && GetCurrentHour() >= 6 && demoVersion)
        {
            Debug.Log("[Demo] Demo version expired. Please purchase full version.");

            LeanTween.delayedCall(0.2f, () =>
            {
                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    if (scene.name == "MenuScene")
                    {
                        if (PopupDemo != null)
                            PopupDemo.SetActive(true);

                        Debug.Log("Da het demo - Popup hien thi sau khi load MenuScene");
                        SceneManager.sceneLoaded -= null;
                    }
                };

                SceneManager.LoadScene("MenuScene");
            });

            if (objectsToDisable != null)
            {
                foreach (var obj in objectsToDisable)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }

            return;
        }
        ////========================

        if (bankDebtData.currentDebt <= 0 && debtCountdownText != null)
        {
            debtCountdownText.text = "No debts in your account.";
        }
        else
        {
            if (currentDay <= 30)
            {
                int daysLeft = 30 - currentDay;
                int hoursLeftToday = 23 - currentGameTime.Hours;
                int minutesLeftToday = 59 - currentGameTime.Minutes;

                int totalDays = daysLeft;
                int totalHours = hoursLeftToday;
                int totalMinutes = minutesLeftToday;

                if (currentDay == 30)
                {
                    totalDays = 0;
                }

                debtCountdownText.text = $"Payment deadline:\n{totalDays}d {totalHours}h {totalMinutes}m";
            }
        }

        if (currentDay == 30 &&
            currentGameTime.Hours == 23 &&
            currentGameTime.Minutes >= 59)
        {
            if (bankDebtData.currentDebt > 0)
            {
                Debug.LogWarning("[Debt] Game Over - Debt unpaid after 23:59 Day 30.");
                gameOver.ShowGameOver("The weight of this debt was too much for me. I couldn't pay it back, and now I've lost everything I worked for.");
                return;
            }
        }


        int hour = currentGameTime.Hours;
        IsRestingTime = hour >= 0 && hour < 6;

        if (IsRestingTime)
        {
            // Debug.Log("[GameTime] Player is in RESTING TIME (00:00 - 06:00)");

            if (!hasShownRestMessage || Time.time >= nextRestReminderTime)
            {
                ShowFloatingMessage("It's late, I'm off the clock. Time to head home and relax.");
                hasShownRestMessage = true;
                nextRestReminderTime = Time.time + 45f;
            }
        }
        else
        {
            hasShownRestMessage = false;
        }

        if (
            !payoutDoneToday
            && previousGameTime.Hours == 23
            && previousGameTime.Minutes >= 59
            && currentGameTime.Hours == 0
            && currentGameTime.Minutes < 2
        )
        {
            previousDay = currentDay;
            currentDay++;

            if (currentDay > 30)
                currentDay = 1;

            if (currentDay == 1)
            {
                isRentPaid = false;
                violationData.ResetViolations();
                PlayerPrefs.SetInt("RentPaid", 0);
                PlayerPrefs.Save();
                Debug.Log("[GameTime] Rent paid flag reset on Day 1.");

                if (MissionManager.Instance.HasUnfinishedMission() || isShippingForBoss)
                {
                    Debug.LogWarning("[Mission] Game Over - mission not completed in time!");
                    gameOver.ShowGameOver("I failed to pay rent, and now my landlord has kicked me out. My home is gone.");
                    return;
                }
            }

            ProcessPayout(Online: true);
            payoutDoneToday = true;
            Debug.Log("[GameTime] Salary payout triggered at 00:00.");

            SaveManager.SaveDayProgress(elapsedTime, currentDay);
        }

        if (elapsedTime >= dayDurationRealSeconds)
        {
            elapsedTime = 0f;
            payoutDoneToday = false;

            Debug.Log("[GameDay] New day started.");
        }
        if (currentDay == 30 && !isRentPaid)
        {
            isRentDue = true;
            hasShownRentMessage = false;

            if (!isSleeping)
            {
                if (!hasShownRentMessage)
                {
                    hasShownRentMessage = true;
                    StartCoroutine(ShowRentBillPopup());
                    isRentDue = false;
                }
            }
        }

        previousGameTime = currentGameTime;

        if (currentSceneName == "CityScene" && directionalLight != null)
        {
            float timeOfDay = (float)currentGameTime.TotalHours;

            float xRotation = (timeOfDay / 24f) * 360f - 90f;
            directionalLight.transform.rotation = Quaternion.Euler(xRotation, 170f, 0f);

            Color lightColor;
            if (timeOfDay >= 6f && timeOfDay < 12f)
            {
                float lerpT = (timeOfDay - 6f) / 6f;
                lightColor = Color.Lerp(morningColor, noonColor, lerpT);
            }
            else if (timeOfDay >= 12f && timeOfDay < 15f)
            {
                float lerpT = (timeOfDay - 12f) / 3f;
                lightColor = Color.Lerp(noonColor, eveningColor, lerpT);
            }
            else if (timeOfDay >= 15f && timeOfDay < 18f)
            {
                float lerpT = (timeOfDay - 15f) / 3f;
                lightColor = Color.Lerp(eveningColor, nightColorSky, lerpT);
            }
            else
            {
                float lerpT = timeOfDay >= 18f ? (timeOfDay - 18f) / 12f : (timeOfDay + 6f) / 12f;
                lightColor = Color.Lerp(nightColorSky, morningColor, lerpT);
            }
            directionalLight.color = lightColor;

            float intensity;
            if (timeOfDay >= 6f && timeOfDay < 18f)
                intensity = Mathf.Lerp(0.8f, 1.2f, Mathf.Sin((timeOfDay - 6f) / 12f * Mathf.PI));
            else
                intensity = Mathf.Lerp(0.1f, 0.4f, Mathf.Cos((timeOfDay) / 24f * Mathf.PI * 2f));
            directionalLight.intensity = intensity;

            RenderSettings.ambientLight = lightColor;
            RenderSettings.ambientIntensity = intensity;

            if (timeOfDay >= 6f && timeOfDay < 12f && morningSkybox != null)
                RenderSettings.skybox = morningSkybox;
            else if (timeOfDay >= 12f && timeOfDay < 15f && noonSkybox != null)
                RenderSettings.skybox = noonSkybox;
            else if (timeOfDay >= 15f && timeOfDay < 18f && eveningSkybox != null)
                RenderSettings.skybox = eveningSkybox;
            else if (nightSkybox != null)
                RenderSettings.skybox = nightSkybox;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject debtTextObj = GameObject.FindGameObjectWithTag("DebtText");
        if (debtTextObj != null)
        {
            debtCountdownText = debtTextObj.GetComponent<TextMeshProUGUI>();
        }

        currentSceneName = scene.name;

        if (currentSceneName == "CityScene")
            FindDirectionalLight();
        else
            directionalLight = null;
    }

    private void FindDirectionalLight()
    {
        GameObject lightObj = GameObject.FindGameObjectWithTag("Light");
        if (lightObj != null)
        {
            directionalLight = lightObj.GetComponent<Light>();
        }
    }

    #endregion

    #region Fade In/Out Transitions

    public void DoFadeIn(bool disableOnComplete = false)
    {
        _fadeOutOverlay.SetActive(false);
        _fadeInOverlay.SetActive(true);

        if (!disableOnComplete)
            return;

        LeanTween.delayedCall(
            1,
            () =>
            {
                _fadeInOverlay.SetActive(false);
            }
        );
    }

    public void DoFadeOut(bool disableOnComplete = false)
    {
        _fadeOutOverlay.SetActive(true);
        _fadeInOverlay.SetActive(false);

        if (!disableOnComplete)
            return;

        LeanTween.delayedCall(
            1,
            () =>
            {
                _fadeOutOverlay.SetActive(false);
            }
        );
    }

    public void DeactivateFadeTransitions(float delayTime = 0)
    {
        LeanTween.delayedCall(
            delayTime,
            () =>
            {
                _fadeOutOverlay.SetActive(false);
                _fadeInOverlay.SetActive(false);
            }
        );
    }

    #endregion

    #region Notifications

    public void DispalyInteractionUI(string interationMsg)
    {
        if (ActiveScene == Scenes.LoadingScene || OrderManager.Instance.PhoneUIOpen)
            return;

        _interactionText.text = interationMsg;

        _interactionOverlay.SetActive(true);
    }

    public void HideInteractionUI()
    {
        _interactionOverlay.SetActive(false);

        _interactionText.text = string.Empty;
    }

    #endregion

    #region Interaction CrossHair

    public void ActivateInteractionCrossHair()
    {
        if (ActiveScene == Scenes.LoadingScene || OrderManager.Instance.PhoneUIOpen)
            return;

        Canvas canvas = _interactionCrossHair.GetComponentInParent<Canvas>();

        if (TutorialManager.IsTutorial)
        {
            canvas.sortingOrder = 0;
        }
        else
        {
            canvas.sortingOrder = 100;
        }

        _interactionCrossHair.gameObject.SetActive(true);
        OnInteractionExit();
    }

    public void DeactivateInteractionCrossHair()
    {
        _interactionCrossHair.gameObject.SetActive(false);
    }

    public void OnInteractionStart(Color newClr)
    {
        AudioManager.Instance.PlaySFX(AudioId.InteractionStart);

        _interactionCrossHair.gameObject.LeanCancel();
        _interactionCrossHair.color = newClr;
        _interactionCrossHair
            .gameObject.LeanRotateZ(45, 0.5f)
            .setEase(LeanTweenType.easeOutElastic);
        _interactionCrossHair
            .gameObject.LeanScale(Vector3.one, 0.5f)
            .setEase(LeanTweenType.easeOutElastic);
    }

    public void OnInteractionExit()
    {
        _interactionCrossHair.gameObject.LeanCancel();
        _interactionCrossHair.color = _defaultCrossHairColor;
        _interactionCrossHair.gameObject.LeanRotateZ(0, 0.25f).setEase(LeanTweenType.easeInElastic);
        _interactionCrossHair
            .gameObject.LeanScale(new Vector3(0.7f, 0.7f, 0.7f), 0.25f)
            .setEase(LeanTweenType.easeInElastic);
    }

    #endregion

    #region Transitions

    public void LoadScene(Scenes targetScene)
    {
        _sceneTransitionOverlay.gameObject.SetActive(true);

        _sceneTransitionOverlay.LoadGameScene(targetScene);

        PreviousScene = ActiveScene;
        ActiveScene = targetScene;
    }

    #endregion

    #region Messages

    private LTDescr _msgLT;

    public void DisplayMsg(string msg, float msgTimer = -1)
    {
        HideMsg();

        AudioManager.Instance.PlaySFX(AudioId.Msg);

        _msgOverlay.gameObject.SetActive(true);
        _msgOverlay.UpdateMsg(msg);

        if (msgTimer > 0)
        {
            _msgLT = LeanTween.delayedCall(msgTimer, HideMsg);
        }

        UIInputManager.Instance.RegisterKey(Key.Enter, HideAlert);
    }

    public void HideMsg()
    {
        if (_msgLT != null)
        {
            LeanTween.cancel(_msgLT.id);
            _msgLT = null;
        }
        _msgOverlay.gameObject.SetActive(false);
        _msgOverlay.UpdateMsg(string.Empty);

        if (UIInputManager.Instance != null)
            UIInputManager.Instance.UnregisterKey(Key.Enter, HideAlert);
    }

    #endregion

    #region Alert

    private LTDescr _alertLT;

    public void DisplayAlert((string header, string description) alertData, float alertTimer = -1)
    {
        HideAlert();

        AlertUIOpen = true;

        LeanTween.delayedCall(
            0.2f,
            () =>
            {
                AudioManager.Instance.PlaySFX(AudioId.Alert);
            }
        );

        _alertOverlay.gameObject.SetActive(true);
        _alertOverlay.UpdateMsg(alertData.header, alertData.description);

        if (alertTimer > 0)
        {
            _alertLT = LeanTween.delayedCall(alertTimer, HideAlert);
        }

        UIInputManager.Instance.RegisterKey(Key.Enter, HideAlert);
    }

    public void HideAlert()
    {
        if (_alertLT != null)
        {
            LeanTween.cancel(_alertLT.id);
            _alertLT = null;
        }

        AlertUIOpen = false;

        _alertOverlay.gameObject.SetActive(false);
        _alertOverlay.UpdateMsg(string.Empty, string.Empty);

        if (UIInputManager.Instance != null)
            UIInputManager.Instance.UnregisterKey(Key.Enter, HideAlert);
    }

    #endregion

    #region Quit

    public void DisplayQuitUI()
    {
        isTimePaused = true;

        QuitUIOpen = true;
        _quitOverlay.gameObject.SetActive(true);

        Canvas canvas = _quitOverlay.GetComponentInParent<Canvas>();

        if (QuitUIOpen)
        {
            canvas.sortingOrder = 100;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
        StopControllers();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DeactivateInteractionCrossHair();
        HideInteractionUI();
        HideMsg();
    }

    public void HideQuitUI()
    {
        isTimePaused = false;

        QuitUIOpen = false;
        _quitOverlay.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ActivateInteractionCrossHair();

        ResumeControllers();
    }

    #endregion

    #region Player States/Data
    public void LoadData()
    {
        SaveManager.LoadData(ref _playerDataSo);

        AudioManager.Instance.FetchSettingsFromJson();
    }

    public void SaveData()
    {
        SaveManager.SaveData(_playerDataSo);

        OnUserDataUpdate?.Invoke();
    }

    public void UpdateMarketID(MarketID marketID) => MarketEntranceID = marketID;

    public void UpdateActivePlayerTransform(Transform newPlayerTransform)
    {
        GetActivePlayerTransform = newPlayerTransform;

        GetPlayerFollowMiniMapRef.UpdatePlayerTransform(newPlayerTransform);
    }

    public void ResumeControllers()
    {
        if (GetMotoControllerRef != null)
        {
            GetMotoControllerRef.enabled = true;

            if (GetMotoControllerRef.gameObject.activeInHierarchy)
            {
                GetMotoControllerRef.ResumeScooter();
                DeactivateInteractionCrossHair();
            }
        }
        else if (GetThirdPersonControllerRef != null)
        {
            GetThirdPersonControllerRef.enabled = true;

            GetPlayerInteractionControllerRef.ClearLastTarget();

            if (GetThirdPersonControllerRef.gameObject.activeInHierarchy)
                ActivateInteractionCrossHair();
        }
    }

    public void StopControllers(bool disable = false)
    {
        if (GetMotoControllerRef != null)
        {
            GetMotoControllerRef.StopScooter();
            GetMotoControllerRef.enabled = false;

            if (disable)
                GetMotoControllerRef.gameObject.SetActive(false);
        }

        if (GetThirdPersonControllerRef != null)
        {
            GetThirdPersonControllerRef.StopPlayer();
            GetThirdPersonControllerRef.enabled = false;

            if (disable)
                GetThirdPersonControllerRef.gameObject.SetActive(false);
        }
    }

    public void CheckAndSchedulePayout()
    {
        //string todayStr = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        //DateTime todayDate = DateTime.Now.Date;

        //string lastPayoutStr = GetPlayerDataSo.GetLastPayoutDate;
        //DateTime lastPayoutDate;

        //bool firstTime = string.IsNullOrEmpty(lastPayoutStr);

        //if (!firstTime && DateTime.TryParseExact(lastPayoutStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastPayoutDate))
        //{
        //    if (lastPayoutDate > todayDate)
        //    {
        //        Debug.Log("[MidnightReward] Future last reward date detected. Resetting to today.");
        //        GetPlayerDataSo.UpdateLastPayout(todayStr);
        //    }
        //    else if (lastPayoutDate < todayDate)
        //    {
        //        ProcessPayout(Online: false);
        //    }
        //}
        //else if (firstTime)
        //{
        //    Debug.Log("[MidnightReward] First run — reward skipped, date recorded.");
        //    GetPlayerDataSo.UpdateLastPayout(todayStr);
        //}

        //DateTime now = DateTime.Now;
        //DateTime nextMidnight = now.Date.AddDays(1);

        //float remainingSeconds = (float)((nextMidnight - now).TotalSeconds);

        //LeanTween.delayedCall(gameObject, remainingSeconds, () =>
        //{
        //    ProcessPayout(Online: true);
        //});

        //Debug.Log($"[MidnightReward] Next reward scheduled for {nextMidnight} in {remainingSeconds} seconds.");

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long lastPayout = GetPlayerDataSo.GetLastPayoutTimestamp;

        const int payoutIntervalSeconds = 15 * 60;

        bool firstTime = lastPayout == 0;
        bool isDue = now - lastPayout >= payoutIntervalSeconds;

        if (firstTime)
        {
            Debug.Log("[MidnightReward] First run — timestamp set.");
            GetPlayerDataSo.UpdateLastPayoutTimestamp(now);
        }
        else if (isDue)
        {
            Debug.Log("[MidnightReward] Payout due — processing now.");
            ProcessPayout(Online: false);
        }

        long nextPayoutTime = (firstTime ? now : lastPayout) + payoutIntervalSeconds;
        float delaySeconds = Mathf.Max(0f, nextPayoutTime - now);

        LeanTween.delayedCall(
            gameObject,
            delaySeconds,
            () =>
            {
                ProcessPayout(Online: true);
            }
        );

        Debug.Log($"[MidnightReward] Next payout scheduled in {delaySeconds} seconds.");
    }

    private void ProcessPayout(bool Online)
    {
        //float walletCoins = GetPlayerDataSo.GetWalletCoins;
        //string todayStr = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        //GetPlayerDataSo.UpdateLastPayout(todayStr, saveData: false);
        //GetPlayerDataSo.DoPayout(saveData: true);

        //if (walletCoins > 0)
        //{

        //    if (Online)
        //    {
        //        DisplayAlert(GameStrings.GetMidnightEarningsAlert(walletCoins), 3);

        //        LeanTween.delayedCall(3, () =>
        //        {
        //            DisplayMsg(GameStrings.GetMidnightEarningsMsg(walletCoins), 5);
        //        });
        //    }
        //    else
        //    {
        //        DisplayAlert(GameStrings.GetAwayEarningsAlert(walletCoins), 3);

        //        LeanTween.delayedCall(3, () =>
        //        {
        //            DisplayMsg(GameStrings.GetAwayEarningsMsg(walletCoins), 5);
        //        });
        //    }
        //}
        //else
        //{
        //    DisplayAlert(GameStrings.GetNoEarningsAlert(), 3);

        //    LeanTween.delayedCall(3, () =>
        //    {
        //        DisplayMsg(GameStrings.GetNoEarningsMsg(), 5);
        //    });
        //}

        //Debug.Log("[MidnightReward] Payout processed.");

        float walletCoins = GetPlayerDataSo.GetWalletCoins;
        GetPlayerDataSo.DoPayout(saveData: true);

        if (walletCoins > 0)
        {
            if (Online)
            {
                DisplayAlert(GameStrings.GetMidnightEarningsAlert(walletCoins), 3);
                LeanTween.delayedCall(
                    3,
                    () =>
                    {
                        DisplayMsg(GameStrings.GetMidnightEarningsMsg(walletCoins), 5);
                    }
                );
            }
        }
        else
        {
            DisplayAlert(GameStrings.GetNoEarningsAlert(), 3);
            LeanTween.delayedCall(
                3,
                () =>
                {
                    DisplayMsg(GameStrings.GetNoEarningsMsg(), 5);
                }
            );
        }

        PlayerPrefs.SetInt("GoHomeDay2", 1);
        PlayerPrefs.Save();

        OrderManager.Instance.ClearAllOrders();

        Debug.Log("[GameDay] Payout processed at midnight.");
        if (PlayerPrefs.GetInt("DoneTutorial", 0) == 0 && PlayerPrefs.GetInt("GoHomeDay2") == 1)
        {
            //DonutShopEvents.Hide();
            //GasStationEvents.Hide();
            OrderManager.Instance.ClearAllOrders();
            OrderManager.Instance.ForceClearAllOrders();
            MissionManager.Instance.HideMarketPortalIcon();
            PlayerPrefs.SetInt("NotFoundOrderDay3", 1);
            PlayerPrefs.SetInt("GoHomeDay2", 0);
            PlayerPrefs.Save();
        }
    }

    public void StartTutorial() => InTutorial = true;

    public void StopTutorial() => InTutorial = false;

    #endregion

    #region Sleep
    public void SleepAndSkipDay()
    {
        PlayerPrefs.SetInt("SleepSkipDay1Tutorial", 1);
        PlayerPrefs.Save();

        isTimePaused = true;
        isSleeping = true;

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;

        LeanTween
            .alphaCanvas(fadeCanvasGroup, 1f, 2f)
            .setOnComplete(() =>
            {
                OrderManager.Instance.ClearAllOrders();
                OrderManager.Instance.ForceClearAllOrders();

                if (!IsRestingTime && PlayerPrefs.GetInt("SleepSkipDay1Tutorial", 1) == 1)
                {
                    elapsedTime = 0f;
                    currentDay++;

                    if (currentDay > 30)
                    { 
                        currentDay = 1;
                        violationData.ResetViolations();
                    }

                    payoutDoneToday = false;

                    PlayerPrefs.SetInt("SleepSkipDay1Tutorial", 0);
                    PlayerPrefs.Save();

                    Debug.Log($"[Sleep] [Tutorial] Auto skipped to Day {currentDay} - 06:00 AM");
                }
                else if (IsRestingTime && PlayerPrefs.GetInt("Tutorial1") == 0)
                {
                    float targetSeconds = 6 * 3600f; 
                    if (elapsedTime < targetSeconds) 
                        elapsedTime = targetSeconds; 
                    else 
                        elapsedTime = targetSeconds;

                    Debug.Log($"[Sleep] Slept and woke up at Day {currentDay} - 06:00 AM (same day)");
                }
                else
                {
                elapsedTime = 0f;
                currentDay++;

                if (currentDay > 30)
                {
                    currentDay = 1;
                    violationData.ResetViolations();
                }

                payoutDoneToday = false;
                
                PlayerPrefs.SetInt("SleepSkipDay1Tutorial", 0);
                PlayerPrefs.Save();

                Debug.Log($"[Sleep] Slept and woke up at Day {currentDay} - 06:00 AM");
                }

                SaveManager.SaveDayProgress(elapsedTime, currentDay);

                LeanTween.delayedCall(1f, () =>
                {
                    int rand = UnityEngine.Random.Range(0, 2);
                    if (rand == 0)
                    {
                        AudioManager.Instance.PlaySFX(AudioId.AlarmClock);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX(AudioId.Rooster);
                    }
                });

                if (HungerSystem.Instance.currentHunger > 20f)
                    HungerSystem.Instance.ReduceHunger(10f);

                LeanTween.delayedCall(4.2f, () =>
                {
                    LeanTween
                        .alphaCanvas(fadeCanvasGroup, 0f, 1f)
                        .setOnComplete(() =>
                        {
                            fadeCanvasGroup.gameObject.SetActive(false);
                            isTimePaused = false;
                            isSleeping = false;
                            HungerSystem.Instance.SkipNextHourCheck();

                            if (GetCurrentDay() > 3 && demoVersion)
                            {
                                LeanTween.delayedCall(0.2f, () =>
                                {
                                    SceneManager.sceneLoaded += (scene, mode) =>
                                    {
                                        if (scene.name == "MenuScene")
                                        {
                                            if (PopupDemo != null)
                                                PopupDemo.SetActive(true);

                                            Debug.Log("Da het demo - Popup hien thi sau khi load MenuScene");
                                            SceneManager.sceneLoaded -= null; 
                                        }
                                    };

                                    SceneManager.LoadScene("MenuScene");
                                });

                                if (objectsToDisable != null)
                                {
                                    foreach (var obj in objectsToDisable)
                                    {
                                        if (obj != null)
                                            obj.SetActive(false);
                                    }
                                }

                                return;
                            }


                            if (PlayerPrefs.GetInt("TutorialDay2", 1) == 1)
                            {
                                showTutorialDay2?.Invoke();
                            }

                            //if (PlayerPrefs.GetInt("TutorialDay3", 1) == 1 && isShowDay3 == false)
                            //{
                            //    isShowDay3 = true;
                            //    showTutorialDay3?.Invoke();
                            //}
                        });
                });
            });
    }

    public void ShowFloatingMessage(string text, float duration = 2f, Action onComplete = null)
    {
        if (GameOver.Instance.isShowing)
            return;

        if (activeMessages.Contains(text))
            return;

        messageQueue.Enqueue((text, duration, onComplete));
        activeMessages.Add(text);

        if (!isShowingMessage)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowingMessage = true;

        while (messageQueue.Count > 0)
        {
            var (text, duration, onComplete) = messageQueue.Dequeue();

            messageText.text = text;
            messageText.gameObject.SetActive(true);

            RectTransform rt = messageText.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -200f);
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);

            LeanTween.moveY(rt, -348f, 1.5f).setEaseOutCubic();

            bool done = false;

            LeanTween
                .alphaText(rt, 0f, duration)
                .setDelay(1f)
                .setOnComplete(() =>
                {
                    messageText.gameObject.SetActive(false);
                    messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);
                    onComplete?.Invoke();
                    done = true;

                    activeMessages.Remove(text);
                });

            yield return new WaitUntil(() => done);
        }

        isShowingMessage = false;
    }
    #endregion

    #region Rent Bill

    private IEnumerator ShowRentBillPopup()
    {
        if (GameOver.Instance.isShowing)
            yield break;

        bed = FindFirstObjectByType<Bed>();

        hungerSystem.isHungerPaused = true;
        isShippingForBoss = true;

        PlayerPrefs.SetInt("isShippingForBoss", 1);
        PlayerPrefs.Save();

        isRentPaid = true;
        isRentDue = false;
        hasShownRentMessage = true;

        while (SceneManager.GetActiveScene().name == "LoadingScene" || sceneTransitionOverlay.activeInHierarchy || SceneManager.GetActiveScene().name == "MenuScene")
        {
            yield return null;
        }

        if (billPopupCanvasGroup == null)
        {
            Debug.LogError("billPopupCanvasGroup is null!");
            yield break;
        }

        billPopupCanvasGroup.alpha = 0f;
        billPopupCanvasGroup.gameObject.SetActive(true);

        RectTransform rt = billPopupCanvasGroup.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 300f);

        bool animationDone = false;

        LeanTween.moveY(rt, 0f, 0.8f).setEaseOutBack();
        LeanTween
            .alphaCanvas(billPopupCanvasGroup, 1f, 0.8f)
            .setOnComplete(() => animationDone = true);

        StopControllers();
        DeactivateInteractionCrossHair();
        bed.DisableInteraction();

        yield return new WaitUntil(() => animationDone);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        if (TryPayRent())
        {
            Debug.Log("[Rent] Rent payment successful, hiding popup.");
            // Hide with animation
            animationDone = false;
            LeanTween.moveY(rt, 300f, 0.5f).setEaseInBack();
            LeanTween
                .alphaCanvas(billPopupCanvasGroup, 0f, 0.5f)
                .setOnComplete(() =>
                {
                    bed.EnableInteraction();
                    ActivateInteractionCrossHair();
                    ResumeControllers();
                    isShippingForBoss = false;

                    PlayerPrefs.SetInt("isShippingForBoss", 0); 
                    PlayerPrefs.Save();

                    AudioManager.Instance.PlaySFX(AudioId.Bill);

                    billPopupCanvasGroup.gameObject.SetActive(false);
                    animationDone = true;
                });
            yield return new WaitUntil(() => animationDone);
        }
        else
        {
            bed.EnableInteraction();
            ActivateInteractionCrossHair();
            ResumeControllers();
            billPopupCanvasGroup.gameObject.SetActive(false);
            OnRentDueFail();
        }

        StartCoroutine(ShippingWarningRoutine());
    }

    private void OnRentDueFail()
    {
        _waitingMissionAfterCityLoad = true;
        SceneManager.sceneLoaded += HandleSceneLoaded_StartMission;
        LeanTween.delayedCall(0.1f, () => LoadScene(Scenes.CityScene));
    }

    private void HandleSceneLoaded_StartMission(Scene scene, LoadSceneMode mode)
    {
        if (!string.Equals(scene.name, "CityScene"))
            return;

        SceneManager.sceneLoaded -= HandleSceneLoaded_StartMission;
        _waitingMissionAfterCityLoad = false;

        var mm = MissionManager.Instance ?? FindObjectOfType<MissionManager>();
        if (mm != null)
        {
            mm.StartMission();
        }
        else
        {
            Debug.LogWarning("[Mission] MissionManager not found after CityScene load.");
        }
    }

    private bool TryPayRent()
    {
        var pdata = GameManager.Instance?.GetPlayerDataSo;
        if (pdata == null)
        {
            Debug.LogError("[Rent] PlayerDataSo is null!");
            return false;
        }

        // Nếu là property
        float balance = pdata.GetTotalBalanceCoins;

        if (balance >= 1000f)
        {
            pdata.RemovePlayerCoins(1000f);

            isRentPaid = true;
            isRentDue = false;

            PlayerPrefs.SetInt("RentPaid", 1);
            PlayerPrefs.Save();
            return true;
        }
        Debug.Log("[Rent] Not enough coins to pay rent. Current balance: " + balance);

        // Không đủ
        isRentPaid = true;
        isRentDue = false;

        PlayerPrefs.SetInt("RentPaid", 1);
        PlayerPrefs.Save();
        return false;
    }

    private IEnumerator ShippingWarningRoutine()
    {
        while (true)
        {
            if (!isShippingForBoss)
                yield break; 

            yield return new WaitForSeconds(70f);

            if (SceneManager.GetActiveScene().name != "LoadingScene"
                && !sceneTransitionOverlay.activeInHierarchy && SceneManager.GetActiveScene().name != "MenuScene")
            {
                ShowFloatingMessage(
                    "I'd better deliver these orders to the landlord right away, or I'm going to get kicked out.",
                    4f
                );
            }
        }
    }

    #endregion

    public void RegisterTutorial(ITutorial tutorial)
    {
        activeTutorial = tutorial;
    }

    public bool IsTutorialActive()
    {
        return activeTutorial != null && activeTutorial.IsTutorial;
    }

    public void Init()
    {
        LeanTween.delayedCall(0.2f, () => LoadScene(Scenes.ApartmentScene));

        GameSetup();

        LoadData();

        DayProgressData data = SaveManager.LoadDayProgress();
        elapsedTime = data.ElapsedTime;
        currentDay = data.CurrentDay;
    }

    public void AutoSave()
    {
        SaveManager.SaveDayProgress(elapsedTime, currentDay);
        SaveData();
    }

    private void GameSetup()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        _defaultCrossHairColor = _interactionCrossHair.color;
    }

    public int GetCurrentHour()
    {
        return currentGameTime.Hours;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    private void OnApplicationQuit()
    {
        AutoSave();
        Debug.Log("Save");
    }

    public void StartNewGame()
    {
        LeanTween.delayedCall(0.2f, () => LoadScene(Scenes.ApartmentScene));

        GameSetup();

        LoadData();

        DayProgressData data = SaveManager.LoadDayProgress();
        elapsedTime = data.ElapsedTime;
        currentDay = data.CurrentDay;
    }

    public void ContinueGame()
    {
        LeanTween.delayedCall(0.2f, () => LoadScene(Scenes.ApartmentScene));

        GameSetup();

        LoadData();

        DayProgressData data = SaveManager.LoadDayProgress();
        elapsedTime = data.ElapsedTime;
        currentDay = data.CurrentDay;
    }

    private float GetElapsedTime(int hour, int minute)
    {
        float unitPerHour = 1200f / 24f; 
        float unitPerMinute = unitPerHour / 60f; 
        return hour * unitPerHour + minute * unitPerMinute;
    }


    [ContextMenu("Reset Time")]
    public void ResetTime()
    {
        SaveManager.DeleteDayProgress();
    }

    [ContextMenu("Reset Data")]
    public void ResetData()
    {
        PlayerPrefs.SetInt("RentPaid", 0);
        PlayerPrefs.Save();
        SaveManager.DeleteSave();
    }

    [ContextMenu("Delete Mission Ship for boss")]
    public void DeleteMission()
    {
        PlayerPrefs.DeleteKey("isShippingForBoss");
        PlayerPrefs.Save();
    }

    [ContextMenu("Delete Keys")]
    public void DeleteKeys()
    {
        ResetTime();
        ResetData();
        DeleteMission();
        HungerSystem.Instance.ResestHunger();
        GasSystem.Instance.ResetGas();

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

    [ContextMenu("Skip Tutorial")]
    public void SkipTutorial()
    {
        TutorialManager.SetTutorialState(false);
        currentDay = 4;
        elapsedTime = 0f;
        payoutDoneToday = false;
        SaveManager.SaveDayProgress(elapsedTime, currentDay);
        SaveManager.SaveData(_playerDataSo);
        DeleteMission();

        PlayerPrefs.SetInt("TutorialDay1", 0);
        PlayerPrefs.SetInt("CityScenePopup", 0);
        PlayerPrefs.SetInt("Tutorial1", 0);
        PlayerPrefs.DeleteKey("OrderSuccessful");
        PlayerPrefs.SetInt("HasShownOrderReceived", 1);
        PlayerPrefs.SetInt("EnterSuperMarket", 1);
        PlayerPrefs.SetInt("CompleteShipping", 1);
        PlayerPrefs.SetInt("AteComplete", 1);
        PlayerPrefs.SetInt("GasComplete", 1);
        PlayerPrefs.SetInt("HasReceivedOrderToday", 0);
        PlayerPrefs.SetInt("SkipSleep", 1);
        PlayerPrefs.SetInt("TutorialDay2", 0);
        PlayerPrefs.SetInt("DoneTutorial", 1);
        PlayerPrefs.SetInt("TutorialDay3", 0);
        PlayerPrefs.SetInt("OrderOne", 0);
        PlayerPrefs.SetInt("NotFoundOrderDay3", 0);
        PlayerPrefs.SetInt("BuySomething", 0);
        PlayerPrefs.SetInt("OpenLaptop", 0);
        PlayerPrefs.SetInt("GoHomeDay1", 0);
        PlayerPrefs.SetInt("HasOrder", 0);
        PlayerPrefs.SetInt("HidePortal", 0);
        PlayerPrefs.SetInt("SleepSkipDay1Tutorial", 0);
        PlayerPrefs.SetInt("GoHomeDay2", 0);
        PlayerPrefs.SetInt("Pickup", 0);
        PlayerPrefs.SetInt("ShippingComplete", 1);
        PlayerPrefs.Save();

        LeanTween.delayedCall(1f, () => LoadScene(Scenes.ApartmentScene));
    }

    [ContextMenu("Delete Audio PlayerPrefs")]
    private void DeleteAudioPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.DeleteKey("MusicVolume");

        if (PlayerPrefs.HasKey("SfxVolume"))
            PlayerPrefs.DeleteKey("SfxVolume");

        PlayerPrefs.Save();
    }

}
