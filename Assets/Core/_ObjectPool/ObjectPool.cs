using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public struct SpawnInfo
        {
            public PoolSpawnId go;
        }

        public List<SpawnInfo> spawnInfoes = new List<SpawnInfo>();
        public Dictionary<string, Pool> dict = new Dictionary<string, Pool>();

        private static ObjectPool instance;
        public static ObjectPool Instance => instance;

        private void Awake()
        {
            instance = this;
            for (int i = 0; i < spawnInfoes.Count; i++)
            {
                var d = spawnInfoes[i];
                Pool pool = new Pool(d.go.gameObject);
                pool.OnCreate = Pool_OnCreate;
                pool.OnUnspawn = Pool_OnUnspawn;
                pool.OnSpawn = Pool_OnSpawn;
                pool.OnDestroy = Pool_OnDestroy;
                dict.Add(d.go.id, pool);
            }
        }

        public GameObject Spawn(string name)
        {
            GameObject res = null;
            if (dict.ContainsKey(name))
            {
                res = dict[name].Spawn();
            }
            return res;
        }

        public void UnSpawn(GameObject go)
        {
            var name = go.name;
            dict[name].UnSpawn(go);
        }

        private GameObject Pool_OnCreate(GameObject goTemplate)
        {
            var go = Instantiate(goTemplate);
            go.SetActive(true);
            go.name = goTemplate.name;
            go.transform.parent = transform;
            return go;
        }

        private void Pool_OnUnspawn(GameObject goTemplate)
        {
            goTemplate.SetActive(false);
        }

        private void Pool_OnSpawn(GameObject goTemplate)
        {
            goTemplate.SetActive(true);
        }

        private void Pool_OnDestroy(GameObject go)
        {
            Destroy(go);
        }

        public void Flush()
        {
            foreach (var item in dict)
            {
                item.Value.Flush();
            }
        }
    }

    public class Pool
    {
        private List<GameObject> activeObject;
        private Stack<GameObject> unActiveObject;

        public System.Action<GameObject> OnSpawn;
        public System.Func<GameObject, GameObject> OnCreate;
        public System.Action<GameObject> OnUnspawn;
        public System.Action<GameObject> OnDestroy;

        public GameObject go;
        public Pool(GameObject go)
        {
            this.go = go;
            activeObject = new List<GameObject>();
            unActiveObject = new Stack<GameObject>();
        }

        public GameObject Spawn()
        {
            GameObject activeGo;
            if (unActiveObject.Count > 0)
            {
                var disableObject = unActiveObject.Pop();
                OnSpawn?.Invoke(disableObject);
                activeObject.Add(disableObject);
                activeGo = disableObject;
            }
            else
            {
                var newGO = OnCreate?.Invoke(go);
                activeObject.Add(newGO);
                activeGo = newGO;
            }
            return activeGo;
        }

        public void UnSpawn(GameObject inActiveGO)
        {
            OnUnspawn?.Invoke(inActiveGO);
            unActiveObject.Push(inActiveGO);
            activeObject.Remove(inActiveGO);
        }

        public void Flush()
        {
            var length = activeObject.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                var go = activeObject[i].gameObject;

                if (go)
                    OnDestroy.Invoke(go);
                activeObject.RemoveAt(i);
            }

            activeObject.Clear();

            length = unActiveObject.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                var go = unActiveObject.Pop();

                if (go)
                    OnDestroy.Invoke(go);
            }

            unActiveObject.Clear();
        }
    }
}
