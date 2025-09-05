using UnityEngine;

public class CircleFade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float duration = 1f;

    private void Start()
    {
        PlayFade();
    }

    private void PlayFade()
    {
        canvasGroup.alpha = 1f;

        LeanTween.alphaCanvas(canvasGroup, 0f, duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
