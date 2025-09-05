using TMPro;
using UnityEngine;

public class DebtCountdownUI : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform panelTransform;
    public TextMeshProUGUI toggleHintText;
    public TextMeshProUGUI owedAmount;

    [Header("Positions")]
    public Vector3 openPos = new Vector3(50, 0, 0);
    public Vector3 closedPos = new Vector3(-219.7f, 0, 0);

    [Header("Settings")]
    public float moveDuration = 0.3f;

    [Header("Debt")]
    public BankDebtData debtData;

    [Header("GameObject")]
    public GameObject warningObj;

    private bool isOpen = true;
    private bool isMove = false;
    private LTDescr currentTween;

    void Start()
    {
        ActiveWarning();

        isOpen = false;
        panelTransform.anchoredPosition = closedPos;
        toggleHintText.text = "Press\n<color=yellow>T</color>\nto\ncheck debt";
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.T) && !isMove)
    //    {
    //        TogglePanel();
    //    }
    //}

    public void TogglePanel()
    {
        ActiveWarning();

        isOpen = !isOpen;
        isMove = true;

        if (currentTween != null)
        {
            LeanTween.cancel(panelTransform);
        }

        Vector2 targetPos = isOpen ? openPos : closedPos;

        currentTween = LeanTween.move(panelTransform, targetPos, moveDuration)
            .setEaseOutCubic()
            .setOnComplete(() => isMove = false);

        toggleHintText.text = isOpen
            ? "Press\n<color=yellow>T</color>\nto\nclose"
            : "Press\n<color=yellow>T</color>\nto\ncheck debt";

        UpdateOwedAmountUI();
    }

    public void UpdateOwedAmountUI()
    {
        if (debtData != null && debtData.currentDebt > 0)
        {
            owedAmount.gameObject.SetActive(true);
            owedAmount.text = "Owed amount: " + "$" + debtData.currentDebt.ToString();
        }
        else
        {
            owedAmount.gameObject.SetActive(false);
        }
    }

    public void ActiveWarning()
    {
        if (warningObj == null) return;

        LeanTween.cancel(warningObj);

        if (debtData.currentDebt > 0)
        {
            if (!warningObj.activeSelf)
            {
                warningObj.SetActive(true);
            }

            warningObj.transform.localScale = Vector3.one;

            LeanTween.scale(warningObj, Vector3.one * 1.2f, 0.8f)
                .setEaseInOutSine()
                .setLoopPingPong();
        }
        else
        {
            if (warningObj.activeSelf)
            {
                LeanTween.cancel(warningObj);
                LeanTween.scale(warningObj, Vector3.zero, 0.3f)
                    .setEaseInBack()
                    .setOnComplete(() => warningObj.SetActive(false));
            }
        }
    }
}

