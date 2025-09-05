using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Mission Settings")]
    public MissionData[] availableMissions;
    public MissionData currentMissionData;

    [Header("UI")]
    public GameObject missionPanel; // Panel cha

    public GameObject playerPanel;

    public GameObject landLordPanel;

    public GameObject sceneTransitionOverlay;

    public TMP_Text playerText;

    public TMP_Text landLordText;
    public TMP_Text missionName; // Nội dung mô tả
    public TMP_Text customerText;

    [Header("Other")]
    public MissionsDeliverPoint missionsDeliverPoint;
    public GameObject characterPrefab;
    public Transform characterIcon;

    private CanvasGroup _panelCanvasGroup;

    private RectTransform _panelRect;

    private RectTransform landlordRect;
    private RectTransform playerRect;

    private MissionStage _stage = MissionStage.None;

    private GameObject Owner_NPC;

    public Transform delivery;
    public Transform package;

    public MissionTrigger missionTrigger;

    public MarketPortalsController marketPortalsController;

    private List<GameObject> activeNpcs = new List<GameObject>();
    private Queue<Transform> packageQueue = new Queue<Transform>();
    private List<Transform> usedPackagePoints = new List<Transform>();
    public int deliveriesCount;
    // private Transform Owner_Hand;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("isShippingForBoss", 0) == 1)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CityScene")
        {
            StartMission();
            HideMarketPortalIcon();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     StartMission();
        // }
    }

    public void SetCurrentMission(MissionData missionData)
    {
        currentMissionData = missionData;
    }

    public void InitMissionPoint()
    {
        missionsDeliverPoint = FindAnyObjectByType<MissionsDeliverPoint>();
    }

    public void StartMission()
    {
        InitMissionPoint();
        // Gán mô tả nếu có dữ liệu
        if (missionName != null)
        {
            missionName.text = currentMissionData.itemName;
        }
        HideMarketPortalIcon();
        // Hiện panel theo flow: in -> wait enter -> out
        StartCoroutine(ShowMissionPanel());
    }

    public IEnumerator ShowMissionPanel()
    {
        missionsDeliverPoint.ApartmenGameObject.SetActive(false);
        if (missionPanel == null)
            yield break;

        if (_panelCanvasGroup == null)
        {
            _panelCanvasGroup = missionPanel.GetComponent<CanvasGroup>();
            if (_panelCanvasGroup == null)
                _panelCanvasGroup = missionPanel.AddComponent<CanvasGroup>();
        }

        if (_panelRect == null)
            _panelRect = missionPanel.GetComponent<RectTransform>();

        while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LoadingScene" || sceneTransitionOverlay.activeInHierarchy || SceneManager.GetActiveScene().name == "MenuScene")
            yield return null;

        missionPanel.SetActive(true);
        _panelCanvasGroup.alpha = 0f;

        Vector2 anchored = _panelRect.anchoredPosition;
        anchored.y = 300f;
        _panelRect.anchoredPosition = anchored;

        try
        {
            GameManager.Instance?.StopControllers();
            GameManager.Instance?.DeactivateInteractionCrossHair();
        }
        catch
        { /* an toàn nếu không có GameManager */
        }

        // Tránh chồng tween
        LeanTween.cancel(_panelRect.gameObject);
        LeanTween.cancel(_panelCanvasGroup.gameObject);

        // 4) Animate IN
        bool animationDone = false;

        LeanTween.moveY(_panelRect, 0f, 0.8f).setEaseOutBack();
        LeanTween
            .alphaCanvas(_panelCanvasGroup, 1f, 0.8f)
            .setOnComplete(() => animationDone = true);

        yield return new WaitUntil(() => animationDone);

        // 5) Chờ người chơi nhấn Enter (Return)
        // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        // 6) Animate OUT
        animationDone = false;
        StartCoroutine(ShowLandLordPanel("Go deliver these orders to my customers. It's the only way you get to stay here."));
        // LeanTween.cancel(_panelRect.gameObject);
        // LeanTween.cancel(_panelCanvasGroup.gameObject);

        // LeanTween.moveY(_panelRect, 300f, 0.5f).setEaseInBack();
        // LeanTween
        //     .alphaCanvas(_panelCanvasGroup, 0f, 0.5f)
        //     .setOnComplete(() =>
        //     {
        //         // missionPanel.SetActive(false);
        //         // animationDone = true;
        //         StartCoroutine(ShowLandLordPanel("Hi hello pay rent please"));
        //         // Resume điều khiển trở lại
        //         try
        //         {
        //             // GameManager.Instance?.ResumeControllers();
        //             // GameManager.Instance?.ActivateInteractionCrossHair();
        //         }
        //         catch { }
        //     });

        // yield return new WaitUntil(() => animationDone);
        // StartCoroutine(SpawnMissionCharacter());
    }

    public IEnumerator ShowLandLordPanel(string text)
    {
        if (landLordPanel == null)
            yield break;
        landLordText.text = text;
        landLordPanel.SetActive(true);
        landlordRect = landLordPanel.GetComponent<RectTransform>();
        Vector2 anchored = landlordRect.anchoredPosition;
        anchored.y = 300f;
        landlordRect.anchoredPosition = anchored;
        LeanTween.cancel(landlordRect.gameObject);
        bool animationDone = false;

        LeanTween
            .moveY(landlordRect, 0f, 0.8f)
            .setEaseOutBack()
            .setOnComplete(() => animationDone = true);

        yield return new WaitUntil(() => animationDone);
        // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        animationDone = false;
        StartCoroutine(ShowPlayerMessages("On my way"));
        // LeanTween.cancel(rect.gameObject);

        // LeanTween
        //     .moveY(rect, 300f, 0.5f)
        //     .setEaseInBack()
        //     .setOnComplete(() =>
        //     {
        //         landLordPanel.SetActive(false);
        //         animationDone = true;
        //     });
        // LeanTween.cancel(_panelCanvasGroup.gameObject);

        // LeanTween.moveY(_panelRect, 300f, 0.5f).setEaseInBack();
        // LeanTween
        //     .alphaCanvas(_panelCanvasGroup, 0f, 0.5f)
        //     .setOnComplete(() =>
        //     {
        //         missionPanel.SetActive(false);
        //         animationDone = true;

        //         // StartCoroutine(ShowLandLordPanel("Hi hello pay rent please"));
        //         // Resume điều khiển trở lại
        //         try
        //         {
        //             // GameManager.Instance?.ResumeControllers();
        //             // GameManager.Instance?.ActivateInteractionCrossHair();
        //         }
        //         catch { }
        //     });
        // yield return new WaitUntil(() => animationDone);
    }

    public IEnumerator ShowPlayerMessages(string text)
    {
        if (playerPanel == null)
            yield break;
        playerText.text = text;
        playerPanel.SetActive(true);
        playerRect = playerPanel.GetComponent<RectTransform>();
        Vector2 anchored = playerRect.anchoredPosition;
        anchored.y = 300f;
        playerRect.anchoredPosition = anchored;
        LeanTween.cancel(playerRect.gameObject);
        bool animationDone = false;
        LeanTween
            .moveY(playerRect, 0f, 0.8f)
            .setEaseOutBack()
            .setOnComplete(() => animationDone = true);

        yield return new WaitUntil(() => animationDone);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        animationDone = false;
        LeanTween.cancel(playerRect.gameObject);
        LeanTween.cancel(landlordRect.gameObject);
        LeanTween.cancel(_panelCanvasGroup.gameObject);
        LeanTween
            .moveY(landlordRect, 300f, 0.5f)
            .setEaseInBack()
            .setOnComplete(() =>
            {
                landLordPanel.SetActive(false);
                animationDone = true;
            });
        LeanTween
            .moveY(playerRect, 300f, 0.5f)
            .setEaseInBack()
            .setOnComplete(() =>
            {
                playerPanel.SetActive(false);
                animationDone = true;
            });
        LeanTween.cancel(_panelCanvasGroup.gameObject);

        LeanTween.moveY(_panelRect, 300f, 0.5f).setEaseInBack();
        LeanTween
            .alphaCanvas(_panelCanvasGroup, 0f, 0.5f)
            .setOnComplete(() =>
            {
                missionPanel.SetActive(false);
                animationDone = true;

                try
                {
                    GameManager.Instance?.ResumeControllers();
                    GameManager.Instance?.ActivateInteractionCrossHair();
                }
                catch { }
            });
        yield return new WaitUntil(() => animationDone);
        StartCoroutine(SpawnMissionCharacter());
        GameManager.Instance.hungerSystem.isHungerPaused = false;
    }

    public IEnumerator SpawnMissionCharacter()
    {
        if (currentMissionData == null || currentMissionData.DeliveryPrefab == null)
            yield break;
        delivery = GetRandomDeliveryPoint();
        package = GetRandomPackagePoint();

        SpawnNpcAt(delivery, MissionNpcRole.Delivery);
        _stage = MissionStage.Accepted;
        yield return null;
        // Chờ một chút để đảm bảo nhân vật đã được tạo xong
        yield return new WaitForSeconds(0.1f);
    }

    public Transform GetRandomDeliveryPoint()
    {
        if (
            missionsDeliverPoint == null
            || missionsDeliverPoint.DeliveryPoints == null
            || missionsDeliverPoint.DeliveryPoints.Length == 0
        )
        {
            Debug.LogWarning("No delivery points available!");
            return null;
        }

        int randomIndex = Random.Range(0, missionsDeliverPoint.DeliveryPoints.Length);
        return missionsDeliverPoint.DeliveryPoints[randomIndex];
    }

    public Transform GetRandomPackagePoint()
    {
        //if (
        //    missionsDeliverPoint == null
        //    || missionsDeliverPoint.GetPackagePoints == null
        //    || missionsDeliverPoint.GetPackagePoints.Length == 0
        //)
        //{
        //    Debug.LogWarning("No package points available!");
        //    return null;
        //}

        //int randomIndex = Random.Range(0, missionsDeliverPoint.GetPackagePoints.Length);
        //return missionsDeliverPoint.GetPackagePoints[randomIndex];

        if (missionsDeliverPoint == null || missionsDeliverPoint.GetPackagePoints == null ||
        missionsDeliverPoint.GetPackagePoints.Length == 0)
        {
            Debug.LogWarning("No package points available!");
            return null;
        }

        // Lọc ra các điểm chưa sử dụng
        var availablePoints = new List<Transform>();
        foreach (var point in missionsDeliverPoint.GetPackagePoints)
        {
            if (!usedPackagePoints.Contains(point))
                availablePoints.Add(point);
        }

        if (availablePoints.Count == 0)
        {
            Debug.LogWarning("All package points are in use!");
            return null;
        }

        int randomIndex = Random.Range(0, availablePoints.Count);
        Transform selectedPoint = availablePoints[randomIndex];
        usedPackagePoints.Add(selectedPoint);
        return selectedPoint;
    }

    public void GetMissionTrigger(MissionTrigger ms)
    {
        customerText.text = "Customer";

        missionTrigger = ms;
        // DELIVERY: hiện hướng dẫn → đóng panel → chuyển sang PACKAGE
        if (ms.role == MissionNpcRole.Delivery && _stage == MissionStage.Accepted)
        {
            StartCoroutine(
                ShowMessage(
                    "You have to deliver the package to the destination.",
                    afterClose: () => DestroyPrefab() // Destroy & spawn ở PACKAGE (đã xử lý trong DestroyAndAdvance)
                )
            );
        }
        // PACKAGE: hiện thông báo hoàn thành → đóng panel → complete
        //else if (ms.role == MissionNpcRole.Package && _stage == MissionStage.AtPackage)
        //{
        //    StartCoroutine(
        //        ShowMessage(
        //            "Package received. Mission completed!",
        //            afterClose: () => DestroyPrefab() // Destroy & complete (trong DestroyAndAdvance)
        //        )
        //    );
        //    OpenMarketPortalIcon();
        //    missionsDeliverPoint.ApartmenGameObject.SetActive(true);
        //}
        else if (ms.role == MissionNpcRole.Package && _stage == MissionStage.AtPackage)
        {
            GameObject npcToRemove = ms.gameObject;
            Transform npcPoint = ms.transform;

            StartCoroutine(
                ShowMessage("Looks good. Thanks for the delivery!",
                afterClose: () =>
                {
                    if (npcToRemove != null)
                    {
                        activeNpcs.Remove(npcToRemove);
                        Destroy(npcToRemove);

                        if (usedPackagePoints.Contains(npcPoint))
                            usedPackagePoints.Remove(npcPoint);
                    }

                    activeNpcs.RemoveAll(npc => npc == null);

                    if (activeNpcs.Count == 0)
                    {
                        CompleteMission();
                    }
                })
            );
        }
    }

    private GameObject SpawnNpcAt(Transform point, MissionNpcRole role)
    {
        //// dọn cũ (nếu còn)
        //if (Owner_NPC != null)
        //{
        //    Destroy(Owner_NPC);
        //    Owner_NPC = null;
        //}

        //Owner_NPC = Instantiate(characterPrefab, point.position, point.rotation);

        //// đảm bảo có MissionTrigger và set role
        //var trig = Owner_NPC.GetComponent<MissionTrigger>();
        //if (trig == null)
        //    trig = Owner_NPC.AddComponent<MissionTrigger>();
        //trig.role = role; // <- QUAN TRỌNG: gán role để trigger biết mình là ai

        var npc = Instantiate(characterPrefab, point.position, point.rotation);

        var trig = npc.GetComponent<MissionTrigger>();
        if (trig == null)
            trig = npc.AddComponent<MissionTrigger>();
        trig.role = role;

        activeNpcs.Add(npc);
        return npc;
    }

    [ContextMenu("Destroy Prefab")]
    public void DestroyPrefab()
    {
        StartCoroutine(DestroyAndAdvance());
    }

    private IEnumerator DestroyAndAdvance()
    {
        if (Owner_NPC != null)
        {
            Destroy(Owner_NPC);
            Owner_NPC = null;
            yield return null; // đợi 1 frame an toàn
        }

        // Nếu đang ở bước DELIVERY -> chuyển sang PACKAGE
        //if (_stage == MissionStage.Accepted)
        //{
        //    if (package == null)
        //        package = GetRandomPackagePoint();
        //    if (characterPrefab == null || package == null)
        //        yield break;

        //    SpawnNpcAt(package, MissionNpcRole.Package);
        //    _stage = MissionStage.AtPackage;
        //}

        if (_stage == MissionStage.Accepted)
        {
            foreach (var npc in activeNpcs)
            {
                Destroy(npc);
            }
            activeNpcs.Clear();
            usedPackagePoints.Clear();

            for (int i = 0; i < deliveriesCount; i++)
            {
                var packagePoint = GetRandomPackagePoint();
                if (packagePoint != null)
                {
                    packageQueue.Enqueue(packagePoint);
                    SpawnNpcAt(packagePoint, MissionNpcRole.Package);
                }
                else
                {
                    Debug.LogError("Not enough unique package points available!");
                }
            }
            _stage = MissionStage.AtPackage;
        }
        // Nếu đang ở bước PACKAGE -> hoàn thành
        else if (_stage == MissionStage.AtPackage)
        {
            _stage = MissionStage.Completed;
            usedPackagePoints.Clear();
            GameManager.Instance.isShippingForBoss = false;
            PlayerPrefs.DeleteKey("isShippingForBoss");
            PlayerPrefs.Save();
            // TODO: Thưởng, đóng UI, bắn event hoàn thành, v.v.
            Debug.Log("Mission Completed!");
        }
    }

    public IEnumerator ShowMessage(string message, System.Action afterClose = null)
    {
        if (missionPanel == null)
            yield break;

        // Đảm bảo đã có tham chiếu
        if (_panelCanvasGroup == null)
            _panelCanvasGroup =
                missionPanel.GetComponent<CanvasGroup>()
                ?? missionPanel.AddComponent<CanvasGroup>();
        if (_panelRect == null)
            _panelRect = missionPanel.GetComponent<RectTransform>();

        // Set nội dung
        if (landLordText != null)
            landLordText.text = message;

        // Hiện panel
        missionPanel.SetActive(true);
        landLordPanel.SetActive(true);
        _panelCanvasGroup.alpha = 0f;

        Vector2 anchored = _panelRect.anchoredPosition;
        anchored.y = 300f;
        _panelRect.anchoredPosition = anchored;

        try
        {
            GameManager.Instance?.StopControllers();
            GameManager.Instance?.DeactivateInteractionCrossHair();
        }
        catch { }

        LeanTween.cancel(_panelRect.gameObject);
        LeanTween.cancel(landlordRect.gameObject);
        LeanTween.cancel(_panelCanvasGroup.gameObject);

        bool animationDone = false;
        LeanTween.moveY(_panelRect, 0f, 0.8f).setEaseOutBack();
        LeanTween.moveY(landlordRect, 0f, 0.8f).setEaseOutBack();
        LeanTween
            .alphaCanvas(_panelCanvasGroup, 1f, 0.8f)
            .setOnComplete(() => animationDone = true);
        yield return new WaitUntil(() => animationDone);

        // Chờ Enter
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        // Animate OUT
        animationDone = false;
        LeanTween.cancel(_panelRect.gameObject);
        LeanTween.cancel(landlordRect.gameObject);
        LeanTween.cancel(_panelCanvasGroup.gameObject);
        LeanTween.moveY(_panelRect, 300f, 0.5f).setEaseInBack();
        LeanTween.moveY(landlordRect, 300f, 0.5f).setEaseInBack();
        LeanTween
            .alphaCanvas(_panelCanvasGroup, 0f, 0.5f)
            .setOnComplete(() =>
            {
                missionPanel.SetActive(false);
                landLordPanel.SetActive(false);
                animationDone = true;

                try
                {
                    GameManager.Instance?.ResumeControllers();
                    GameManager.Instance?.ActivateInteractionCrossHair();
                    if (missionTrigger != null)
                    {
                        // Tắt trigger vừa chạm để không bị kích lại
                        missionTrigger.SetTriggerEnabled(false);
                        missionTrigger = null;
                        AudioManager.Instance.PlaySFX(AudioId.Complete);
                    }
                }
                catch { }
            });
        yield return new WaitUntil(() => animationDone);

        // Gọi callback sau khi panel đóng (advance mission)
        afterClose?.Invoke();
    }

    public bool HasUnfinishedMission()
    {
        MissionTrigger[] allMissions = FindObjectsOfType<MissionTrigger>();

        foreach (var mission in allMissions)
        {
            if (mission.CurrentStage == MissionStage.Accepted ||
                mission.CurrentStage == MissionStage.AtPackage)
            {
                return true;
            }
        }
        return false;
    }

    #region Call Martket Icon
    public void HideMarketPortalIcon()
    {
        marketPortalsController = FindFirstObjectByType<MarketPortalsController>();
        if (marketPortalsController == null)
        {
            Debug.Log("Errorrrrrrr");
        }
        else
        {
            marketPortalsController.DisableAllPortals();
        }
    }

    public void OpenMarketPortalIcon()
    {
        marketPortalsController = FindFirstObjectByType<MarketPortalsController>();
        if (marketPortalsController == null)
        {
            Debug.Log("Errorrrrrrr");
        }
        else
        {
            marketPortalsController.EnableAllPortals();
        }
    }
    #endregion

    private void CompleteMission()
    {
        _stage = MissionStage.Completed;
        usedPackagePoints.Clear();
        GameManager.Instance.isShippingForBoss = false;
        PlayerPrefs.DeleteKey("isShippingForBoss");
        PlayerPrefs.Save();

        OpenMarketPortalIcon();
        missionsDeliverPoint.ApartmenGameObject.SetActive(true);
        Debug.Log("Mission Completed! All packages delivered.");
        GameManager.Instance.ShowFloatingMessage("Whew, all the orders are finally delivered. I don't have to worry about being kicked out now.", 4f);
    }
}
