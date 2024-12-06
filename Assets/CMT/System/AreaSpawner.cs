using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace CMT
{
    public class AreaSpawner : MonoBehaviour
    {
        private Coroutine _coroutineSpawn;
        [SerializeField] private PoolSpawnId[] _objectToSpawn;
        [SerializeField] private float _radius;
        [SerializeField] private int _howManyObjects = 100;
        [SerializeField] private AudioClip _audioClipPop;

        public UnityAction OnFinishSpawn;

        private void Start()
        {
            if (_coroutineSpawn != null)
                StopCoroutine(_coroutineSpawn);

            _coroutineSpawn = StartCoroutine(SpawnDelay());
        }

        private IEnumerator SpawnDelay()
        {
            for (int i = 0; i < _howManyObjects; i++)
            {
                var index = Random.Range(0, _objectToSpawn.Length);
                var go = ObjectPool.Instance.Spawn(_objectToSpawn[index].id);
                go.transform.position = transform.position + Random.insideUnitSphere * _radius;
                yield return null;
            }
            OnFinishSpawn?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
