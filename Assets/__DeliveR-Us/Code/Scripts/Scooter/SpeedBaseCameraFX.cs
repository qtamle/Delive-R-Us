using UnityEngine;
using Cinemachine;

public class SpeedBaseCameraFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _targetRigidbody;

    [Header("Focal Length Settings")]
    [SerializeField] private float _minFocalLength = 14f;
    [SerializeField] private float _maxFocalLength = 10f;
    [SerializeField] private float _maxSpeed = 50f;
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Speed Lines Settings")]
    [SerializeField] private float _minEmission = 5f;
    [SerializeField] private float _maxEmission = 50f;
    [SerializeField] private float _enableThreshold = 20f; // new

    private float _currentFocalLength;
    public CinemachineVirtualCamera VirtualCam;
    public  ParticleSystem SpeedLines;

    void Start()
    {
        if (VirtualCam != null)
            _currentFocalLength = VirtualCam.m_Lens.FieldOfView; // Correct property
    }

    void LateUpdate()
    {
        if (VirtualCam == null || _targetRigidbody == null) return;

        float speed = _targetRigidbody.linearVelocity.magnitude; // Use .velocity, not .linearVelocity
        float t = Mathf.Clamp01(speed / _maxSpeed);

        // Focal Length (zoom in/out based on speed)
        float targetFocalLength = Mathf.Lerp(_minFocalLength, _maxFocalLength, t);
        _currentFocalLength = Mathf.Lerp(_currentFocalLength, targetFocalLength, Time.deltaTime * _smoothSpeed);
        VirtualCam.m_Lens.FieldOfView = _currentFocalLength;

        // Speed Lines (toggle + adjust emission)
        if (SpeedLines != null)
        {
            if (speed > _enableThreshold)
            {
                if (!SpeedLines.isPlaying)
                    SpeedLines.Play();

                var emission = SpeedLines.emission;
                emission.rateOverTime = Mathf.Lerp(_minEmission, _maxEmission, t);
            }
            else
            {
                if (SpeedLines.isPlaying)
                    SpeedLines.Stop();
            }
        }
    }
}
