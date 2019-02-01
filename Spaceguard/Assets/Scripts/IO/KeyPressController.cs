using UnityEngine;
using Game.Cameras;
using Game.Bodies;

namespace Game.IO
{
    [AddComponentMenu("Custom IO/Key Press Controller")]
    public class KeyPressController : MonoBehaviour
    {
        public static BodyManager bodyManager;
        public static CameraController cameraController;

        void Awake()
        {
            if(bodyManager == null)
            {
                bodyManager = FindObjectOfType<BodyManager>();
                if (bodyManager == null)
                    Debug.LogError("KeyPressController unable to find BodyManager");
            }

            if(cameraController == null)
            {
                cameraController = FindObjectOfType<CameraController>();
                if (cameraController == null)
                    Debug.LogError("KeyPressController unable to find CameraController");
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Press 'P' to Pause/Unpause
            if (Input.GetKeyDown(KeyCode.P))
            {
                bodyManager.IsPaused = !bodyManager.IsPaused;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
            }
        }
    }
}
