using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPhoneUIController : MonoBehaviour
{
    #region Setters/Private Variables



    #endregion

    #region Unity Events

    private void OnEnable()
    {
        DefaultUI();
    }

    private void Start()
    {
        RegisterButtonEvents();
    }

    #endregion

    #region Register Button Events

    private void RegisterButtonEvents() { }

    #endregion

    private void DefaultUI() { }
}
