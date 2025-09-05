using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class HideOnScooter : MonoBehaviour
{
    [SerializeField] private float checkInterval = 0.2f;

    private float _timer = 0f;
    private bool _currentVisibleState;

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MenuScene")
        {
            if (_currentVisibleState)
            {
                GameManager.Instance.DeactivateInteractionCrossHair();
                _currentVisibleState = false;
            }
            return;
        }

        _timer += Time.deltaTime;
        if (_timer < checkInterval) return;
        _timer = 0f;

        bool isOnScooter = ScooterManager.Instance != null && ScooterManager.Instance.IsOnScooter;
        bool isOpenPhone = OrderManager.Instance.PhoneUIOpen;

        bool isInTutorial = (PlayerPrefs.GetInt("DoneTutorial") == 0) && TutorialManager.IsTutorial;

        bool shouldBeVisible = !isInTutorial && !isOnScooter && !isOpenPhone;

        if (shouldBeVisible != _currentVisibleState)
        {
            if (shouldBeVisible)
            { 
                GameManager.Instance.ActivateInteractionCrossHair(); 
            }
            else
            {
                GameManager.Instance.DeactivateInteractionCrossHair();
            }

            _currentVisibleState = shouldBeVisible;
        }
    }
}


public static class TutorialManager
{
    public static event Action<bool> OnTutorialStateChanged;

    private static bool _isTutorial;
    public static bool IsTutorial => _isTutorial;

    public static void SetTutorialState(bool isTutorial)
    {
        if (_isTutorial != isTutorial)
        {
            _isTutorial = isTutorial;
            OnTutorialStateChanged?.Invoke(_isTutorial);
        }
    }
}

