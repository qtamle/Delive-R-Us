using Cinemachine;
using MHUtility;
using SMPScripts;
using StarterAssets;
using UnityEngine;

public class ResourceController : Singleton<ResourceController>
{
    public ScooterRideExit ScooterPF;

    public CinemachineVirtualCamera GetCinemachineVirtualCamera;
    public ParticleSystem GetSpeedLines;
    public Transform ScooterDummyTransform;
    public ThirdPersonController GetThirdPersonController;

    public GameObject[] ScooterObjs;
    public GameObject[] FpsObjs;

    public Transform position;
    private void Start()
    {
        GameObject gameobject = GameObject.FindGameObjectWithTag("ResetPosition");
        position = gameobject.GetComponent<Transform>();
    }

    public void InstantiateScooter(Vector3 pos, Quaternion rot)
    {

        ScooterRideExit scooter = Instantiate(ScooterPF, pos, rot);

        scooter.ScooterDummyTransform = ScooterDummyTransform;
        scooter.FpsController = GetThirdPersonController; 
        scooter.ScooterController = scooter.GetComponent<MotoController>();

        scooter.ScooterObjs = ScooterObjs;
        scooter.FpsObjs = FpsObjs;
        GameManager.Instance.UpdateActivePlayerTransform(scooter.transform);


        SpeedBaseCameraFX baseCameraFX = scooter.GetComponent<SpeedBaseCameraFX>();
        baseCameraFX.VirtualCam = GetCinemachineVirtualCamera;
        baseCameraFX.SpeedLines = GetSpeedLines;

        GetCinemachineVirtualCamera.LookAt = scooter.ScooterController.GetCameraLook;
        GetCinemachineVirtualCamera.Follow = scooter.ScooterController.GetCameraFollow;
    }
}
