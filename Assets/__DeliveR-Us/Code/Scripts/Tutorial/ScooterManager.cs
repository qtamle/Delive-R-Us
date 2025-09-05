using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScooterManager : MonoBehaviour
{
    public static ScooterManager Instance { get; private set; }

    public static event Action<bool> OnScooterStateChanged;

    private bool _isOnScooter = false;
    public bool IsOnScooter => _isOnScooter;

    private GameObject arrowObj;
    private GameObject movementTxt;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        FindUIObjects();
    }

    private void OnEnable()
    {
        OnScooterStateChanged += HandleScooterStateChanged;
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        OnScooterStateChanged -= HandleScooterStateChanged;
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void SetScooterState(bool state)
    {
        if (_isOnScooter == state) return;

        _isOnScooter = state;
        OnScooterStateChanged?.Invoke(state);
    }

    private void HandleScooterStateChanged(bool isOnScooter)
    {
        if (arrowObj != null)
            arrowObj.SetActive(!isOnScooter);

        if (movementTxt != null)
            movementTxt.SetActive(isOnScooter);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        FindUIObjects();

        HandleScooterStateChanged(_isOnScooter);
    }

    private void FindUIObjects()
    {
        arrowObj = GameObject.FindWithTag("Arrow");
        movementTxt = GameObject.FindWithTag("MovementTxt");
    }
}
