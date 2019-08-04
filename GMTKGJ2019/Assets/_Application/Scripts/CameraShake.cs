using UnityEngine;
using DG.Tweening;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(Camera))]
    public class CameraShake : MonoBehaviour
    {
        private new Camera camera;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        public void TriggerShake()
        {
            GameParameters parameters = GameParameters.Instance;
            camera.DOShakePosition(
                parameters.CameraShakeDuration,
                parameters.CameraShakeStrength,
                parameters.CameraShakeVibrato,
                parameters.CameraShakeRandomness);
        }
    }
}