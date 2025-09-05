using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SMPScripts
{
    public class MotoSound : MonoBehaviour
    {
        AudioSource audioSource;
        AudioMixer mixer;
        MotoController motoController;
        public AnimationCurve engineRPM;
        float prevPitch;
        public float EngineFlow = 1;
        public float maxVolumeOnMove = 0.5f;
        public float maxVolumeOnAirborne = 0.3f;

        private float _volumeMultiplier = 1f;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponents<AudioSource>()[0];
            motoController = FindObjectOfType<MotoController>();

            _volumeMultiplier = AudioManager.Instance.GetAudioSettings.SfxVolume;
        }

        private void OnEnable()
        {
            AudioManager.OnSettingsChanges += UpdateVolume;
        }

        private void OnDisable()
        {
            AudioManager.OnSettingsChanges -= UpdateVolume;
        }

        // Update is called once per frame
        void Update()
        {
            if (motoController.rawCustomAccelerationAxis > 0 && !motoController.isAirborne)
            {
                audioSource.pitch = (engineRPM.Evaluate(motoController.engineSettings.gearRatio + (motoController.engineSettings.currentGear) * 0.1f) + 1 + motoController.rb.linearVelocity.magnitude*0.05f);

                audioSource.pitch = Mathf.Lerp(prevPitch, audioSource.pitch, Time.deltaTime * EngineFlow);
                audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolumeOnMove * _volumeMultiplier, Time.deltaTime * EngineFlow);
                prevPitch = audioSource.pitch;
            }
            else if(motoController.isAirborne)
            {
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, 1.5f + motoController.engineSettings.currentGear*0.2f + engineRPM.Evaluate(1), Time.deltaTime * EngineFlow);
                audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolumeOnAirborne * _volumeMultiplier, Time.deltaTime * EngineFlow);
            }
            else
            {
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, 1 + motoController.engineSettings.currentGear*0.2f, Time.deltaTime * EngineFlow);
                audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolumeOnAirborne * _volumeMultiplier, Time.deltaTime * EngineFlow);
            }
        }

        private void UpdateVolume()
        {
            _volumeMultiplier =  AudioManager.Instance.GetAudioSettings.SfxVolume;
        }
    }
}
