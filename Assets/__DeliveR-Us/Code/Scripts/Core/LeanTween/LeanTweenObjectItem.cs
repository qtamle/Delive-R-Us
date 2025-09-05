using UnityEngine;

public class LeanTweenObjectItem : MonoBehaviour
{
    [SerializeField] Vector3 Original = Vector3.zero;
    [SerializeField] Vector3 target = Vector3.zero;

    [Space]
    [SerializeField] TweenType tweenType;
    [SerializeField] LeanTweenType easeType;

    [Space]
    [SerializeField] bool repeatEndless = false;
    [SerializeField] float delayBeforeTween = 0;
    [SerializeField] float tweenTime = 0;

    [Space]
    [SerializeField] GameObject targetObject;

    private void OnEnable()
    {
        if(targetObject == null) 
            targetObject = gameObject;

        LeanTween.cancel(targetObject);



        if (tweenType == TweenType.PositionType)
            DoPositionTween();
        else
            DoScaleTween();
    }
    private void OnDisable()
    {
        LeanTween.cancel(targetObject);

        if (tweenType == TweenType.PositionType)
            LeanTween.moveLocal(targetObject, Original, 0).setIgnoreTimeScale(true);
        else
            LeanTween.scale(targetObject, Original, 0).setIgnoreTimeScale(true);
    }


    void DoPositionTween()
    {
        LeanTween.moveLocal(targetObject, target, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            if (!repeatEndless) return;

            LeanTween.moveLocal(targetObject, Original, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                DoPositionTween();
            });

        });
    }
    void DoScaleTween()
    {
        LeanTween.scale(targetObject, target, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            if (!repeatEndless) return;

            LeanTween.scale(targetObject, Original, tweenTime).setDelay(delayBeforeTween).setEase(easeType).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                DoScaleTween();
            });

        });
    }
}
