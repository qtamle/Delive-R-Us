using UnityEngine;

public class BankButton : MonoBehaviour
{
    public GameObject bankPopup;
    public GameObject bankIcon;
    public float tweenTime = 0.25f;

    private Vector3 originalScale;
    private Vector3 iconOriginalScale;

    private void Start()
    {
        if (bankPopup != null)
        {
            originalScale = bankPopup.transform.localScale;
            bankPopup.SetActive(false);
        }

        if (bankIcon != null)
        {
            iconOriginalScale = bankIcon.transform.localScale;
        }
    }

    public void OnClickBank()
    {
        if (bankPopup == null) return;

        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        LeanTween.cancel(bankIcon);
        bankIcon.transform.localScale = iconOriginalScale;

        LeanTween.scale(bankIcon, iconOriginalScale * 1.1f, 0.6f)
            .setEaseInOutSine()
            .setLoopPingPong(-1); 

        bankPopup.SetActive(true);
        bankPopup.transform.localScale = Vector3.zero;
        LeanTween.scale(bankPopup, originalScale, tweenTime).setEaseOutBack();
    }

    public void OnClickClose()
    {
        if (bankPopup == null) return;

        AudioManager.Instance.PlaySFX(AudioId.BtnClick);

        LeanTween.scale(bankPopup, Vector3.zero, tweenTime)
            .setEaseInBack()
            .setOnComplete(() =>
            {
                bankPopup.SetActive(false);

                LeanTween.cancel(bankIcon);
                LeanTween.scale(bankIcon, iconOriginalScale, 0.15f).setEaseOutBack();
            });
    }
}
