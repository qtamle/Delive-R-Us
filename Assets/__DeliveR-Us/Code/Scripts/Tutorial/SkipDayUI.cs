using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkipDayUI : MonoBehaviour
{
    [Header("UI")]
    public Image skipIcon;
    public Slider progressSlider;

    [Header("Settings")]
    public KeyCode skipKey = KeyCode.Space;
    public float holdDuration = 3f;

    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool shouldShowSkipUI = false;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }

        if (skipIcon != null)
        {
            skipIcon.gameObject.SetActive(false);
        }

        CheckSkipUIVisibility();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSkipUIVisibility();
    }

    private void CheckSkipUIVisibility()
    {
        bool canShowSkip = CanShowSkipUI();
        SetSkipUIActive(canShowSkip);
        shouldShowSkipUI = canShowSkip;
    }

    private bool CanShowSkipUI()
    {
        if (PlayerPrefs.GetInt("DoneSkipTutorial") == 1)
            return false;

        if (GameManager.Instance.demoVersion)
            return false;

        if (SceneManager.GetActiveScene().name == "MenuScene")
            return false;

        if (GameManager.Instance.sceneTransitionOverlay.activeInHierarchy)
            return false;

        if (Day1Tutorial.IsOpening)
            return false;

        return true;
    }

    private void Update()
    {
        if (shouldShowSkipUI != CanShowSkipUI())
        {
            CheckSkipUIVisibility();
        }

        if (!shouldShowSkipUI) return;

        if (Input.GetKeyDown(skipKey))
        {
            StartHold();
        }

        if (Input.GetKey(skipKey) && isHolding)
        {
            holdTimer += Time.deltaTime;

            if (progressSlider != null)
                progressSlider.value = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                CompleteSkip();
            }
        }

        if (Input.GetKeyUp(skipKey))
        {
            ResetHold();
        }
    }

    private void StartHold()
    {
        isHolding = true;
        holdTimer = 0f;

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.value = 0f;
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }
    }

    private void CompleteSkip()
    {
        GameManager.Instance.SkipTutorial();
        OrderManager.Instance.ClearAllOrders();
        OrderManager.Instance.ForceClearAllOrders();
        PlayerPrefs.SetInt("DoneSkipTutorial", 1);
        PlayerPrefs.Save();
        ResetHold();
        CheckSkipUIVisibility(); 
    }

    private void SetSkipUIActive(bool active)
    {
        shouldShowSkipUI = active;

        if (skipIcon != null)
            skipIcon.gameObject.SetActive(active);

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }

        if (!active && isHolding)
        {
            ResetHold();
        }
    }

    [ContextMenu("Delete Key Skip Tutorial")]
    void DeleteKeySkipTutorial()
    {
        PlayerPrefs.DeleteKey("DoneSkipTutorial");
        PlayerPrefs.Save();
        CheckSkipUIVisibility(); 
    }
}