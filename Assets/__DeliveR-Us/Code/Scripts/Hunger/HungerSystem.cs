using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HungerSystem : MonoBehaviour
{
    [Header("Hunger Settings")]
    [SerializeField]
    private float maxHunger = 100f;
    public PlayerDataSO playerDataSO;
    public BankDebtData bankDebtData;

    [SerializeField]
    private float decreaseRatePerGameHour = 5f;

    [SerializeField]
    private Slider hungerSlider;

    public float currentHunger;
    private int previousHour;
    private const string HUNGER_KEY = "PlayerHunger";
    private const string LAST_HOUR_KEY = "PlayerLastHour";

    private bool isFirstLoad = true;
    public bool isHungerPaused = false;
    private bool skipNextHourCheck = false;

    [Header("Fainting")]
    [SerializeField]
    private CanvasGroup fadeCanvasGroup;

    [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField]
    private Vector3 playerFaintPosition;

    [SerializeField]
    private Vector3 playerFaintRotationEuler;

    [SerializeField]
    private Vector3 motoFaintPosition;

    [SerializeField]
    private Vector3 motoFaintRotationEuler;

    [Header("Bill Popup")]
    [SerializeField]
    private CanvasGroup billPopupCanvasGroup;

    [SerializeField] private GameObject hungerTutorial;
    [SerializeField] private TextMeshProUGUI hungerText;

    private bool isFainting = false;
    private Coroutine hungerWarningCoroutine;
    //private bool isWarningActive = false;

    public GameObject sceneTransitionOverlay;

    public static HungerSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Day1Tutorial.OnToEat += ShowAteTutorial;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //Day1Tutorial.OnToEat -= ShowAteTutorial;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindHungerSlider();
    }

    public void FindHungerSlider()
    {
        if (hungerSlider == null)
        {
            GameObject sliderObj = GameObject.FindGameObjectWithTag("HungerSlider");
            if (sliderObj != null)
            {
                hungerSlider = sliderObj.GetComponent<Slider>();
            }
        }
        UpdateUI();
    }

    private void Start()
    {
        if (currentHunger <= 0)
        {
            currentHunger = maxHunger;
        }

        FindHungerSlider();
        LoadHunger();

        previousHour = GameManager.Instance.GetCurrentHour();

        UpdateUI();

        fadeCanvasGroup.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        billPopupCanvasGroup.gameObject.SetActive(false);
        hungerTutorial.SetActive(false);

        hungerWarningCoroutine = StartCoroutine(HungerWarningRoutine());
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "LoadingScene" || SceneManager.GetActiveScene().name == "SuperMarketScene" || sceneTransitionOverlay.activeInHierarchy || SceneManager.GetActiveScene().name == "MenuScene")
            return;

        if (isHungerPaused || GameManager.Instance.isSleeping)
            return;

        if (GameManager.Instance.demoVersion && currentHunger <= 30f)
        {
            return;
        }

        int currentHour = GameManager.Instance.GetCurrentHour();

        if (currentHour != previousHour)
        {
            if (skipNextHourCheck)
            {
                skipNextHourCheck = false;
                previousHour = currentHour;
                return;
            }

            if (isFirstLoad)
            {
                isFirstLoad = false;
                previousHour = currentHour;
                return;
            }

            int hoursPassed = currentHour - previousHour;
            if (hoursPassed < 0)
                hoursPassed += 24;

            if (GameManager.Instance.isSleeping) return;

            if (GameManager.Instance.demoVersion && currentHunger <= 30f)
            {
                return;
            }

            float hungerLost = hoursPassed * decreaseRatePerGameHour;
            currentHunger -= hungerLost;
            currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

            UpdateUI();
            SaveHunger();

            previousHour = currentHour;

            if (currentHunger <= 0f)
            {
                OnStarving();
            }
        }
    }

    void UpdateUI()
    {
        if (hungerSlider != null)
            hungerSlider.value = currentHunger / maxHunger;
    }

    void OnStarving()
    {
        if (isFainting)
            return;

        Debug.LogWarning("[HungerSystem] Player is starving and fainted!");
        isFainting = true;

        GameManager.Instance.ShowFloatingMessage("I'm getting dizzy from hunger. I can't... I'm fainting.");

        StartCoroutine(HandleFainting());
    }

    private IEnumerator HungerWarningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(35f);

            if (
                currentHunger > 0f
                && currentHunger < 30f
                && SceneManager.GetActiveScene().name != "LoadingScene"
                && !sceneTransitionOverlay.activeInHierarchy
                && SceneManager.GetActiveScene().name != "MenuScene"
                )
            {
                GameManager.Instance.ShowFloatingMessage("I'm feeling very hungry...");
            }
        }
    }

    private IEnumerator HandleFainting()
    {
        if (GameOver.Instance.isShowing)
            yield break;

        isFainting = true;

        yield return new WaitForSeconds(3.5f);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = true;
            fadeCanvasGroup.interactable = false;

            bool fadeInDone = false;

            LeanTween
                .value(fadeCanvasGroup.gameObject, 0f, 1f, 1f)
                .setOnUpdate((float val) => fadeCanvasGroup.alpha = val)
                .setOnComplete(() => fadeInDone = true);

            yield return new WaitUntil(() => fadeInDone);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene((int)Scenes.ApartmentScene);

        yield return new WaitForSeconds(0.5f);

        if (fadeCanvasGroup != null)
        {
            AddHunger(50f);

            fadeCanvasGroup.alpha = 1f;

            bool fadeOutDone = false;

            LeanTween
                .value(fadeCanvasGroup.gameObject, 1f, 0f, 1f)
                .setOnUpdate((float val) => fadeCanvasGroup.alpha = val)
                .setOnComplete(() =>
                {
                    fadeCanvasGroup.gameObject.SetActive(false);
                    fadeOutDone = true;
                });

            yield return new WaitUntil(() => fadeOutDone);
        }

        isFainting = false;

        yield return StartCoroutine(ShowBillPopup());
    }

    //public void ShowFloatingMessage(string text, Action onComplete = null)
    //{
    //    if (GameOver.Instance.isShowing)
    //        return;

    //    messageText.text = text;
    //    messageText.color = new Color(
    //        messageText.color.r,
    //        messageText.color.g,
    //        messageText.color.b,
    //        1f
    //    );
    //    messageText.gameObject.SetActive(true);
    //    messageText.transform.SetAsLastSibling();

    //    RectTransform rt = messageText.GetComponent<RectTransform>();
    //    rt.anchoredPosition = new Vector2(0, 0f);

    //    LeanTween.moveY(rt, -348f, 1.5f).setEaseOutCubic();
    //    LeanTween
    //        .value(messageText.gameObject, 1f, 0f, 1.5f)
    //        .setDelay(3f)
    //        .setOnUpdate(
    //            (float val) =>
    //            {
    //                Color c = messageText.color;
    //                c.a = val;
    //                messageText.color = c;
    //            }
    //        )
    //        .setOnComplete(() =>
    //        {
    //            messageText.gameObject.SetActive(false);
    //            Color c = messageText.color;
    //            c.a = 1f;
    //            messageText.color = c;
    //            onComplete?.Invoke();
    //        });
    //}

    private IEnumerator ShowBillPopup()
    {
        while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LoadingScene" || sceneTransitionOverlay.activeInHierarchy || SceneManager.GetActiveScene().name == "MenuScene")
        {
            yield return null;
        }

        float cost = 50f;
        float currentCoins = playerDataSO.GetTotalBalanceCoins;
        int today = GameManager.Instance.GetCurrentDay();

        billPopupCanvasGroup.alpha = 0f;
        billPopupCanvasGroup.gameObject.SetActive(true);

        GameManager.Instance.StopControllers();
        GameManager.Instance.DeactivateInteractionCrossHair();

        RectTransform rt = billPopupCanvasGroup.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(0, 300f);

        bool animationDone = false;

        LeanTween.moveY(rt, 0f, 0.8f).setEaseOutBack();
        LeanTween
            .alphaCanvas(billPopupCanvasGroup, 1f, 0.8f)
            .setOnComplete(() => animationDone = true);

        yield return new WaitUntil(() => animationDone);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        animationDone = false;

        LeanTween.moveY(rt, 300f, 0.5f).setEaseInBack();
        LeanTween
            .alphaCanvas(billPopupCanvasGroup, 0f, 0.5f)
            .setOnComplete(() =>
            {
                if (currentCoins >= cost)
                {
                    GameManager.Instance.GetPlayerDataSo.RemovePlayerCoins(cost);
                }
                else
                {
                    bankDebtData.AddDebt(cost, today);
                    GameManager.Instance.ShowFloatingMessage("I'm safe, but I have a rescue fee to pay. Since I'm broke, it’s now a debt with the bank.", 4f);
                }
                billPopupCanvasGroup.gameObject.SetActive(false);
                animationDone = true;
                AudioManager.Instance.PlaySFX(AudioId.Bill);
                GameManager.Instance.ResumeControllers();
                GameManager.Instance.ActivateInteractionCrossHair();
            });

        yield return new WaitUntil(() => animationDone);
    }

    public void AddHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
        UpdateUI();
        SaveHunger();
    }

    public void ReduceHunger(float amount)
    {
        if (GameManager.Instance.demoVersion && currentHunger <= 30)
        {
            return;
        }

        currentHunger -= amount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
        UpdateUI();
        SaveHunger();

        if (currentHunger <= 0f)
        {
            OnStarving();
        }
    }

    public bool IsStarving()
    {
        return currentHunger <= 0f;
    }

    public float GetHungerPercent()
    {
        return currentHunger / maxHunger;
    }

    private void OnDestroy()
    {
        SaveHunger();
    }

    private void OnApplicationQuit()
    {
        SaveHunger();
    }

    public void SaveHunger()
    {
        PlayerPrefs.SetFloat(HUNGER_KEY, currentHunger);
        PlayerPrefs.SetInt(LAST_HOUR_KEY, GameManager.Instance.GetCurrentHour());
        PlayerPrefs.Save();
    }

    private void LoadHunger()
    {
        currentHunger = PlayerPrefs.GetFloat(HUNGER_KEY, maxHunger);
        UpdateUI();
    }

    //public bool IsHungerPaused
    //{
    //    get => isHungerPaused;
    //    set => isHungerPaused = value;
    //}

    public void SkipNextHourCheck()
    {
        skipNextHourCheck = true;
    }

    public void ShowAteTutorial()
    {
        if (PlayerPrefs.GetInt("AteComplete", 0) == 1)
            return;

        hungerTutorial.SetActive(true);
        hungerText.text = "Let's go to the restaurant and enjoy our meal!";
        PlayerPrefs.SetInt("AteComplete", 0);
        PlayerPrefs.Save();
    }

    public void CompleteAteTutorial()
    {
        hungerTutorial.SetActive(false);
        GasStationEvents.Show();
        PlayerPrefs.SetInt("AteComplete", 1);
        PlayerPrefs.Save();
    }

    public void GotoDonutShop()
    {
        GameManager.Instance.ShowFloatingMessage("I'm getting pretty hungry. Maybe I should go get something to eat...", 3f);

        if (PlayerPrefs.GetInt("Tutorial1") == 1)
        {
            ShowAteTutorial();
        }
    }

    [ContextMenu("Hunger Delelte")]
    public void ResestHunger(bool saveData = true)
    {
        currentHunger = maxHunger;
        UpdateUI();
        if (saveData)
            SaveHunger();
    }
}
