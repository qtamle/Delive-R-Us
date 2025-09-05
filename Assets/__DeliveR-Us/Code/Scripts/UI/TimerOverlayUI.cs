using TMPro;
using UnityEngine;

public class TimerOverlayUI : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Read-Only")]
    [SerializeField] private float _targetTimer = 0;
    [SerializeField] private float _remainingTimerSeconds = 0;
    [SerializeField] private string timeFormatted;

    private float _startTime = 0;
    private bool _doTimer = false;

    #endregion

    #region Getters/Public Variables

    public string GetFormattedTime => timeFormatted;
    public float GetTimerFiller =>  Mathf.Clamp01(_remainingTimerSeconds / _targetTimer);
    public int GetTimerSeconds => Mathf.CeilToInt(_remainingTimerSeconds);

    #endregion

    private void Update()
    {
        if (!_doTimer) return;

        ValidateTimer();

        UpdateTimerUI();
    }

    public void StartTimer(float targetTimer)
    {
        _targetTimer = targetTimer;

        _startTime = Time.time;
        _remainingTimerSeconds = _targetTimer;

        if (PlayerPrefs.GetInt("Tutorial1", 0) == 1)
        {
            _remainingTimerSeconds = Mathf.Infinity;
        }

        _doTimer = true;
    }
    public void Pause()
    {
        _doTimer = false;
    }

    public void Resume()
    {
        _startTime = Time.time - (_targetTimer - _remainingTimerSeconds);
        _doTimer = true;
    }

    private void ValidateTimer()
    {
        if (_remainingTimerSeconds == Mathf.Infinity) return;

        float elapsedTime = Time.time - _startTime;

        _remainingTimerSeconds= Mathf.Max(0, _targetTimer - elapsedTime);

        if(_remainingTimerSeconds <= 0f)
        {
            Pause();

            OrderManager.Instance.OrderFailed();
        }
    }
    private void UpdateTimerUI()
    {
        if (_remainingTimerSeconds == Mathf.Infinity)
        {
            timeFormatted = "∞";
        }
        else
        {
            int minutes = Mathf.FloorToInt(_remainingTimerSeconds / 60);
            int seconds = Mathf.FloorToInt(_remainingTimerSeconds % 60);
            timeFormatted = $"{minutes:00} : {seconds:00}";
        }

        _timerText.text = timeFormatted;
    }
}
