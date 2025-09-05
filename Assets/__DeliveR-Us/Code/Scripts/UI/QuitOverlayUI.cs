using UnityEngine;
using UnityEngine.UI;

public class QuitOverlayUI : MonoBehaviour
{
    [SerializeField] private Button _confirmQuitBtn;
    [SerializeField] private Button _cancelQuitBtn;

    [SerializeField] private GameObject targetObject;

    private void Start()
    {
        _confirmQuitBtn.onClick.RemoveAllListeners();
        _confirmQuitBtn.onClick.AddListener(ConfirmQuitBtn_fn);

        _cancelQuitBtn.onClick.RemoveAllListeners();
        _cancelQuitBtn.onClick.AddListener(CancelQuitBtn_fn);
    }

    private void ConfirmQuitBtn_fn()
    {
        AudioManager.Instance.PlaySFX(AudioId.BtnClick);
        GameManager.Instance.AutoSave();
        Application.Quit();
    }

    private void CancelQuitBtn_fn()
    {

        AudioManager.Instance.PlaySFX(AudioId.BtnClick);


        GameManager.Instance.HideQuitUI();
    }

}
