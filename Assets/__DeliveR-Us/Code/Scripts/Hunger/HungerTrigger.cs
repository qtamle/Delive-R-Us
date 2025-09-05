using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class HungerTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject popupPanel;

    public CanvasGroup fadePanel;
    public TextMeshProUGUI messageText;

    [Header("Hunger")]
    public float amount;

    private bool hasTriggered = false;
    private bool isPopupActive = false;

    public static event Action showGasStation;
    private void Start()
    {
        popupPanel.SetActive(false);
        messageText.gameObject.SetActive(false);
        fadePanel.alpha = 0f;
    }

    //private void Update()
    //{
    //    if (isPopupActive && !GameManager.Instance.isShippingForBoss)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Return))
    //        {
    //            OnYesClicked();
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Q))
    //        {
    //            OnNoClicked();
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            popupPanel.SetActive(true);
            GameManager.Instance.DeactivateInteractionCrossHair();
            GameManager.Instance.StopControllers();

            RectTransform rect = popupPanel.GetComponent<RectTransform>();
            Vector3 startPos = new Vector3(rect.anchoredPosition.x, -Screen.height, 0);
            Vector3 endPos = new Vector3(rect.anchoredPosition.x, 0, 0);

            rect.anchoredPosition = startPos;

            LeanTween.move(rect, endPos, 0.5f).setEaseOutBack().setOnComplete(() =>
            {
                StartCoroutine(WaitForPopupInput());
            });
        }

        isPopupActive = true;
    }

    private IEnumerator WaitForPopupInput()
    {
        bool inputReceived = false;

        while (!inputReceived)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputReceived = true;
                OnYesClicked();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                inputReceived = true;
                OnNoClicked();
            }

            yield return null; 
        }
    }

    void OnYesClicked()
    {
        if (GameManager.Instance.GetPlayerDataSo.GetTotalBalanceCoins >= 5f)
        {
            GameManager.Instance.GetPlayerDataSo.RemovePlayerCoins(5f);
            GameManager.Instance.DeactivateInteractionCrossHair();
            GameManager.Instance.StopControllers();
        }
        else
        {
            GameManager.Instance.ShowFloatingMessage("Looks like I'm a bit short on cash.", 2f);
            return;
        }
        isPopupActive = false;
        HungerSystem hunger = FindFirstObjectByType<HungerSystem>();

        hunger.isHungerPaused = true;
        popupPanel.SetActive(false);

        //GameManager.Instance.ActivateInteractionCrossHair();
        //GameManager.Instance.ResumeControllers();

        StartCoroutine(FadeSequence());
    }

    void OnNoClicked()
    {
        isPopupActive = false;
        popupPanel.SetActive(false);

        GameManager.Instance.ActivateInteractionCrossHair();
        GameManager.Instance.ResumeControllers();

        StartCoroutine(TriggerPopup());
    }

    IEnumerator FadeSequence()
    {
        Debug.Log("Hunger Trigger!!!!");

        HungerSystem hunger = FindFirstObjectByType<HungerSystem>();
        fadePanel.gameObject.SetActive(true);

        LeanTween.alphaCanvas(fadePanel, 1f, 1f);
        AudioManager.Instance.PlaySFX(AudioId.Eat);
        yield return new WaitForSeconds(3f);

        LeanTween.alphaCanvas(fadePanel, 0f, 1f);
        yield return new WaitForSeconds(1f);

        hunger.isHungerPaused = false;
        hunger.AddHunger(amount);

        fadePanel.gameObject.SetActive(false);
        GameManager.Instance.ShowFloatingMessage("That was delicious, I'm stuffed!", 2f);
        GameManager.Instance.ActivateInteractionCrossHair();
        GameManager.Instance.ResumeControllers();

        if (PlayerPrefs.GetInt("Tutorial1", 1) == 1)
        {
            HungerSystem.Instance.CompleteAteTutorial();
            GasSystem.Instance.ShowPopupGas();
        }

        StartCoroutine(TriggerPopup());
    }

    void ShowFloatingMessage(string text)
    {
        if (GameOver.Instance.isShowing)
            return;

        messageText.text = text;
        messageText.gameObject.SetActive(true);

        RectTransform rt = messageText.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -200f);

        LeanTween.moveY(rt, -348f, 1.5f).setEaseOutCubic();
        LeanTween
            .alphaText(rt, 0f, 1.5f)
            .setDelay(1f)
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

    IEnumerator TriggerPopup()
    {
        yield return new WaitForSeconds(2f);

        hasTriggered = false;
    }
}
