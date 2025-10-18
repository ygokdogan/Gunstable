using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Utilities
{
    public class CameraShake : MonoBehaviour
    {
        private CinemachineCamera cam;
        private static CinemachineBasicMultiChannelPerlin _noise;
        private static float _shakeTimer;

        private void Awake()
        {
            cam = GetComponent<CinemachineCamera>();
            _noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
        
        private void Update()
        {
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.unscaledDeltaTime;
                if (_shakeTimer <= 0f)
                {
                    _noise.AmplitudeGain = 0f;
                }
            }
        }

        public static void Shake(float intensity, float duration)
        {
            _noise.AmplitudeGain = intensity;
            _shakeTimer = duration;
        }
    }
}
