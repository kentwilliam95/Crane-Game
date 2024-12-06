using Core;
using UnityEngine;
using UnityEngine.Events;

namespace CMT
{
    public struct PlayerStateRelease : IState<PlayerController>
    {
        private float _lerpProgress;
        private Vector3 _initPosition;
        private Vector3 _finalPosition;
        public int AnimationID;
        public UnityAction OnRelease;

        public void OnStateEnter(PlayerController t)
        {
            t.PlaySoundCraneMove();
            _lerpProgress = 0f;
            _initPosition = t.transform.localPosition;
            _finalPosition = _initPosition;
            _finalPosition.y = 2f; 
        }

        public void OnStateExit(PlayerController t)
        {

        }

        public void OnStateUpdate(PlayerController t)
        {
            _lerpProgress += Time.deltaTime;
            t.transform.localPosition = Vector3.Lerp(_initPosition, _finalPosition, _lerpProgress);

            if (_lerpProgress >= 1f)
            {
                t.Animator.CrossFade(AnimationID, 0.25f);
                OnRelease?.Invoke();
                t.ChangeState(PlayerController.State.Idle);
            }
        }
    }
}