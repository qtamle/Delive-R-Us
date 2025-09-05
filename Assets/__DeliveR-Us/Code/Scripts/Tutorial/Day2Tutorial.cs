using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Day2Tutorial : MonoBehaviour, ITutorial
{
    [Header("Day 2 Popup")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    [TextArea]
    [SerializeField] private string[] popupTexts; 
    private int currentTextIndex = 0;

    public static event Action<bool> OnTutorialStateChanged;
    private bool _isTutorial;
    public bool IsTutorial
    {
        get => _isTutorial;
        set
        {
            if (_isTutorial != value)
            {
                _isTutorial = value;
                OnTutorialStateChanged?.Invoke(_isTutorial);

                TutorialManager.SetTutorialState(_isTutorial);
            }
        }
    }

    private void Awake()
    {
        GameManager.Instance.RegisterTutorial(this);
    }

    private void OnEnable()
    {
        GameManager.Instance.showTutorialDay2 += InitTutorial;
    }

    private void OnDisable()
    {
        GameManager.Instance.showTutorialDay2 -= InitTutorial;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("DoneTutorial") == 1)
        {
            return;
        }

        if (!PlayerPrefs.HasKey("TutorialDay2"))
        {
            PlayerPrefs.SetInt("TutorialDay2", 1);
            PlayerPrefs.Save();
        }
    }

    public void InitTutorial()
    {
        if (PlayerPrefs.GetInt("DoneSkipTutorial") == 1)
            return;

        if (!PlayerPrefs.HasKey("TutorialDay2"))
        {
            PlayerPrefs.SetInt("TutorialDay2", 1);
            PlayerPrefs.Save();
        }

        if (GameManager.Instance.GetCurrentDay() == 2 && PlayerPrefs.GetInt("TutorialDay2", 1) == 1)
        {
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        IsTutorial = true;
        GameManager.Instance.isTimePaused = true;

        Bed bed = FindFirstObjectByType<Bed>();
        if (bed != null)
        {
            bed.DisableInteraction();
        }

        popupPanel.SetActive(true);
        popupPanel.transform.localScale = Vector3.zero;
        popupText.text = popupTexts[0];
        currentTextIndex = 0;

        LeanTween.scale(popupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyPopup());
    }

    private IEnumerator WaitForEnterKeyPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentTextIndex++;

            if (currentTextIndex < popupTexts.Length)
            {
                popupText.text = popupTexts[currentTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(popupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        Bed bed = FindFirstObjectByType<Bed>();
                        if (bed != null)
                        {
                            bed.EnableInteraction();
                        }
                        popupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;
                GameManager.Instance.isTimePaused = false;

                PlayerPrefs.SetInt("TutorialDay2", 0);
                PlayerPrefs.Save();

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
