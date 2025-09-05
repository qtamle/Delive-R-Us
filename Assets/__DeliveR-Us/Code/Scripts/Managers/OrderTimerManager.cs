using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderTimerManager : MonoBehaviour
{
    #region Singleton
    public static OrderTimerManager Instance { get; private set; }

    public GameObject sceneTransitionOverlay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    private List<OrderData> _activeOrders = new List<OrderData>();

    private Dictionary<OrderData, Action<OrderData>> _onOrderTimeout = new Dictionary<OrderData, Action<OrderData>>();

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = _activeOrders.Count - 1; i >= 0; i--)
        {
            var order = _activeOrders[i];

            if (PlayerPrefs.GetInt("Tutorial1") == 1)
            {
                order.RemainingTime = Mathf.Infinity;
                continue; 
            }

            if (order.RemainingTime > 0f && !sceneTransitionOverlay.activeInHierarchy)
            {
                order.RemainingTime -= dt;

                if (order.RemainingTime <= 0f)
                {
                    order.RemainingTime = 0f;

                    if (_onOrderTimeout.TryGetValue(order, out var callback))
                    {
                        callback?.Invoke(order);
                    }

                    _activeOrders.RemoveAt(i);
                    _onOrderTimeout.Remove(order);
                }
            }
        }
    }

    public void AddOrder(OrderData order, Action<OrderData> onTimeout = null)
    {
        if (!_activeOrders.Contains(order))
        {
            _activeOrders.Add(order);

            if (onTimeout != null)
            {
                _onOrderTimeout[order] = onTimeout;
            }

            if (PlayerPrefs.GetInt("Tutorial1") == 1)
            {
                order.RemainingTime = Mathf.Infinity;
            }
            else if (order.RemainingTime <= 0f)
            {
                order.RemainingTime = UnityEngine.Random.Range(180f, 240f);
            }
        }
    }

    public void RemoveOrder(OrderData order)
    {
        if (_activeOrders.Contains(order))
        {
            _activeOrders.Remove(order);
        }
        if (_onOrderTimeout.ContainsKey(order))
        {
            _onOrderTimeout.Remove(order);
        }
    }

    public float GetRemainingTime(OrderData order)
    {
        return order.RemainingTime;
    }
}
