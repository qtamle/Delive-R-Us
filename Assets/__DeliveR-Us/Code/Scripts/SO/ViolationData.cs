using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ViolationData", menuName = "Violation/Violation Data")]
public class ViolationData : ScriptableObject
{
    [Header("Settings")]
    public int maxViolations = 5;

    [Header("Runtime Data")]
    [SerializeField] private int currentViolations = 0;

    private const string SaveKey = "Violation_Current";
    
    public int CurrentViolations => currentViolations;

    public event Action<int, int> OnViolationChanged;

    public void ResetViolations()
    {
        currentViolations = 0;
        SaveViolations();
        OnViolationChanged?.Invoke(currentViolations, maxViolations);
    }

    public void AddViolation()
    {
        if (GameManager.Instance.demoVersion)
            return;

        currentViolations++;
        if (currentViolations > maxViolations)
            currentViolations = maxViolations;

        SaveViolations();
        OnViolationChanged?.Invoke(currentViolations, maxViolations);

        if (currentViolations < maxViolations)
            { GameManager.Instance.ShowFloatingMessage("I've had one delivery violation. I need to be more careful from now on.", 3f); }
    }

    public bool IsGameOver()
    {
        return currentViolations >= maxViolations;
    }

    public void SaveViolations()
    {
        PlayerPrefs.SetInt(SaveKey, currentViolations);
        PlayerPrefs.Save();
    }

    public void LoadViolations()
    {
        currentViolations = PlayerPrefs.GetInt(SaveKey, 0);
        OnViolationChanged?.Invoke(currentViolations, maxViolations);
    }
}
