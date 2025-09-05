using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankPopup : MonoBehaviour
{
    [Header("References")]
    public BankDebtData debtData;
    public PlayerDataSO playerCoin; 

    [Header("Borrow UI")]
    public TextMeshProUGUI borrowAmountText;
    public Button plus1Button, plus5Button, plus10Button, clearButton, borrowButton;

    [Header("Repay UI")]
    public TextMeshProUGUI currentDebtText;
    public Button repayButton;
    public TextMeshProUGUI messageText;

    private int borrowAmount = 0;

    private void OnEnable()
    {
        UpdateUI();
    }

    private void Awake()
    {
        debtData.LoadDebt();
    }

    private void Start()
    {
        plus1Button.onClick.AddListener(() => ChangeBorrowAmount(1));
        plus5Button.onClick.AddListener(() => ChangeBorrowAmount(5));
        plus10Button.onClick.AddListener(() => ChangeBorrowAmount(10));
        clearButton.onClick.AddListener(() => ChangeBorrowAmount(-borrowAmount));
        borrowButton.onClick.AddListener(BorrowMoney);
        repayButton.onClick.AddListener(RepayDebt);

        messageText.gameObject.SetActive(false);
    }

    void ChangeBorrowAmount(int amount)
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        int maxDebt = 300;
        int currentDebt = (int)debtData.currentDebt;

        if (amount < 0)
        {
            borrowAmount = Mathf.Max(borrowAmount + amount, 0);
        }
        else
        {
            if (currentDebt + borrowAmount + amount > maxDebt)
            {
                int maxCanBorrow = maxDebt - currentDebt;

                if (maxCanBorrow <= 0)
                {
                    GameManager.Instance.ShowFloatingMessage("I cannot borrow more. Maximum debt reached.");
                    return;
                }
                else
                {
                    borrowAmount = maxCanBorrow;
                    GameManager.Instance.ShowFloatingMessage($"You can only borrow ${maxCanBorrow} more to reach the maximum debt of $300.");
                }
            }
            else
            {
                borrowAmount += amount;
            }
        }

        UpdateUI();
    }

    void BorrowMoney()
    {
        int getDay = GameManager.Instance.GetCurrentDay();

        if (borrowAmount <= 0)
        {
            GameManager.Instance.ShowFloatingMessage("Wait, I haven't entered an amount yet. I need to put a number in before I can borrow.", 3f);
            return;
        }

        if (getDay == 29 || getDay == 30)
        {
            GameManager.Instance.ShowFloatingMessage("I can't take out a loan this late in the month. The bank won't approve new loans until next month.", 4f);
            return;
        }

        if (borrowAmount > 0)
        {
            if (debtData.currentDebt + borrowAmount > 300)
            {
                int maxCanBorrow = 300 - (int)debtData.currentDebt;
                if (maxCanBorrow <= 0)
                {
                    GameManager.Instance.ShowFloatingMessage("I cannot borrow more. Maximum debt reached.", 2f);
                    return;
                }
                else
                {
                    borrowAmount = maxCanBorrow;
                    GameManager.Instance.ShowFloatingMessage($"You can only borrow ${maxCanBorrow} more to reach the maximum debt of $300.", 3f);
                }
            }

            AudioManager.Instance.PlaySFX(AudioId.BorrowMoney);

            int today = GameManager.Instance.GetCurrentDay();

            debtData.AddDebt(borrowAmount, today);
            playerCoin.AddCoinsInWallet(borrowAmount);
            playerCoin.DoPayout();
            borrowAmount = 0;
            UpdateUI();

            DebtCountdownUI debtCountdownUI = FindFirstObjectByType<DebtCountdownUI>();
            if (debtCountdownUI != null)
            {
                debtCountdownUI.UpdateOwedAmountUI();
                debtCountdownUI.ActiveWarning();
            }
        }
    }

    void RepayDebt()
    {
        if (playerCoin.GetTotalBalanceCoins < debtData.currentDebt)
        {
            GameManager.Instance.ShowFloatingMessage("I'm a little strapped for cash and can't pay it back yet.");
            return;
        }

        if (debtData.currentDebt <= 0)
        {
            GameManager.Instance.ShowFloatingMessage("I have no outstanding loans with the bank. Why am I trying to pay?", 3f);
            return;
        }

        AudioManager.Instance.PlaySFX(AudioId.PayMoney);

        playerCoin.RemovePlayerCoins(debtData.currentDebt);
        debtData.RepayDebt(debtData.currentDebt);
        debtData.borrowDay = 0;
        UpdateUI();
        GameManager.Instance.ShowFloatingMessage("I'm finally out of debt.");

        DebtCountdownUI debtCountdownUI = FindFirstObjectByType<DebtCountdownUI>();
        if (debtCountdownUI != null)
        {
            debtCountdownUI.UpdateOwedAmountUI();
            debtCountdownUI.ActiveWarning();
        }
    }

    public void UpdateUI()
    {
        borrowAmountText.text = "$" + borrowAmount.ToString();
        currentDebtText.text = "Amount Owed: " + "$" + debtData.currentDebt.ToString();
    }

    //void ShowFloatingMessage(string text)
    //{
    //    if (GameOver.Instance.isShowing)
    //        return;

    //    messageText.text = text;
    //    messageText.gameObject.SetActive(true);

    //    RectTransform rt = messageText.GetComponent<RectTransform>();
    //    rt.anchoredPosition = new Vector2(0, -200f);

    //    LeanTween.moveY(rt, -348f, 1.5f).setEaseOutCubic();
    //    LeanTween
    //        .alphaText(rt, 0f, 1.5f)
    //        .setDelay(1f)
    //        .setOnComplete(() =>
    //        {
    //            messageText.gameObject.SetActive(false);
    //            messageText.color = new Color(
    //                messageText.color.r,
    //                messageText.color.g,
    //                messageText.color.b,
    //                1f
    //            );
    //        });
    //}
}
