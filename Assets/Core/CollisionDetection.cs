using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class CollisionDetection : MonoBehaviour
    {
        public UnityAction<Collision> OnCollisionenter;
        public UnityAction<Collision2D> OnCollisionenter2D;

        private void OnCollisionEnter2D(Collision2D other) 
        {
            OnCollisionenter2D?.Invoke(other);
        }

        private void OnCollisionEnter(Collision other) 
        {
            OnCollisionenter?.Invoke(other);    
        }
    }
}
