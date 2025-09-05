using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Day3Tutorial : MonoBehaviour, ITutorial
{
    public static Day3Tutorial Instance { get; private set; }

    [Header("Day 3 Popup")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    [TextArea]
    [SerializeField] private string[] popupTexts;
    private int currentTextIndex = 0;

    [Header("Target Object After Tutorial")]
    public GameObject tutorialTargetObject;
    public GameObject areaBlue;

    public PlayerDataSO playerCoin;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameManager.Instance.RegisterTutorial(this);
    }

    private void OnEnable()
    {
        GameManager.Instance.showTutorialDay3 += InitTutorial;
    }

    private void OnDisable()
    {
        GameManager.Instance.showTutorialDay3 -= InitTutorial;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("DoneTutorial") == 1)
        {
            return;
        }

        if (!PlayerPrefs.HasKey("TutorialDay3"))
        {
            PlayerPrefs.SetInt("TutorialDay3", 1);
            PlayerPrefs.Save();
        }

        if (tutorialTargetObject != null)
            tutorialTargetObject.SetActive(false);

        if (areaBlue != null && GameManager.Instance.GetCurrentDay() == 3 && PlayerPrefs.GetInt("TutorialDay3", 1) == 1)
            areaBlue.SetActive(false);
    }

    public void InitTutorial()
    {
        if (PlayerPrefs.GetInt("DoneSkipTutorial") == 1)
            return;

        if (!PlayerPrefs.HasKey("TutorialDay3"))
        {
            PlayerPrefs.SetInt("TutorialDay3", 1);
            PlayerPrefs.Save();
        }

        if (GameManager.Instance.GetCurrentDay() == 3 && PlayerPrefs.GetInt("TutorialDay3", 1) == 1)
        {
            ShowPopup();
            PlayerPrefs.SetInt("OpenLaptop", 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowPopup()
    {
        IsTutorial = true;

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

                if (tutorialTargetObject != null)
                    tutorialTargetObject.SetActive(true);

                playerCoin.AddCoinsInWallet(50f);
                playerCoin.DoPayout();

                PlayerPrefs.SetInt("BuySomething", 1);
                PlayerPrefs.SetInt("TutorialDay3", 0);
                PlayerPrefs.SetInt("NotFoundOrderDay3", 0);
                PlayerPrefs.SetInt("DoneTutorial", 1);
                PlayerPrefs.Save();

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
