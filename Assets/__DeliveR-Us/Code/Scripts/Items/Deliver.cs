using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Deliver : MonoBehaviour
{
    [SerializeField] private KeyCode _checkoutKey = KeyCode.X;
    [SerializeField] private GameObject _vfx;

    private bool _checkForKey = false;

    private Transform _target = null;

    private void Update()
    {
        if (!_checkForKey) return;

        LookAtTarget();

        if (Input.GetKeyDown(_checkoutKey))
        {
            Interact();
        }
    }

    public void OnStartInteraction()
    {
        _checkForKey = true;
        GameManager.Instance.DispalyInteractionUI($"Press <b>{_checkoutKey}</b> to <b><color=#FEC601>deliver the order</color></b> to <color=#4B79A1>{OrderManager.Instance.GetCurrentOrderData.GetCustomerInfo.CustomerName}</color>");

    }
    public void OnStopInteraction()
    {
        _checkForKey = false;

        GameManager.Instance.HideInteractionUI();
    }


    public void Interact()
    {
        _checkForKey = false;

        AudioManager.Instance.PlaySFX(AudioId.Success);

        OrderManager.Instance.OnOrderDelivered();

        GameObject vfx  = Instantiate (_vfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public  void UpdateLookTarget( Transform newTarget)
    {
        _target = newTarget;

    }

    private void LookAtTarget()
    {
        if (_target == null) return;

         Vector3 direction = _target.position - transform.position;
        Quaternion toRotate = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation,  Quaternion.Euler(0f, toRotate.eulerAngles.y, 0f), 15f*Time.deltaTime);
    }
}
