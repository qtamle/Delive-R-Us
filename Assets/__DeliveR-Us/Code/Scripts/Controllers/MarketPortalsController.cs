using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarketPortalsController : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField]
    private MarketPortal[] _marketPortals;

    private List<int> _shuffledIndexes = new List<int>();
    private int _currentIndex = 0;
    private System.Random _rng = new System.Random();

    #endregion

    #region Getters/Public Variables

    public int GetNextRandomPortalIndex()
    {
        if (_marketPortals == null || _marketPortals.Length == 0)
            return -1;

        if (_currentIndex >= _shuffledIndexes.Count)
        {
            Reshuffle();
        }

        int index = _shuffledIndexes[_currentIndex];
        _currentIndex++;

        return index;
    }

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        OrderManager.UpdateTargetMarketAction += SetTargetPoint;
        OrderManager.OrderCleanupAction += OrderCleanUp;
    }

    private void OnDisable()
    {
        OrderManager.UpdateTargetMarketAction -= SetTargetPoint;
        OrderManager.OrderCleanupAction -= OrderCleanUp;
    }

    private void Start()
    {
        Reshuffle();

        if (
            OrderManager.Instance.PlayerBusy
            && !OrderManager.Instance.OrderCollected
            && GameManager.Instance.ActiveScene == Scenes.CityScene
        )
        {
            OrderManager.UpdateTargetMarketAction?.Invoke();
        }
        else if (OrderManager.Instance.OrderCollected)
        {
            foreach (MarketPortal market in _marketPortals)
            {
                market._marketEntrance.SetActive(false);
                market._marketExit.SetActive(false);
                market._miniMapIcon.SetActive(false);
            }
        }
    }

    #endregion

    private void SetTargetPoint()
    {
        if (!OrderManager.Instance.PlayerBusy)
            return;

        int marketPortalIndex = OrderManager.Instance.GetMarketPortalIndex;

        if (marketPortalIndex == -1)
        {
            marketPortalIndex = GetNextRandomPortalIndex();
            OrderManager.Instance.UpdateTargetMarketPortal(marketPortalIndex);
        }

        if (marketPortalIndex == -1)
        {
            print("No Market Data Found in Market Portal List");
            return;
        }

        for (int i = 0; i < _marketPortals.Length; i++)
        {
            bool isActive = (i == marketPortalIndex);

            _marketPortals[i]._marketEntrance.SetActive(isActive);
            _marketPortals[i]._marketExit.SetActive(isActive);
            _marketPortals[i]._miniMapIcon.SetActive(isActive);
        }
    }

    private void OrderCleanUp()
    {
        OrderManager.Instance.UpdateTargetMarketPortal(-1);

        foreach (MarketPortal market in _marketPortals)
        {
            market._marketEntrance.SetActive(true);
            market._marketExit.SetActive(true);
            market._miniMapIcon.SetActive(true);
        }
    }

    private void Reshuffle()
    {
        _shuffledIndexes = Enumerable.Range(0, _marketPortals.Length).ToList();

        for (int i = 0; i < _shuffledIndexes.Count; i++)
        {
            int rand = _rng.Next(i, _shuffledIndexes.Count);
            (_shuffledIndexes[i], _shuffledIndexes[rand]) = (
                _shuffledIndexes[rand],
                _shuffledIndexes[i]
            );
        }

        _currentIndex = 0;
    }

    public void DisableAllPortals()
    {
        foreach (var market in _marketPortals)
        {
            market._marketEntrance.SetActive(false);
            market._marketExit.SetActive(false);
            market._miniMapIcon.SetActive(false);
        }
    }

    public void EnableAllPortals()
    {
        foreach (var market in _marketPortals)
        {
            market._marketEntrance.SetActive(true);
            market._marketExit.SetActive(true);
            market._miniMapIcon.SetActive(true);
        }
    }
}

[System.Serializable]
public class MarketPortal
{
    public GameObject _marketEntrance;
    public GameObject _marketExit;
    public GameObject _miniMapIcon;
}
