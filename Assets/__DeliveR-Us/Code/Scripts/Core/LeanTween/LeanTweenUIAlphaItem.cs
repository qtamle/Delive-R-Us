using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class LeanTweenUIAlphaItem : MonoBehaviour
{
    [SerializeField] float original = 0f;
    [SerializeField] float target = 1f;

    [Space]
    [SerializeField] bool repeatEndless = false;
    [SerializeField] float delayBeforeTween = 0;
    [SerializeField] float leanTime = 0.3f;

    [Space]
    [SerializeField] CanvasGroup canvasGroup;

    [Header("if not repeat endless and want to add on complete action")]
    [SerializeField] UnityEvent OnCompleteAction ;

    private void OnEnable()
    {
        LeanTween.cancel(canvasGroup.gameObject);
        DoAlpha();
    }
    private void OnDisable()
    {
        LeanTween.cancel(canvasGroup.gameObject);
        canvasGroup.LeanAlpha(original, 0).setIgnoreTimeScale(true);
    }


    void DoAlpha()
    {
        canvasGroup.LeanAlpha(target, leanTime).setDelay(delayBeforeTween).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            if (!repeatEndless)
            {
                OnCompleteAction?.Invoke();
                return;
            }

            canvasGroup.LeanAlpha(original, leanTime).setDelay(delayBeforeTween).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                DoAlpha();
            });
        });

    }
}
