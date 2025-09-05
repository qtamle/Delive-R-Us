using System;
using UnityEngine;

public class Checkout : MonoBehaviour
{
    [SerializeField] private KeyCode _checkoutKey = KeyCode.F2;
    [SerializeField] private Collider _checkoutCollider;
    [SerializeField] private GameObject _arrowObj;
    [SerializeField] private GameObject areaBlue;

    private bool _checkForKey = false;

    private void OnEnable()
    {
        OrderManager.EnableCheckoutsAction += DisplayCollider;
        OrderManager.DisableCheckoutsAction += HideCollider;
    }
    private void OnDisable()
    {
        OrderManager.EnableCheckoutsAction -= DisplayCollider;
        OrderManager.DisableCheckoutsAction -= HideCollider;
    }

    private void Start()
    {
        HideCollider();
    }
    private void Update()
    {
        if (!_checkForKey) return;

        if (Input.GetKeyDown(_checkoutKey))
        {
            Interact();
        }
    }

    public void OnStartInteraction()
    {
        _checkForKey = true;
        GameManager.Instance.DispalyInteractionUI($"Press {_checkoutKey.ToString()} to <b><color=#FEC601>Checkout</color></b>");
    }
    public void OnStopInteraction()
    {
        _checkForKey = false;

        GameManager.Instance.HideInteractionUI();
    }

    public void Interact()
    {
        AudioManager.Instance.PlaySFX(AudioId.CollectItem);

        OrderManager.Instance.OnOrderCollected();
        GameManager.Instance.DisplayAlert(GameStrings.GetCheckoutSuccessAlert(), 1.5f);

        OrderManager.DisableCheckoutsAction?.Invoke();

        if (PlayerPrefs.GetInt("Pickup") == 1)
        {
            areaBlue.SetActive(true);
            PlayerPrefs.SetInt("Pickup", 0);
            PlayerPrefs.Save();
        }

        LeanTween.delayedCall(1.5f, () =>
        {
            GameManager.Instance.LoadScene(Scenes.CityScene);
        });
    }

    private void DisplayCollider()
    {
        _checkoutCollider.enabled = true;
        _arrowObj.gameObject.SetActive(true);
    }

    private void HideCollider()
    {
        _checkoutCollider.enabled = false;
        _arrowObj.gameObject.SetActive(false);
    }
}
