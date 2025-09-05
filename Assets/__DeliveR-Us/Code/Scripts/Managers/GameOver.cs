using MHUtility;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Settings")]
    [SerializeField] private Vector2 startPosition = new Vector2(0, -300f);
    [SerializeField] private float popupDuration = 0.8f;
    [SerializeField] private float hideDuration = 0.5f;

    [Header("Script")]
    public HungerSystem HungerSystem;
    public PlayerDataSO PlayerDataSO;
    public BankDebtData BankDebtData;
    public ViolationData ViolationData;
    public BankPopup BankPopup;

    private RectTransform rt;
    public bool isShowing = false;

    public static GameOver Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (gameOverCanvasGroup != null)
        {
            rt = gameOverCanvasGroup.GetComponent<RectTransform>();
            gameOverCanvasGroup.gameObject.SetActive(false);
        }

        messageText.gameObject.SetActive(false);
    }

    public void ShowGameOver(string reason = "")
    {
        if (isShowing) return;
        StartCoroutine(ShowGameOverRoutine(reason));
    }

    private IEnumerator ShowGameOverRoutine(string reason)
    {
        if (gameOverCanvasGroup == null)
        {
            Debug.LogError("[GameOverPopup] CanvasGroup is null!");
            yield break;
        }

        AudioManager.Instance.PlaySFX(AudioId.GameOver);

        isShowing = true;
        OrderManager.Instance.ClearAllOrders();
        OrderManager.Instance.ForceClearAllOrders();

        if (!string.IsNullOrEmpty(reason))
        {
            ShowFloatingMessage(reason);
            yield return new WaitForSeconds(4f);
        }

        gameOverCanvasGroup.alpha = 0f;
        rt.anchoredPosition = startPosition;
        gameOverCanvasGroup.gameObject.SetActive(true);

        GameManager.Instance.StopControllers();
        GameManager.Instance.DeactivateInteractionCrossHair();

        bool animationDone = false;

        LeanTween.moveY(rt, 0f, popupDuration).setEaseOutBack();
        LeanTween.alphaCanvas(gameOverCanvasGroup, 1f, popupDuration)
            .setOnComplete(() => animationDone = true);

        yield return new WaitUntil(() => animationDone);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        animationDone = false;
        LeanTween.moveY(rt, startPosition.y, hideDuration).setEaseInBack();
        LeanTween.alphaCanvas(gameOverCanvasGroup, 0f, hideDuration)
            .setOnComplete(() =>
            {
                GameManager.Instance.DeleteMission();
                GameManager.Instance.isTimePaused = true;
                GameManager.Instance.isShippingForBoss = false;
                GameManager.Instance.payoutDoneToday = false;
                OrderManager.Instance.CloseBtn_fn();
                HungerSystem.ResestHunger();
                GameManager.Instance.ResetTime();
                GameManager.Instance.ResetData();
                PlayerDataSO.GetPlayerData.ResetData();
                BankDebtData.ResetDebt();
                ViolationData.ResetViolations();
                GasSystem.Instance.ResetGas();
                SaveManager.SaveData(PlayerDataSO);
                BankPopup.UpdateUI();

                gameOverCanvasGroup.gameObject.SetActive(false);
                animationDone = true;
            });

        yield return new WaitUntil(() => animationDone);
        isShowing = false;

        GameManager.Instance.isTimePaused = false;
        GameManager.Instance.ResumeControllers();
        GameManager.Instance.ActivateInteractionCrossHair();

        LeanTween.delayedCall(0.5f, () => GameManager.Instance.LoadScene(Scenes.ApartmentScene));
    }

    void ShowFloatingMessage(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);

        RectTransform rt = messageText.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -200f);

        LeanTween.moveY(rt, -348f, 1.5f).setEaseOutCubic();
        LeanTween
            .alphaText(rt, 0f, 1.5f)
            .setDelay(4f)
            .setOnComplete(() =>
            {
                messageText.gameObject.SetActive(false);
                messageText.color = new Color(
                    messageText.color.r,
                    messageText.color.g,
                    messageText.color.b,
                    1f
                );
            });
    }

    //[ContextMenu("Reset All Data")]
    //void ResetAllData()
    //{
    //    GameManager.Instance.isTimePaused = true;
    //    GameManager.Instance.isShippingForBoss = false;
    //    OrderManager.Instance.CloseBtn_fn();
    //    HungerSystem.DeleteHungerProgress();
    //    GameManager.Instance.ResetTime();
    //    GameManager.Instance.ResetData();
    //    PlayerDataSO.GetPlayerData.ResetData();
    //    BankDebtData.ResetDebt();
    //    GasSystem.Instance.ResetGas();
    //    SaveManager.SaveData(PlayerDataSO);
    //    BankPopup.UpdateUI();
    //}
}
