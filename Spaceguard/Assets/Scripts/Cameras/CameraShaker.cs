using UnityEngine;

namespace Game.Cameras
{
    [AddComponentMenu("Custom Camera/Camera Shaker")]
    /// <summary>
    /// Camera shake effect script.
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {
        public float power = 0.7f;
        public float duration = 1.0f;
        public Transform mainCamera;
        public float slowDownAmount = 1.0f;
        public bool shouldShake = false;

        Vector3 startPosition;
        float initialDuration;

        void Start()
        {
            mainCamera = Camera.main.transform;
            startPosition = mainCamera.localPosition;
            initialDuration = duration;
        }

        void Update()
        {
            if (shouldShake)
            {
                if (duration > 0)
                {
                    mainCamera.localPosition = startPosition + Random.insideUnitSphere * power;
                }
                else
                {
                    shouldShake = false;
                    duration = initialDuration;
                    mainCamera.localPosition = startPosition;

                }
            }
        }
    }
}