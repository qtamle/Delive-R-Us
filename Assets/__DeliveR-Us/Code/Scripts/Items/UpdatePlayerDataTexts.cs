using System;
using System.Globalization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UpdatePlayerDataTexts : MonoBehaviour
{
    [SerializeField] private TextType _iType;

    private TextMeshProUGUI _iText;

    private void OnEnable()
    {
        if(_iText == null)
            _iText = GetComponent<TextMeshProUGUI>();
        
        UpdateText();

        GameManager.OnUserDataUpdate += UpdateText;
    }

    private void OnDisable()
    {
        GameManager.OnUserDataUpdate -= UpdateText;

        LeanTween.cancel(gameObject);
    }

    private void UpdateText()
    {
        string str = string.Empty;

        if (_iType == TextType.PlayerName)
        {
            str = GameManager.Instance.GetPlayerDataSo.GetPlayerName;
        }
        else if (_iType == TextType.PlayerBalance)
        {
            str = $"{GameManager.Instance.GetPlayerDataSo.GetTotalBalanceCoins}";
        }

        else if (_iType == TextType.PlayerTodaysEarning)
        {
            str = $"{GameManager.Instance.GetPlayerDataSo.GetWalletCoins}";
        }
        else if (_iType == TextType.CurrentDate)
        {
            str = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
        }
        else if (_iType == TextType.CurrentTime)
        {
            str = DateTime.Now.ToString("HH:mm", CultureInfo.CurrentCulture);
            UpdateTimeUiPerSecond();
        }

        _iText.text = str;
    }


    private void UpdateTimeUiPerSecond()
    {
        if (!gameObject.activeInHierarchy) return;

        _iText.text = DateTime.Now.ToString("HH:mm", CultureInfo.CurrentCulture);

        float waitTime = 60 - DateTime.Now.Second;
        
        if (waitTime == 60) 
            waitTime = 0;

        LeanTween.delayedCall(gameObject, waitTime, UpdateTimeUiPerSecond);
    }


    public enum TextType
    {
        PlayerName = 0,
        PlayerBalance = 1,
        PlayerTodaysEarning = 2,
        CurrentDate = 3,
        CurrentTime = 4,
    }
}
