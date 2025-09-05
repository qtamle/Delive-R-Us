using SMPScripts;
using StarterAssets;
using UnityEngine;

public class ScooterRideExit : MonoBehaviour
{

    [SerializeField] private KeyCode _exitRideKey = KeyCode.F;
    [SerializeField] private float _distanceFromScooter = 1;

    [Space]
    public Transform ScooterDummyTransform;
    public ThirdPersonController FpsController;
    public MotoController ScooterController;
    public GameObject[] ScooterObjs;
    public GameObject[] FpsObjs;

    private float _updatePlayerPosInterval = 5f;
    private float _timeSinceUpdatePlayerPos = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(_exitRideKey))
        {
            ExitRide();
        }

        if (ScooterController == null) return;

        _timeSinceUpdatePlayerPos += Time.deltaTime;

        if (_timeSinceUpdatePlayerPos >= _updatePlayerPosInterval)
        {
            _timeSinceUpdatePlayerPos = 0;

            FpsController.transform.position = GetExitPos();
        }
    }


    private void ExitRide()
    {
        ScooterManager.Instance.SetScooterState(false);

        ScooterRadioUI.ToggleUIAction?.Invoke(null);


        FpsController.transform.position = GetExitPos();

        GameManager.Instance.UpdateActivePlayerTransform(FpsController.transform);

        foreach (var obj in ScooterObjs)
        {
            obj.SetActive(false);
        }

        foreach (var obj in FpsObjs)
        {
            obj.SetActive(true);
        }

        FpsController.transform.position = GetExitPos();

        Destroy(ScooterController.gameObject);


        ScooterDummyTransform.transform.position = transform.position;
        ScooterDummyTransform.transform.rotation = Quaternion.Euler(ScooterDummyTransform.transform.eulerAngles.x, transform.rotation.eulerAngles.y, ScooterDummyTransform.transform.eulerAngles.z);

        GameManager.Instance.GetPlayerInteractionControllerRef.ClearLastTarget();
        GameManager.Instance.ActivateInteractionCrossHair();
        FpsController.enabled = true;

        gameObject.SetActive(false);
    }

    private Vector3 GetExitPos()
    {
        float heightOffset = 0.5f;
        Vector3 basePos = transform.position + new Vector3(0, heightOffset, 0);

        Vector3[] posToCheck = new Vector3[2]
        {
        basePos + (transform.right * _distanceFromScooter),
        basePos + (-transform.right * _distanceFromScooter)
        };


        foreach (Vector3 pos in posToCheck)
        {
            if (!Physics.CheckSphere(pos, 0.3f))
                return pos;
        }

        return posToCheck[0];
    }
}
