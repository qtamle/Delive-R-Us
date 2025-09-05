using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Day1Tutorial : MonoBehaviour, ITutorial
{
    [Header("Opening")]
    [SerializeField] private GameObject openingPanel;
    private CanvasGroup canvasGroup;

    [Header("Popup Day1")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    [TextArea]
    [SerializeField] private string[] popupTexts;
    private int currentTextIndex = 0;

    [Header("CityScene Popup")]
    [SerializeField] private GameObject cityPopupPanel;
    [SerializeField] private TextMeshProUGUI cityPopupText;
    [TextArea]
    [SerializeField] private string[] cityPopupTexts;
    private int currentCityTextIndex = 0;

    [Header("Order Popup")]
    [SerializeField] private GameObject orderPopupPanel;
    [SerializeField] private TextMeshProUGUI orderPopupText;
    [TextArea]
    [SerializeField] private string[] orderPopupTexts;
    private int currentOrderTextIndex = 0;

    [Header("Supermarket Popup")]
    [SerializeField] private GameObject supermarketPopupPanel;
    [SerializeField] private TextMeshProUGUI supermarketPopupText;
    [TextArea]
    [SerializeField] private string[] supermarketPopupTexts;
    private int currentSupermarketTextIndex = 0;

    [Header("Shipping")]
    [SerializeField] private GameObject shippingPopupPanel;
    [SerializeField] private TextMeshProUGUI shippingPopupText;
    [TextArea]
    [SerializeField] private string[] shippingPopupTexts;
    private int currentShippingTextIndex = 0;

    [Header("Complete Shipping")]
    [SerializeField] private GameObject CompleteShippingPopupPanel;
    [SerializeField] private TextMeshProUGUI CompleteShippingPopupText;
    [TextArea]
    [SerializeField] private string[] CompleteShippingPopupTexts;
    private int currentCompleteShippingTextIndex = 0;

    [Header("Gas Station")]
    [SerializeField] private GameObject GasPopupPanel;
    [SerializeField] private TextMeshProUGUI GasPopupText;
    [TextArea]
    [SerializeField] private string[] GasPopupTexts;
    private int currentGasTextIndex = 0;

    [Header("Go Home")]
    [SerializeField] private GameObject HomePopupPanel;
    [SerializeField] private TextMeshProUGUI HomePopupText;
    [TextArea]
    [SerializeField] private string[] HomePopupTexts;
    private int currentHomeTextIndex = 0;

    [Header("Tooltip")]
    [SerializeField] private GameObject popupTutorial;
    [SerializeField] private TextMeshProUGUI tutorialText;

    public static event Action<bool> OnTutorialStateChanged;

    public static bool IsOpening { get; private set; }

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

    public GameObject apartmentPoint;

    private void Awake()
    {
        GameManager.Instance.RegisterTutorial(this);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("DoneTutorial") == 1)
        {
            return;
        }

        if (!PlayerPrefs.HasKey("Tutorial1"))
        {
            PlayerPrefs.SetInt("Tutorial1", 1);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("CityScenePopup"))
        {
            PlayerPrefs.SetInt("CityScenePopup", 1);
            PlayerPrefs.Save();
        }

        if (openingPanel != null)
        { canvasGroup = openingPanel.GetComponent<CanvasGroup>(); }

        if (!PlayerPrefs.HasKey("TutorialDay1"))
        {
            PlayerPrefs.SetInt("TutorialDay1", 1);
            PlayerPrefs.Save();
        }

        //if (PlayerPrefs.GetInt("TutorialDay1") == 0)
        //{
        //    IsTutorial = false;
        //}
        if (GameManager.Instance.GetCurrentDay() == 1 && SceneManager.GetActiveScene().name == "ApartmentScene" && PlayerPrefs.GetInt("HidePortal") == 0)
        {
            ShowOpening();
        }

        if (SceneManager.GetActiveScene().name == "CityScene" || cityPopupPanel != null)
        {
            if (!PlayerPrefs.HasKey("CityScenePopup"))
            {
                PlayerPrefs.SetInt("CityScenePopup", 1); 
                PlayerPrefs.Save();
            }

            if (PlayerPrefs.GetInt("CityScenePopup") == 1)
            {
                //ShowCityPopup();    
                ShowPopupTutorial("Hop on your vehicle and wait for a new mission to come in!");
            }
        }

        if (PlayerPrefs.GetInt("ShippingComplete") == 0)
        {
            DonutShopEvents.Hide();
            GasStationEvents.Hide();
        }
    }

    private void OnEnable()
    {
        OrderManager.OnOrderReceived += ShowOrderPopup;
        SuperMarketTutorial.EnterSuperMarket += ShowSupermarketPopup;
        ShippingForCustomer.OnShipping += ShowShippingPopup;
        OrderManager.OnCompleteShipping += ShowCompleteShippingPopup;
        HungerTrigger.showGasStation += ShowCompleteGasPopup;
        GasSystem.GasComplete += ShowHomePopup;
        OrderManager.HidePopupCity += EndPopup;
    }

    private void OnDisable()
    {
        OrderManager.OnOrderReceived -= ShowOrderPopup;
        SuperMarketTutorial.EnterSuperMarket -= ShowSupermarketPopup;
        ShippingForCustomer.OnShipping -= ShowShippingPopup;
        OrderManager.OnCompleteShipping -= ShowCompleteShippingPopup;
        HungerTrigger.showGasStation -= ShowCompleteGasPopup;
        GasSystem.GasComplete -= ShowHomePopup;
        OrderManager.HidePopupCity -= EndPopup;
    }

    #region Day1 Opening (Apartment Scene)
    private void ShowOpening()
    {
        IsTutorial = true;
        IsOpening = true;
        openingPanel.SetActive(true);
        canvasGroup.alpha = 0f;

        LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f).setEaseOutQuad();

        StartCoroutine(WaitForEnterKey());
    }

    private IEnumerator WaitForEnterKey()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        bool finished = false;
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.5f).setEaseInQuad()
            .setOnComplete(() =>
            {
                openingPanel.SetActive(false);
                finished = true;
            });

        yield return new WaitUntil(() => finished);

        IsTutorial = false;
        IsOpening = false;

        ShowPopup();
    }

    private void ShowPopup()
    {
        IsTutorial = true;

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
                        PlayerPrefs.SetInt("TutorialDay1", 0);
                        PlayerPrefs.Save();
                        popupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;

                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

    #region CityScene Popup
    private void ShowCityPopup()
    {
        IsTutorial = true;

        if (apartmentPoint != null) 
        {
            apartmentPoint.SetActive(false);
        }

        cityPopupPanel.SetActive(true);
        cityPopupPanel.transform.localScale = Vector3.zero;
        cityPopupText.text = cityPopupTexts[0];
        currentCityTextIndex = 0;

        LeanTween.scale(cityPopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyCityPopup());
    }

    private IEnumerator WaitForEnterKeyCityPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentCityTextIndex++;

            if (currentCityTextIndex < cityPopupTexts.Length)
            {
                cityPopupText.text = cityPopupTexts[currentCityTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(cityPopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        cityPopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                PlayerPrefs.SetInt("CityScenePopup", 0); 
                PlayerPrefs.Save();
                IsTutorial = false;

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Order Popup
    public void ShowOrderPopup()
    {
        IsTutorial = true;

        PlayerPrefs.SetInt("OrderOne", 1);
        PlayerPrefs.Save();

        orderPopupPanel.SetActive(true);
        orderPopupPanel.transform.localScale = Vector3.zero;
        orderPopupText.text = orderPopupTexts[0];
        currentOrderTextIndex = 0;

        LeanTween.scale(orderPopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyOrderPopup());
    }

    private IEnumerator WaitForEnterKeyOrderPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentOrderTextIndex++;

            if (currentOrderTextIndex < orderPopupTexts.Length)
            {
                orderPopupText.text = orderPopupTexts[currentOrderTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(orderPopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        orderPopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Supermarket Popup
    public void ShowSupermarketPopup()
    {
        IsTutorial = true;

        supermarketPopupPanel.SetActive(true);
        supermarketPopupPanel.transform.localScale = Vector3.zero;
        supermarketPopupText.text = supermarketPopupTexts[0];
        currentSupermarketTextIndex = 0;

        LeanTween.scale(supermarketPopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeySupermarketPopup());
    }

    private IEnumerator WaitForEnterKeySupermarketPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentSupermarketTextIndex++;

            if (currentSupermarketTextIndex < supermarketPopupTexts.Length)
            {
                supermarketPopupText.text = supermarketPopupTexts[currentSupermarketTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(supermarketPopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        supermarketPopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Shipping Popup
    public void ShowShippingPopup()
    {
        IsTutorial = true;

        GasStationEvents.Hide();
        DonutShopEvents.Hide();

        shippingPopupPanel.SetActive(true);
        shippingPopupPanel.transform.localScale = Vector3.zero;
        shippingPopupText.text = shippingPopupTexts[0];
        currentShippingTextIndex = 0;

        LeanTween.scale(shippingPopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyShippingPopup());
    }

    private IEnumerator WaitForEnterKeyShippingPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentShippingTextIndex++;

            if (currentShippingTextIndex < shippingPopupTexts.Length)
            {
                shippingPopupText.text = shippingPopupTexts[currentShippingTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(shippingPopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        shippingPopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;
                OrderManager.Instance.GetTimer.Resume();
                TutorialManager.SetTutorialState(false);

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Complete Shipping Popup
    public void ShowCompleteShippingPopup()
    {
        IsTutorial = true;

        CompleteShippingPopupPanel.SetActive(true);
        CompleteShippingPopupPanel.transform.localScale = Vector3.zero;
        CompleteShippingPopupText.text = CompleteShippingPopupTexts[0];
        currentCompleteShippingTextIndex = 0;

        LeanTween.scale(CompleteShippingPopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyCompleteShippingPopup());
    }

    private IEnumerator WaitForEnterKeyCompleteShippingPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentCompleteShippingTextIndex++;

            if (currentCompleteShippingTextIndex < CompleteShippingPopupTexts.Length)
            {
                CompleteShippingPopupText.text = CompleteShippingPopupTexts[currentCompleteShippingTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(CompleteShippingPopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        CompleteShippingPopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Gas Popup
    public void ShowCompleteGasPopup()
    {
        IsTutorial = true;

        GasPopupPanel.SetActive(true);
        GasPopupPanel.transform.localScale = Vector3.zero;
        GasPopupText.text = GasPopupTexts[0];
        currentGasTextIndex = 0;

        LeanTween.scale(GasPopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyGasPopup());
    }

    private IEnumerator WaitForEnterKeyGasPopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentGasTextIndex++;

            if (currentGasTextIndex < GasPopupTexts.Length)
            {
                GasPopupText.text = GasPopupTexts[currentGasTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(GasPopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        GasPopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;
                GasSystem.Instance.ShowPopupGas();

                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Home Popup
    public void ShowHomePopup()
    {
        IsTutorial = true;

        HomePopupPanel.SetActive(true);
        HomePopupPanel.transform.localScale = Vector3.zero;
        HomePopupText.text = HomePopupTexts[0];
        currentHomeTextIndex = 0;

        LeanTween.scale(HomePopupPanel, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(WaitForEnterKeyHomePopup());
    }

    private IEnumerator WaitForEnterKeyHomePopup()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

            currentHomeTextIndex++;

            if (currentHomeTextIndex < HomePopupTexts.Length)
            {
                HomePopupText.text = HomePopupTexts[currentHomeTextIndex];
            }
            else
            {
                bool finished = false;
                LeanTween.scale(HomePopupPanel, Vector3.zero, 0.5f).setEaseInBack()
                    .setOnComplete(() =>
                    {
                        HomePopupPanel.SetActive(false);
                        finished = true;
                    });

                yield return new WaitUntil(() => finished);

                IsTutorial = false;

                if (apartmentPoint != null)
                {
                    apartmentPoint.SetActive(true);
                }

                PlayerPrefs.SetInt("OrderOne", 0);
                PlayerPrefs.SetInt("SkipSleep", 1);
                PlayerPrefs.SetInt("HidePortal", 1);
                PlayerPrefs.Save();

                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

    #region Popup

    void ShowPopupTutorial(string text)
    {
        if (GameManager.Instance.GetCurrentDay() == 1 && PlayerPrefs.GetInt("Tutorial1", 0) == 1)
        {
            popupTutorial.SetActive(true);
            tutorialText.text = text;
        }
    }

    void EndPopup()
    {
        popupTutorial.SetActive(false);
    }

    #endregion
}
