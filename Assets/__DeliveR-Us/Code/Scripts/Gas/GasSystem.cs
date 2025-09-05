using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GasSystem : MonoBehaviour
{
    public static GasSystem Instance { get; private set; }

    [Header("Gas Settings")]
    [SerializeField]
    private float MaxGas = 10f;

    [SerializeField]
    private float currentGas;
    private const string GAS_KEY = "PlayerGas";

    [SerializeField]
    private Slider FuelSlider;

    [Header("Pricing")]
    [SerializeField]
    private float pricePerLiter = 2.0f;

    private bool isFirstLoad = true;
    private bool isOutOfGas = false;

    [Header("Confirm Panel")]
    [SerializeField]
    private CanvasGroup confirmPanel;

    [SerializeField]
    private TMP_Text confirmText;

    [SerializeField]
    private Button confirmYesButton;

    [SerializeField]
    private Button confirmNoButton;

    [Header("Purchased Panel")]
    [SerializeField]
    private CanvasGroup purchasedPanel;

    [SerializeField]
    private TextMeshProUGUI purchasedText;

    [SerializeField]
    private Button purchasedOkButton;

    // Thêm field trong class GasSystem
    private Coroutine _purchasedRoutine;

    private GasTrigger gasTrigger;

    public GameObject gasObject;
    public TextMeshProUGUI gasText;

    public static event Action GasComplete;

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
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindGasSlider();
        if (gasTrigger == null)
        {
            gasTrigger = FindFirstObjectByType<GasTrigger>();
        }
    }

    private void Start()
    {
        FindGasSlider();
        LoadGas();
    }

    private void Update() { }

    [ContextMenu("Find Gas Slider")]
    public void FindGasSlider()
    {
        if (FuelSlider == null)
        {
            GameObject Slider = GameObject.FindGameObjectWithTag("GasSlider");
            if (Slider != null)
            {
                FuelSlider = Slider.GetComponent<Slider>();
            }

            UpdateUI();
        }
    }

    public void RefillGas(float amount)
    {
        currentGas += amount;
        UpdateUI();
        SaveGas();
    }

    public void ConsumeGas(float rate)
    {
        currentGas -= rate * Time.deltaTime;
        currentGas = Mathf.Clamp(currentGas, 0f, MaxGas);
        UpdateUI();
        SaveGas();
        if (currentGas <= 0f)
        {
            IsOutOfGas();
        }
    }

    public bool IsOutOfGas()
    {
        return currentGas <= 0f;
    }

    public float GetGasPercent()
    {
        return currentGas / MaxGas;
    }

    /// <summary> Số lít cần để đổ đầy bình ở thời điểm gọi. </summary>
    public float GetLitersToFull()
    {
        return Mathf.Max(0f, MaxGas - currentGas);
    }

    /// <summary> Tiền cần để đổ đầy bình ngay bây giờ (làm tròn 2 số thập phân). </summary>
    public float CalculateRefuelCostToFull()
    {
        float liters = GetLitersToFull();
        return Round2(liters * pricePerLiter);
    }

    /// <summary> Làm tròn 2 chữ số thập phân cho hiển thị/thu phí. </summary>
    private float Round2(float v)
    {
        return Mathf.Round(v * 100f) / 100f;
    }

    public void GasStation(GasTrigger trigger)
    {
        gasTrigger = trigger;
        if (isOutOfGas)
        {
            ShowPurchasedMessage("You are out of gas. Please refuel at a gas station.");
            return;
        }

        float litersToFull = GetLitersToFull();
        float totalCost = CalculateRefuelCostToFull();

        ShowConfirm(litersToFull, totalCost);
    }

    private void ShowConfirm(float liters, float total)
    {
        //if (!ValidateConfirmRefs())
        //    return;

        //confirmText.text =
        //    $"Refuel <b>{liters:0.#}L</b> at $ {pricePerLiter:0.##}/L?\n"
        //    + $"Total: <color=red> $ {total:0.##}</color>";

        //// clear old listeners
        //confirmYesButton.onClick.RemoveAllListeners();
        //confirmNoButton.onClick.RemoveAllListeners();

        //confirmYesButton.onClick.AddListener(() =>
        //{
        //    OnConfirmRefuel(liters, total);
        //    HideCanvasGroup(confirmPanel);
        //});

        //confirmNoButton.onClick.AddListener(() =>
        //{
        //    HideCanvasGroup(confirmPanel);
        //});
        //GameManager.Instance.StopControllers();

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        //GameManager.Instance.DeactivateInteractionCrossHair();
        //ShowCanvasGroup(confirmPanel);

        if (!ValidateConfirmRefs())
            return;

        confirmText.text =
            $"Refuel <b>{liters:0.#}L</b> at $ {pricePerLiter:0.##}/L?\n"
            + $"Total: <color=red> $ {total:0.##}</color>";

        GameManager.Instance.StopControllers();
        GameManager.Instance.DeactivateInteractionCrossHair();
        ShowCanvasGroup(confirmPanel);

        LeanTween.value(gameObject, 0, 1, 0.1f) 
            .setLoopClamp()
            .setOnUpdate((float val) =>
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    AudioManager.Instance.PlaySFX(AudioId.Bill);
                    OnConfirmRefuel(liters, total);
                    HideCanvasGroup(confirmPanel);
                    LeanTween.cancel(gameObject);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    HideCanvasGroup(confirmPanel);
                    LeanTween.cancel(gameObject);
                }
            });
    }

    private void OnConfirmRefuel(float liters, float total)
    {
        if (GameManager.Instance.GetPlayerDataSo.GetTotalBalanceCoins >= total)
        {
            GameManager.Instance.GetPlayerDataSo.RemovePlayerCoins(total);

            RefillGas(liters);
            ShowPurchasedMessage(
                $"You refueled your scooter.\nYour account has been charged <color=red> ${total:0.##}</color>.\n"
                    + $"Drive safely and keep an eye on your fuel gauge!",
                1.5f
            );

            StartCoroutine(CompleteGasTutorial(1.5f));
        }
        else
        {
            ShowPurchasedMessage("Looks like I'm a bit short on cash.");
            return;
        }

        StartCoroutine(TriggerPopup());
    }

    // Sửa ShowPurchasedMessage: auto-close, không dùng button
    private void ShowPurchasedMessage(string message, float autoCloseDelaySeconds = 1.5f)
    {
        if (!ValidatePurchasedRefs())
            return;

        purchasedText.text = message;

        // Hiện panel
        ShowCanvasGroup(purchasedPanel);

        // Hủy coroutine cũ nếu đang chạy
        if (_purchasedRoutine != null)
            StopCoroutine(_purchasedRoutine);
        _purchasedRoutine = StartCoroutine(AutoClosePurchased(autoCloseDelaySeconds));
    }

    private IEnumerator AutoClosePurchased(float delay)
    {
        // Tắt tương tác trong lúc chờ (tuỳ bạn giữ hay bỏ)
        purchasedPanel.interactable = false;

        yield return new WaitForSeconds(delay);

        HideCanvasGroup(purchasedPanel);
        _purchasedRoutine = null;
        //if (gasTrigger != null)
        //{
        //    gasTrigger.SetTriggerEnabled(false); // hoặc _pendingTrigger.gameObject.SetActive(false);
        //    gasTrigger = null;
        //}
    }

    void UpdateUI()
    {
        if (FuelSlider != null)
            FuelSlider.value = currentGas / MaxGas;
    }

    private void ShowCanvasGroup(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    private void HideCanvasGroup(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        cg.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.ActivateInteractionCrossHair();

        GameManager.Instance.ResumeControllers();
        //if (gasTrigger != null)
        //{
        //    gasTrigger.SetTriggerEnabled(false); // hoặc _pendingTrigger.gameObject.SetActive(false);
        //    gasTrigger = null;
        //}
        StartCoroutine(TriggerPopup());
    }

    private bool ValidateConfirmRefs()
    {
        if (!confirmPanel || !confirmText || !confirmYesButton || !confirmNoButton)
        {
            Debug.LogError("[GasSystem] Confirm panel references are missing.");
            return false;
        }
        return true;
    }

    private bool ValidatePurchasedRefs()
    {
        if (!purchasedPanel || !purchasedText || !purchasedOkButton)
        {
            Debug.LogError("[GasSystem] Purchased panel references are missing.");
            return false;
        }
        return true;
    }

    IEnumerator TriggerPopup()
    {
        yield return new WaitForSeconds(2f);

        gasTrigger.SetTriggerEnabled(false);
    }

    public void SaveGas()
    {
        PlayerPrefs.SetFloat(GAS_KEY, currentGas);
        PlayerPrefs.Save();
    }

    private void LoadGas()
    {
        currentGas = PlayerPrefs.GetFloat(GAS_KEY, MaxGas);
        UpdateUI();
    }

    private void OnDestroy()
    {
        SaveGas();
    }

    private void OnApplicationQuit()
    {
        SaveGas();
    }

    public void ResetGas(bool saveData = true)
    {
        currentGas = MaxGas;
        UpdateUI();
        if (saveData)
            SaveGas();
    }

    public void ShowPopupGas()
    {
        if (PlayerPrefs.GetInt("GasComplete", 0) == 1)
            return;

        GameManager.Instance.ShowFloatingMessage("I need to get gas for my vehicle and call it a day.", 2f);
        gasObject.SetActive(true);
        gasText.text = "Head to the gas station and fill 'er up!";
        PlayerPrefs.SetInt("GasComplete", 0);
        PlayerPrefs.Save();
    }

    public IEnumerator CompleteGasTutorial(float delay)
    {
        if (PlayerPrefs.GetInt("Tutorial1", 1) == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        gasObject.SetActive(false);
        PlayerPrefs.SetInt("GasComplete", 1);
        PlayerPrefs.Save();

        GasComplete?.Invoke();

        GasStationEvents.Hide();
        DonutShopEvents.Hide();
        MissionManager.Instance.HideMarketPortalIcon();

        ShowPopupGoHome();
    }

    public void ShowPopupGoHome()
    {
        if (GameManager.Instance.GetCurrentDay() == 1 && PlayerPrefs.GetInt("Tutorial1", 0) == 1)
        {
            gasObject.SetActive(true);
            gasText.text = "Time to go home and rest.";
        }
        PlayerPrefs.SetInt("GoHomeDay1", 1);
        PlayerPrefs.Save();
    }

    public IEnumerator ShowPopupAccepted()
    {
        GameManager.Instance.ShowFloatingMessage(
            "I've got my first order! Time to accept it and hit the road!", 2f);

        yield return new WaitForSeconds(3.2f);

        if (GameManager.Instance.GetCurrentDay() == 1 && PlayerPrefs.GetInt("Tutorial1", 0) == 1)
        {
            gasObject.SetActive(true);
            gasText.text = "Go to the Order menu on your phone to Accept your order.";
        }
    }

    public void HidePopupAccepted()
    {
        gasObject.SetActive(false);
    }

    [ContextMenu("Reset Gas")]
    void ResetGasContext()
    {
        ResetGas();
    }
}
