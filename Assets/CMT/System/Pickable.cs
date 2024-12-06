using UnityEngine;

namespace CMT
{
    public class Pickable : MonoBehaviour
    {
        private float _initDepenetration;
        private float _delayDepenetration = 0.5f;
        private bool _isDelayDepenetrationExecute;

        private Collider _collider;
        private Rigidbody _rigidbody;
        private Transform _targetToFollow;

        [Header("Data")]
        [SerializeField] private string objectName;
        [SerializeField] private Sprite sprite;

        [Header("Outline")]
        [SerializeField] private Outline outline;

        public string ObjectName { get { return objectName; } }
        public Sprite Sprite { get { return sprite; } }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _initDepenetration = _rigidbody.maxDepenetrationVelocity;
            _rigidbody.maxDepenetrationVelocity = _initDepenetration * 0.25f;
        }

        public void PickUp(Transform targetToFollow)
        {
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _targetToFollow = targetToFollow;
        }

        public void EnableHighlight()
        {
            outline.Highlight();
        }

        public void DisableHighlight()
        {
            outline.UnHighlight();
        }

        private void Update()
        {
            if (_targetToFollow != null)
            {
                var diff = _targetToFollow.position - transform.position;
                var dir = diff.normalized;

                if (diff.sqrMagnitude > 0.1f * 0.1f)
                    transform.position += dir * Time.deltaTime;
            }

            if (!_isDelayDepenetrationExecute)
            {
                _delayDepenetration -= Time.deltaTime;
                if (_delayDepenetration <= 0f)
                {
                    _isDelayDepenetrationExecute = true;
                    _rigidbody.maxDepenetrationVelocity = _initDepenetration;
                }
            }
        }
    }
}
