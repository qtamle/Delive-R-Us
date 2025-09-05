using SMPScripts;
using StarterAssets;
using UnityEngine;

public class ScooterDummy : Interactable
{
    [SerializeField] private string _iTag = "Scooter";

    [SerializeField] private ThirdPersonController _fpsController;
    [SerializeField] private GameObject[] scooterObjs;
    [SerializeField] private GameObject[] fpsObjs;

    public override void Interact()
    {
        base.Interact();

        ResourceController.Instance.InstantiateScooter(transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));

        _fpsController.StopPlayer();

        foreach(var obj in fpsObjs)
        {
            obj.SetActive(false);
        }

        foreach (var obj in scooterObjs)
        {
            obj.SetActive(true);
        }


        if (PlayerPrefs.GetInt("CityScenePopup") == 1)
        {
            PlayerPrefs.SetInt("CityScenePopup", 0);
            PlayerPrefs.Save(); 
        }

        GameManager.Instance.DeactivateInteractionCrossHair();

        ScooterManager.Instance.SetScooterState(true);

        gameObject.SetActive(false);

    }

    public override void OnStartDetection(string tag = null)
    {
        if (!IsInteractive) return;

        base.OnStartDetection(_iTag);
    }

    public override void OnStopDetection()
    {
        base.OnStopDetection(); 
    }
}
