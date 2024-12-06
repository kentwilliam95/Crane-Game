using Core;
using UnityEngine;
using UnityEngine.Events;

namespace CMT
{
    public struct PlayerStateGrab : IState<PlayerController>
    {
        private float _delayAfterAnimation;
        private int _layerMaskGameObject;
        private bool _isPlayAnimation;
        private bool _isFoundTarget;
        private Pickable _selectedTarget;

        public int AnimationID;
        public UnityAction<Pickable> OnGrabItem;
        public UnityAction OnNotGrabItem;

        public void OnStateEnter(PlayerController t)
        {               
            t.PlaySoundCraneMove();
            t.Rigidbody.velocity = Vector3.zero;
            t.Rigidbody.constraints = RigidbodyConstraints.FreezePosition;
            
            _delayAfterAnimation = 1f;
            t.EnableCraneCollider(false);
            _selectedTarget = null;
        }

        public void OnStateExit(PlayerController t)
        {
            t.Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        }

        public void OnStateUpdate(PlayerController t)
        {
            if (_isFoundTarget)
            {
                if(!_isPlayAnimation)
                {
                    t.Animator.CrossFade(AnimationID, 0.25f);
                    t.StopSoundCrandeMove();
                    _isPlayAnimation = true;

                    if(_selectedTarget == null)
                        OnNotGrabItem?.Invoke();
                }   
                else
                {
                    _delayAfterAnimation -= Time.deltaTime;
                    if(_delayAfterAnimation <= 0f)
                    {
                        t.ChangeState(PlayerController.State.Release);
                    }
                }             
                return;
            }
            
            t.transform.Translate(Vector3.down * Time.deltaTime);

            int hitCount = Physics.RaycastNonAlloc(t.RaycastTransform.position, Vector3.down, t.HitResult, 0.1f, t.LayerGameObjects);
            for (int i = 0; i < hitCount; i++)
            {
                t.HitResult[i].collider.TryGetComponent<Pickable>(out Pickable pickable);
                if (pickable)
                {
                    _isFoundTarget = true;
                    t.EnableCraneCollider(true);
                    _selectedTarget = pickable;
                    pickable.PickUp(t.RaycastTransform);
                    OnGrabItem?.Invoke(pickable);
                    t.PlayCraneCloseSound();
                    break;
                }

                t.HitResult[i].collider.TryGetComponent<Ground>(out Ground ground);
                if(ground)
                {
                    _isFoundTarget = true;
                }
            }  
        }
    }
}