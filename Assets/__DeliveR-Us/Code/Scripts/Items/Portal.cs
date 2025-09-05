using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class Portal : MonoBehaviour
{
    #region Unity Methods

    [SerializeField] private Scenes _newTargetScene = Scenes.CityScene;

    [SerializeField] private UnityEvent _onTransitionAction;

    [Header("If Portal City -> Super Market")]
    [SerializeField] private bool _isCityToMarket = false;
    [SerializeField] private MarketID _marketID;
    #endregion

    public void Interact()
    {
        if(_isCityToMarket && GameManager.Instance.ActiveScene == Scenes.CityScene && _newTargetScene == Scenes.SuperMarketScene)
        {
            GameManager.Instance.UpdateMarketID(_marketID);
        }

        GameManager.Instance.StopControllers(true);


        AudioManager.Instance.PlaySFX(AudioId.Teleport);

        _onTransitionAction?.Invoke();
        GameManager.Instance.LoadScene(_newTargetScene);

        if (PlayerPrefs.GetInt("GoHomeDay1") == 1)
        {
            GasSystem.Instance.gasObject.SetActive(false);
            PlayerPrefs.SetInt("GoHomeDay1", 0);
            PlayerPrefs.Save();
        }
    }
}
