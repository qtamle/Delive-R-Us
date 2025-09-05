using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetPositionPopup : MonoBehaviour
{
    [Header("Popup UI")]
    public GameObject resetPopup;
    public float tweenTime = 0.25f;

    [Header("Fade UI")]
    public Image fadePanel;
    public float fadeDuration = 0.5f;
    public float waitDuration = 2f;

    private Vector3 originalScale;

    private void Start()
    {
        if (resetPopup != null)
        {
            originalScale = resetPopup.transform.localScale;
            resetPopup.SetActive(false);
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false); 
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
        }
    }

    public void ShowPopupReset()
    {
        if (resetPopup == null) return;

        if (SceneManager.GetActiveScene().name != "CityScene")
        {
            GameManager.Instance.ShowFloatingMessage("You can't use that here.", 2f);
            return;
        }

        resetPopup.SetActive(true);
        resetPopup.transform.localScale = Vector3.zero;
        LeanTween.scale(resetPopup, originalScale, tweenTime).setEaseOutBack();
    }

    public void OnClickClose()
    {
        if (resetPopup == null) return;

        LeanTween.scale(resetPopup, Vector3.zero, tweenTime)
            .setEaseInBack()
            .setOnComplete(() =>
            {
                resetPopup.SetActive(false);
            });
    }

    public void OnConfirm()
    {
        OnClickClose();
        OrderManager.Instance.CloseBtn_fn();

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true); 
            LeanTween.value(fadePanel.gameObject, 0f, 1f, fadeDuration)
                .setOnUpdate((float val) =>
                {
                    Color c = fadePanel.color;
                    c.a = val;
                    fadePanel.color = c;
                })
                .setOnComplete(() =>
                {
                    if (ResetManager.Instance != null)
                    {
                        ResetManager.Instance.TriggerReset(ScooterManager.Instance.IsOnScooter);
                    }

                    LeanTween.delayedCall(waitDuration, () =>
                    {
                        LeanTween.value(fadePanel.gameObject, 1f, 0f, fadeDuration)
                            .setOnUpdate((float val) =>
                            {
                                Color c = fadePanel.color;
                                c.a = val;
                                fadePanel.color = c;
                            })
                            .setOnComplete(() =>
                            {
                                fadePanel.gameObject.SetActive(false); 
                            });
                    });
                });
        }
        else
        {
            if (ResetManager.Instance != null)
            {
                ResetManager.Instance.TriggerReset(ScooterManager.Instance.IsOnScooter);
            }
        }
    }
}

public static class ResetEvents
{
    public static Action OnResetPlayer;
    public static Action OnResetScooter;
    public static Action OnResetClone;
}
