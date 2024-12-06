using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class TouchController : MonoBehaviour
    {
        private static TouchController instance;
        public static TouchController Instance => instance;
        public static bool isInputEnable;
        public static Vector2 velocity2D;
        public static Vector3 velocity3D;
        public static bool isFire;

        private bool _isThumbstickMode;
        [SerializeField] private ThumbstickController thumbstickController;
        public Canvas canvas;
        public UnityAction onFire;

        private void Awake()
        {
            instance = this;
            thumbstickController.Thumbstick_OnClick = Thumbstick_OnClick;
        }

        private void OnDestroy()
        {
            thumbstickController.Thumbstick_OnClick = null;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (!isInputEnable)
            {
                velocity2D = Vector2.zero;
                velocity3D = Vector3.zero;
                isFire = false;
                thumbstickController.SetVisibility(false);
            }
            else
            {
                velocity2D.x = Input.GetAxis("Horizontal");
                velocity2D.y = Input.GetAxis("Vertical");

                _isThumbstickMode = velocity2D.sqrMagnitude <= 0 && Input.GetMouseButton(0);
                thumbstickController.SetVisibility(_isThumbstickMode);
                if (_isThumbstickMode)
                {
                    velocity2D = thumbstickController.Velocity;
                }
                else
                {
                    isFire = Input.GetAxis("Fire1") > 0 || Input.GetKeyDown(KeyCode.Space);
                }

                if (isFire)
                    onFire?.Invoke();

                velocity3D.x = velocity2D.x;
                velocity3D.z = velocity2D.y;
            }
        }

        private void Thumbstick_OnClick()
        {
            if (isInputEnable && _isThumbstickMode)
                onFire?.Invoke();
        }

        public void DisableInput()
        {
            canvas.enabled = true;
            isInputEnable = false;
        }

        public void EnableInput()
        {
            canvas.enabled = false;
            isInputEnable = true;
        }
    }
}