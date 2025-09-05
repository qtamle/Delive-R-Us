using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputManager : MonoBehaviour
{
    public static UIInputManager Instance { get; private set; }

    private Dictionary<Key, InputAction> _keyActions = new Dictionary<Key, InputAction>();
    private Dictionary<Key, Action> _callbacks = new Dictionary<Key, Action>();

    private InputAction _mouseClickAction;
    private Action _mouseClickCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Keyboard
    public void RegisterKey(Key key, Action callback)
    {
        if (!_keyActions.ContainsKey(key))
        {
            var action = new InputAction(binding: $"<Keyboard>/{key.ToString().ToLower()}");
            action.performed += ctx =>
            {
                if (_callbacks.ContainsKey(key))
                    _callbacks[key]?.Invoke();
            };
            action.Enable();

            _keyActions[key] = action;
        }

        if (_callbacks.ContainsKey(key))
            _callbacks[key] += callback;
        else
            _callbacks[key] = callback;
    }

    public void UnregisterKey(Key key, Action callback)
    {
        if (_callbacks.ContainsKey(key))
        {
            _callbacks[key] -= callback;
            if (_callbacks[key] == null)
                _callbacks.Remove(key);
        }
    }
    #endregion

    //#region Mouse
    //public void RegisterMouseClick(Action callback, bool includeRightClick = false)
    //{
    //    if (_mouseClickAction == null)
    //    {
    //        string binding = includeRightClick
    //            ? "<Mouse>/leftButton;<Mouse>/rightButton"
    //            : "<Mouse>/leftButton";

    //        _mouseClickAction = new InputAction(binding: binding);
    //        _mouseClickAction.performed += ctx => _mouseClickCallback?.Invoke();
    //        _mouseClickAction.Enable();
    //    }

    //    _mouseClickCallback += callback;
    //}

    //public void UnregisterMouseClick(Action callback)
    //{
    //    if (_mouseClickAction == null) return;

    //    _mouseClickCallback -= callback;
    //    if (_mouseClickCallback == null)
    //    {
    //        _mouseClickAction.Disable();
    //        _mouseClickAction.Dispose();
    //        _mouseClickAction = null;
    //    }
    //}
    //#endregion

    private void OnDestroy()
    {
        foreach (var action in _keyActions.Values)
        {
            action.Dispose();
        }
        _keyActions.Clear();
        _callbacks.Clear();

        if (_mouseClickAction != null)
        {
            _mouseClickAction.Dispose();
            _mouseClickAction = null;
            _mouseClickCallback = null;
        }
    }
}
