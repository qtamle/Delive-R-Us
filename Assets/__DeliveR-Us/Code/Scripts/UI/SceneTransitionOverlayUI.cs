using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionOverlayUI : MonoBehaviour
{
    #region Setters/Private Variables

    [Header("Controls")]
    [SerializeField] private float _maxWidthLoadingFiller = 589.84f;
    [Space][SerializeField] private float _minPenguineX = 500f;
    [SerializeField] private float _maxPenguineX = 1000f;

    [Header("Resources")]
    [SerializeField] private RectTransform _loadingFiller;
    [SerializeField] private RectTransform _loadingPenguine;

    private bool _isLoading = false;
    #endregion

    #region Unity Methods

    public void LoadGameScene(Scenes TargetScene)
    {
        if (_isLoading) return; 

        StartCoroutine(LoadGameSceneCrt(TargetScene));
    }

    #endregion


    IEnumerator LoadGameSceneCrt(Scenes TargetScene)
    {
        _isLoading = true;

        GameManager.Instance.DoFadeIn();
        OrderManager.Instance.DeactivateOrderManager();

        yield return new WaitForSeconds(1);

        float fillAmount = 0;
        float penguinePos = _minPenguineX;

        _loadingFiller.sizeDelta = new Vector2(0, _loadingFiller.sizeDelta.y);
        _loadingPenguine.anchoredPosition = new Vector2(penguinePos, _loadingPenguine.anchoredPosition.y);

        AsyncOperation operation = SceneManager.LoadSceneAsync(TargetScene.ToString());
        operation.allowSceneActivation = false;

        float loadingProgress = 0f;
        bool readyToActivate = false;

        while (!operation.isDone && !readyToActivate)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            fillAmount = Mathf.MoveTowards(fillAmount, targetProgress, 0.4f * Time.deltaTime);
            loadingProgress = fillAmount;

            _loadingFiller.sizeDelta = new Vector2(_maxWidthLoadingFiller * fillAmount, _loadingFiller.sizeDelta.y);

            penguinePos = Mathf.Lerp(_minPenguineX, _maxPenguineX, fillAmount);
            _loadingPenguine.anchoredPosition = new Vector2(penguinePos, _loadingPenguine.anchoredPosition.y);

            if (operation.progress >= 0.9f && fillAmount >= 0.95f)
            {
                readyToActivate = true;
            }

            yield return null;
        }

        _loadingFiller.sizeDelta = new Vector2(_maxWidthLoadingFiller, _loadingFiller.sizeDelta.y);
        _loadingPenguine.anchoredPosition = new Vector2(_maxPenguineX, _loadingPenguine.anchoredPosition.y);

        GameManager.Instance.DoFadeOut();
        yield return new WaitForSeconds(1f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        OrderManager.Instance.ActivateOrderManager();
        _loadingFiller.sizeDelta = new Vector2(0, _loadingFiller.sizeDelta.y);
        _loadingPenguine.anchoredPosition = new Vector2(_minPenguineX, _loadingPenguine.anchoredPosition.y);

        gameObject.SetActive(false);
        _isLoading = false;
    }
}