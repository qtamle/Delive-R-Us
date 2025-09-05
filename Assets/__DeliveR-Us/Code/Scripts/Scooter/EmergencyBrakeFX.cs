using UnityEngine;

public class EmergencyBrakeFX : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Minimum velocity magnitude before brake SFX is allowed")]
    [SerializeField] private float minSpeedThreshold = 0.1f;

    [Header("Resources")]
    [SerializeField] private Audio _brakeAudio;
    [SerializeField] private AudioSource ScooterSfxSrc;

    private Rigidbody _rb;
    private bool _canPlayBrakeSFX = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_canPlayBrakeSFX) return;

        if (Input.GetKey(KeyCode.Space))
        {
            // Correct velocity check
            if (_rb.linearVelocity.magnitude < minSpeedThreshold)
                return;

            _canPlayBrakeSFX = false;

            bool isBrakeClipPlaying = ScooterSfxSrc.isPlaying && ScooterSfxSrc.clip == _brakeAudio.Clip;

            if (!isBrakeClipPlaying)
            {
                ScooterSfxSrc.clip = _brakeAudio.Clip;
                ScooterSfxSrc.volume = _brakeAudio.ClipVolume * AudioManager.Instance.GetAudioSettings.SfxVolume;
                ScooterSfxSrc.Play();

                Invoke(nameof(ResetCanBrakeSFX), 3f);
            }
        }
    }

    private void ResetCanBrakeSFX() => _canPlayBrakeSFX = true;
}
