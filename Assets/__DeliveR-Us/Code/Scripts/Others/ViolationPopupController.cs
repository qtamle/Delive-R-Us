using UnityEngine;

public class ViolationPopupController : MonoBehaviour
{
    [Header("Popup Settings")]
    public GameObject popupPanel;
    public float tweenDuration = 0.3f;

    private bool isVisible = false;

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            popupPanel.transform.localScale = Vector3.zero;
        }

        UIInputManager.Instance.RegisterKey(UnityEngine.InputSystem.Key.L, ShowPopup);
        UIInputManager.Instance.RegisterKey(UnityEngine.InputSystem.Key.Enter, HidePopup);
    }

    private void OnDestroy()
    {
        if (UIInputManager.Instance != null)
        {
            UIInputManager.Instance.UnregisterKey(UnityEngine.InputSystem.Key.L, ShowPopup);
            UIInputManager.Instance.UnregisterKey(UnityEngine.InputSystem.Key.Enter, HidePopup);
        }
    }

    private void ShowPopup()
    {
        if (popupPanel == null || isVisible || GameManager.Instance.demoVersion) return;

        popupPanel.SetActive(true);
        GameManager.Instance.DeactivateInteractionCrossHair();
        popupPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(popupPanel, Vector3.one, tweenDuration).setEaseOutBack();

        isVisible = true;
    }

    private void HidePopup()
    {
        if (popupPanel == null || !isVisible || GameManager.Instance.demoVersion) return;

        LeanTween.scale(popupPanel, Vector3.zero, tweenDuration).setEaseInBack()
            .setOnComplete(() =>
            {
                popupPanel.SetActive(false);
                GameManager.Instance.ActivateInteractionCrossHair();
            });

        isVisible = false;
    }
}
