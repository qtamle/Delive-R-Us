using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateNameUI : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField _nameInputField;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _namePlaceHolderText;

    [Header("Buttons")]
    [SerializeField] private Button _saveNameBtn;
    [SerializeField] private Button _closeBtn;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        _nameInputField.text = string.Empty;
        _namePlaceHolderText.text = GameManager.Instance.GetPlayerDataSo.GetPlayerName;

    }
    private void Start()
    {
        RegisterBtnEvents();
    }

    #endregion

    #region Register Button Events

    private void RegisterBtnEvents()
    {
        _closeBtn.onClick.RemoveAllListeners();
        _saveNameBtn.onClick.RemoveAllListeners();

        _closeBtn.onClick.AddListener(CloseBtn_fn);
        _saveNameBtn.onClick.AddListener(SaveNameBtn_fn);
    }

    #endregion

    private void SaveNameBtn_fn()
    {
        string newName = _nameInputField.text.Trim();

        if (string.IsNullOrEmpty(newName) || newName.Length < 3)
        {
            GameManager.Instance.DisplayAlert(GameStrings.GetInvalidNameAlert(), 3);
        }
        else
        {
            GameManager.Instance.GetPlayerDataSo.UpdatePlayerName(newName);
            _nameInputField.text = string.Empty;
            _namePlaceHolderText.text = newName;

            GameManager.OnUserDataUpdate?.Invoke();

            gameObject.SetActive(false);
        }
    }
    private void CloseBtn_fn() 
    {
        gameObject.SetActive(false);
    }

}
