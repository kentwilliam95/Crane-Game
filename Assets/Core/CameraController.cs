using UnityEngine;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController _instance;
        public static CameraController Instance => _instance;

        [field: SerializeField] public Camera mainCamera { get; private set; }
        [field: SerializeField] public Camera uiCamera { get; private set; }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            bool isLandscape = (float)Screen.width / (float)Screen.height > 1;
            if(isLandscape)
            {
                mainCamera.fieldOfView = 50;
            }
            else
            {
                mainCamera.fieldOfView = 70;
            }
        }
    }
}
