using UnityEngine;

public class Laptop : Interactable
{
    private string GetTag => $"{GameManager.Instance.GetPlayerDataSo.GetPlayerName}'s Laptop";

    private bool isShowingMessage = false;

    public override void Interact()
    {
        if (!IsInteractive) return;

        if (PlayerPrefs.GetInt("Tutorial1", 1) == 1 || PlayerPrefs.GetInt("TutorialDay3", 1) == 1)
        {
            if (!isShowingMessage)
            {
                isShowingMessage = true;
                GameManager.Instance.ShowFloatingMessage(
                    "I can't use the laptop right now.",
                    onComplete: () =>
                    {
                        isShowingMessage = false;
                    });
            }
            return;
        }

        base.Interact();

        ShopController.OpenShopAction?.Invoke();
    }

    public override void OnStartDetection(string itemTag = null)
    {
        if (!IsInteractive) return;

        base.OnStartDetection(GetTag);
    }

    public override void OnStopDetection()
    {
        base.OnStopDetection();

    }
}