using Core;
using UnityEngine;
using UnityEngine.Events;

namespace CMT
{
    public class PlayerController : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Grab,
            Release
        }

        private PlayerStateIdle _stateIdle;
        private PlayerStateGrab _stateGrab;
        private PlayerStateRelease _stateRelease;

        private Pickable _pickedItem;

        [SerializeField] private CollisionDetection _collisionDetection;
        [SerializeField] private Collider[] _craneCollider;

        [Header("Animations")]
        [SerializeField] private AnimationClip _animationClipIdle;
        [SerializeField] private AnimationClip _animationClipGrab;
        [SerializeField] private AnimationClip _animationClipRelease;

        [Header("Sound")]
        [SerializeField] private AudioSource _audioCraneMove;
        [SerializeField] private AudioSource _audioCraneClose;

        public StateMachine<PlayerController> StateMachine { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody2 { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Transform RaycastTransform { get; private set; }
        public float AnimationGrabDuration { get { return _animationClipGrab.length; } }
        public RaycastHit[] HitResult { get; private set; }
        public int LayerGameObjects { get; private set; }

        public UnityAction<Pickable> GotAnItem;
        public UnityAction NotGetAnyItem;

        private void Start()
        {
            _collisionDetection.OnCollisionenter = CollisionDetection_OnCollideWithPickableObject;
            HitResult = new RaycastHit[8];
            LayerGameObjects = LayerMask.GetMask("GameObject");

            StateMachine = new StateMachine<PlayerController>(this);
            StateMachine.ChangeState(new PlayerStateIdle());

            _stateIdle = new PlayerStateIdle() { AnimationID = Animator.StringToHash("Idle") };
            _stateRelease = new PlayerStateRelease() { AnimationID = Animator.StringToHash("Release") };
            _stateGrab = new PlayerStateGrab() { AnimationID = Animator.StringToHash("Grab") };
            _stateGrab.OnGrabItem = StateGrab_OnGrabItem;
            _stateGrab.OnNotGrabItem = StateGrab_NotGrabItem;
            _stateRelease.OnRelease = StateRelease_OnRelease;
        }

        private void Update()
        {
            StateMachine.OnUpdate();
        }

        private void OnDestroy()
        {
            _collisionDetection.OnCollisionenter = null;
        }

        public void ChangeState(IState<PlayerController> state)
        {
            StateMachine.ChangeState(state);
        }

        public void ChangeState(State state)
        {
            switch (state)
            {
                case State.Idle:
                    ChangeState(_stateIdle);
                    break;

                case State.Grab:
                    ChangeState(_stateGrab);
                    break;

                case State.Release:
                    ChangeState(_stateRelease);
                    break;
            }
        }

        public void EnableCraneCollider(bool isEnable)
        {
            for (int i = 0; i < _craneCollider.Length; i++)
            {
                _craneCollider[i].enabled = isEnable;
            }
        }

        public void PlaySoundCraneMove()
        {
            if(!_audioCraneMove.isPlaying)
                _audioCraneMove.Play();
        }

        public void StopSoundCrandeMove()
        {
            _audioCraneMove.Stop();
        }

        public void PlayCraneCloseSound()
        {
            if(!_audioCraneClose.isPlaying)
                _audioCraneClose.Play();
        }

        public void StopCraneCloseSound()
        {
            _audioCraneClose.Stop();
        }

        private void StateGrab_OnGrabItem(Pickable item)
        {
            _pickedItem = item;
        }

        private void StateGrab_NotGrabItem()
        {
            NotGetAnyItem?.Invoke();
        }

        private void StateRelease_OnRelease()
        {
            if(_pickedItem)
                GotAnItem?.Invoke(_pickedItem);
            
            _pickedItem = null;
        }

        private void CollisionDetection_OnCollideWithPickableObject(Collision other)
        {

        }
    }
}