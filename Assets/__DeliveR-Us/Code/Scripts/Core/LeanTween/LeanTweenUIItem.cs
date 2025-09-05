using UnityEngine;

public enum TweenType { PositionType, SizeType}

[RequireComponent(typeof(RectTransform))]
public class LeanTweenUIItem : MonoBehaviour
{
    [SerializeField] Vector2 Original = Vector2.zero;
    [SerializeField] Vector2 target = Vector2.zero;

    [Space]
    [SerializeField] TweenType tweenType;
    [SerializeField] LeanTweenType easeType;

    [Space]
    [SerializeField] bool repeatEndless = false;
    [SerializeField] float delayBeforeTween = 0;
    [SerializeField] float tweenTime = 0;

    [Space]
    [SerializeField] RectTransform rectTransform;

    private void OnEnable()
    {
        LeanTween.cancel(rectTransform);

        if (tweenType == TweenType.PositionType)
            DoPositionTween();
        else
            DoScaleTween();
    }
    private void OnDisable()
    {
        LeanTween.cancel(rectTransform);

        if (tweenType == TweenType.PositionType)
            LeanTween.move(rectTransform, Original, 0).setIgnoreTimeScale(true);
        else
            LeanTween.scale(rectTransform, Original, 0).setIgnoreTimeScale(true);
    }


    void DoPositionTween()
    {
        LeanTween.move(rectTransform, target, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            if (!repeatEndless) return;

            LeanTween.move(rectTransform, Original, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                DoPositionTween();
            });

        });
    }
    void DoScaleTween()
    {
        LeanTween.scale(rectTransform, target, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            if (!repeatEndless) return;

            LeanTween.scale(rectTransform, Original, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                DoScaleTween();
            });

        });
    }
}
