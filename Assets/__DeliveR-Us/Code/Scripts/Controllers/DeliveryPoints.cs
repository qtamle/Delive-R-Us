using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryPoints : MonoBehaviour
{
    #region Setters/Private Variables

    [SerializeField] private Transform[] _deliveryPoints;

    private List<int> _shuffledIndexes = new List<int>();
    private int _currentIndex = 0;
    private System.Random _rng = new System.Random();
    private GameObject _npcPF;

    #endregion

    #region Getters/public Variables

    public int GetNextRandomDeliveryPointIndex()
    {
        if (_deliveryPoints == null || _deliveryPoints.Length == 0)
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
        OrderManager.OrderCleanupAction += OrderCleanUp;
    }
    private void OnDisable()
    {
        OrderManager.OrderCleanupAction -= OrderCleanUp;
    }


    private void Start()
    {
        Reshuffle();

        SetTargetPoint();
    }

    #endregion

    private void SetTargetPoint()
    {
        if (!OrderManager.Instance.OrderCollected) return;

        int deliveryPointIndex = OrderManager.Instance.GetDeliveryPointIndex;

        if (deliveryPointIndex == -1)
        {
            deliveryPointIndex = GetNextRandomDeliveryPointIndex();
            OrderManager.Instance.UpdateDeliveryPoint(deliveryPointIndex);
        }

        if (deliveryPointIndex == -1)
        {
            print("No Delivery Point Data Found in Delivery Points List");
            return;
        }

        GameObject npcPF = OrderManager.Instance.GetCurrentOrderData.GetCustomerInfo.CustomerAvatar.prefab;

        _npcPF = Instantiate(npcPF, _deliveryPoints[deliveryPointIndex].position, _deliveryPoints[deliveryPointIndex].rotation);
    }
    private void OrderCleanUp()
    {
        if (_npcPF == null) return;

        Destroy(_npcPF);

        OrderManager.Instance.UpdateDeliveryPoint(-1);
    }

    private void Reshuffle()
    {
        _shuffledIndexes = Enumerable.Range(0, _deliveryPoints.Length).ToList();

        for (int i = 0; i < _shuffledIndexes.Count; i++)
        {
            int rand = _rng.Next(i, _shuffledIndexes.Count);
            (_shuffledIndexes[i], _shuffledIndexes[rand]) = (_shuffledIndexes[rand], _shuffledIndexes[i]);
        }

        _currentIndex = 0;
    }
}

