using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core
{
    public class ThumbstickController : MonoBehaviour
    {
        private Vector2 _startPosition;
        private Vector2 _initPosition;
        private Vector2 _direction;
        private float _power;
        private float _clickCounter;
        private bool _isPointerDown;
        private bool _isClick;

        private bool _isDrag;
        [SerializeField] private RectTransform _overlay;
        [SerializeField] private Image _container;
        [SerializeField] private Image _center;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _radius;

        public UnityAction Thumbstick_OnDrag;
        public UnityAction Thumbstick_OnPointerEnter;
        public UnityAction Thumbstick_OnPointerExit;
        public UnityAction Thumbstick_OnClick;

        public Vector3 Direction { get { return _direction; } }
        public float Power { get { return _power; } }
        public Vector2 Velocity { get { return _direction * _power; } }
        public bool IsClick { get { return _isClick; } }
        private void Start()
        {
            _startPosition = _center.rectTransform.localPosition;
        }

        private void LateUpdate()
        {
            _isClick = false;
            if (_isDrag)
                Thumbstick_OnDrag?.Invoke();

            if (_isPointerDown)
            {
                _clickCounter += Time.deltaTime;
            }
        }

        public void SetVisibility(bool isVisible)
        {
            _container.enabled = isVisible;
            _center.enabled = isVisible;
        }

        public void ShowInput()
        {
            _container.enabled = true;
            _center.enabled = true;
        }

        public void HideInput()
        {
            _container.enabled = true;
            _center.enabled = true;
        }

        public void OnPointerEnter(BaseEventData evt)
        {
            var mp = (PointerEventData)evt;
            var ml = _overlay.InverseTransformPoint(mp.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_overlay, mp.position, _canvas.worldCamera, out Vector2 newPos);
            _initPosition = newPos;
            _container.rectTransform.localPosition = newPos;
            _center.rectTransform.localPosition = newPos;

            _isPointerDown = true;
            _power = 0f;
            _direction = Vector2.zero;
            Thumbstick_OnPointerEnter?.Invoke();
        }

        public void OnDrag(BaseEventData evt)
        {
            _isDrag = true;
            var mp = (PointerEventData)evt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_overlay, mp.position, _canvas.worldCamera, out Vector2 newPos);

            var rect = _overlay.rect;
            if (!rect.Contains(newPos))
                return;

            var diff = (_initPosition - newPos);
            var distance = diff.magnitude;
            _direction = -diff.normalized;

            _center.rectTransform.localPosition = newPos;
            if (distance >= _radius)
            {
                _initPosition = newPos - _direction * _radius;
                _container.rectTransform.localPosition = _initPosition;
            }

            _power = distance / _radius;
        }

        public void OnPointerExit(BaseEventData evt)
        {
            var mp = (PointerEventData)evt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_overlay, mp.position, _canvas.worldCamera, out Vector2 newPos);

            _isDrag = false;
            _container.rectTransform.localPosition = newPos;
            _center.rectTransform.localPosition = newPos;

            _power = 0f;
            _direction = Vector2.zero;
            Thumbstick_OnPointerExit?.Invoke();

            if (_clickCounter <= 0.2f)
            {
                Thumbstick_OnClick?.Invoke();
                _isClick = true;
            }                
            
            _clickCounter = 0;
            _isPointerDown = false;
        }
    }
}
