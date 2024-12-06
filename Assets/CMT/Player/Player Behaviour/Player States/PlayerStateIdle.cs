using Core;
using UnityEngine;

namespace CMT
{
    public struct PlayerStateIdle : IState<PlayerController>
    {
        private Pickable _pickedObject;
        private PlayerController _playerController;
        public int AnimationID;

        public void OnStateEnter(PlayerController t)
        {
            //if (_playerController == null)
                _playerController = t;

            if(TouchController.Instance.onFire == null)
                TouchController.Instance.onFire = TouchController_OnFire;
        }

        public void OnStateExit(PlayerController t)
        {
            //TouchController.Instance.onFire = null;
        }

        public void OnStateUpdate(PlayerController t)
        {
            t.Rigidbody.velocity = TouchController.velocity3D;
            
            if(t.Rigidbody.velocity.sqrMagnitude > 0)
                t.PlaySoundCraneMove();
            else
                t.StopSoundCrandeMove();

            int hitCount = Physics.RaycastNonAlloc(t.RaycastTransform.position, Vector3.down, t.HitResult, 10f, t.LayerGameObjects);
            for (int i = 0; i < hitCount; i++)
            {
                t.HitResult[i].collider.TryGetComponent<Pickable>(out Pickable target);
                if (target)
                {
                    if(_pickedObject)
                        _pickedObject.DisableHighlight();

                    _pickedObject = target;
                    _pickedObject.EnableHighlight();
                    break;
                }
            }    
        }

        private void TouchController_OnFire()
        {
            if (_pickedObject)
                _pickedObject.DisableHighlight();

            _playerController.StopSoundCrandeMove();
            _playerController.ChangeState(PlayerController.State.Grab);
        }
    }
}