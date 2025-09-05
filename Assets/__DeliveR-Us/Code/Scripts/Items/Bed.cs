using UnityEngine;

public class Bed : Interactable
{
    private string GetTag => $"{GameManager.Instance.GetPlayerDataSo.GetPlayerName}'s Bed";

    private bool isShowingMessage = false;
    [SerializeField] private GameObject areaBlue;

    public override void Interact()
    {
        if (!IsInteractive) return;
        if (GameManager.Instance.isSleeping) return;

        if (GameManager.Instance.currentDay == 30 && GameManager.Instance.bankDebtData.currentDebt > 0)
        {
            if (!isShowingMessage)
            {
                isShowingMessage = true;
                GameManager.Instance.ShowFloatingMessage(
                    "Your bank debt is due. You cannot rest at this time.",
                    onComplete: () =>
                    {
                        isShowingMessage = false;
                    });

            }
            return;
        }

        if (GameManager.Instance.isShippingForBoss)
        {
            if (!isShowingMessage)
            {
                isShowingMessage = true;
                GameManager.Instance.ShowFloatingMessage(
                    "I can't sleep. I have to go deliver the orders for the landlord, or I'll be kicked out.",
                    onComplete: () =>
                    {
                        isShowingMessage = false;
                    });
            }
            return;
        }

        if (PlayerPrefs.GetInt("SkipSleep", 0) == 0 || PlayerPrefs.GetInt("BuySomething") == 1 || PlayerPrefs.GetInt("OpenLaptop") == 1)
        {
            if (!isShowingMessage)
            {
                isShowingMessage = true;
                GameManager.Instance.ShowFloatingMessage(
                    "I can't sleep right now.",
                    onComplete: () =>
                    {
                        isShowingMessage = false;
                    });
            }
            return;
        }

        if (GameManager.Instance.phoneUIController != null && GameManager.Instance.phoneUIController.HasAcceptedOrder())
        {
            if (!isShowingMessage)
            {
                isShowingMessage = true;
                GameManager.Instance.ShowFloatingMessage(
                    "I can't sleep. I still have pending deliveries to make.",
                    onComplete: () => { isShowingMessage = false; });
            }
            return;
        }

        base.Interact();
        GameManager.Instance.SleepAndSkipDay();
        EndOfDay();
    }

    public override void OnStartDetection(string itemTag = null)
    {
        if (!IsInteractive) return;

        if (GameManager.Instance.currentDay == 30 && GameManager.Instance.bankDebtData.currentDebt > 0)
        {
            GameManager.Instance.ShowFloatingMessage("Your bank debt is due. You cannot rest at this time.");
        }
        else
        {
            base.OnStartDetection(GetTag);
        }
    }

    public override void OnStopDetection()
    {
        base.OnStopDetection();
    }

    private void Reset()
    {
        _interactionMsg = "Press F to Sleep at";
    }

    public void DisableInteraction()
    {
        IsInteractive = false;
        GameManager.Instance.HideInteractionUI();
        GameManager.Instance.OnInteractionExit();
    }

    public void EnableInteraction()
    {
        IsInteractive = true;
    }

    void EndOfDay()
    {
        if (GameManager.Instance.currentDay == 1)
        {
            PlayerPrefs.SetInt("Tutorial1", 0);
            PlayerPrefs.SetInt("HasReceivedOrderToday", 0);
            PlayerPrefs.SetInt("HasOrder", 0);
            PlayerPrefs.SetInt("HidePortal", 0);
            PlayerPrefs.Save();
            areaBlue.SetActive(true);
            Debug.Log("Done day 1");
        }
        else if (GameManager.Instance.currentDay == 2)
        {
            PlayerPrefs.SetInt("TutorialDay2", 0);
            PlayerPrefs.Save();
        }
        else if (GameManager.Instance.currentDay == 3)
        {
            PlayerPrefs.SetInt("TutorialDay3", 0);
            PlayerPrefs.Save();
        }
    }
}
