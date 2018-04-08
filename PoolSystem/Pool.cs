using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace Modules.PoolSystem
{
    public interface IPool
    {
        GameObject GetObject(string key);
        T GetObject<T>(string key);
        void AddObject(PoolObject poolObject);
        List<GameObject> GetObjectsTypeOf(string key);
        void GetBackToPool(GameObject go);
    }

    public class Pool : GenericSingleton<Pool>, IPool
    {
        [SerializeField]
        private List<PoolObject> objectsToPool;

        private int defaultPoolSize = 1;

        private Dictionary<string, List<GameObject>> storageCollection;

        protected override void Awake()
        {
            base.Awake();
            storageCollection = new Dictionary<string, List<GameObject>>();
        }

        private void Start()
        {
            InitializePool();
        }

        /// <summary>
        /// Initializing pool objects and they are in inactive
        /// </summary>
        private void InitializePool()
        {
            if (objectsToPool.Count == 0) return;

            for (int i = 0; i<objectsToPool.Count; i++)
            {
                CreateObjects(objectsToPool[i]);
            }
        }

        /// <summary>
        /// Creates objects to pool.
        /// </summary>
        /// <param name="poolObject"></param>
        private void CreateObjects(PoolObject poolObject)
        {
            if (poolObject.name != null && poolObject.gameObject != null)
            {
                storageCollection.Add(poolObject.name, new List<GameObject>());

                if (poolObject.poolSize <= 0)
                    poolObject.poolSize = defaultPoolSize;

                for (int j = 0; j < poolObject.poolSize; j++)
                {
                    GameObject go = Instantiate(poolObject.gameObject) as GameObject;
                    storageCollection[poolObject.name].Add(go);
                    go.transform.SetParent(transform, false);
                    go.SetActive(false);
                }
               
            }
            else
                Debug.LogError("PoolObject name or Prefab reference should not null");
        }

        /// <summary>
        /// Adding new object if they are no objects to return
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns name = "GameObject"></returns>
        private GameObject AddNewObjectToPool(string objectName)
        {
            PoolObject newObject = objectsToPool.Find(x => x.name.Equals(objectName));

            GameObject go = Instantiate(newObject.gameObject);

            storageCollection[objectName].Add(go);

            go.transform.SetParent(transform, false);
            go.SetActive(true);
            return go;
        }

        /// <summary>
        /// Returns the gameobject which you want by object name you provided in the intialization
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns name = "GameObject"></returns>
        private GameObject GetObjectFromPool(string objectName)
        {
            if (storageCollection.ContainsKey(objectName))
            {
                List<GameObject> pooledObjects = storageCollection[objectName];
                int count = pooledObjects.Count;

                GameObject go = null;

                for (int i = 0; i < count; i++)
                {
                    if (!pooledObjects[i].activeInHierarchy)
                    {
                        go = pooledObjects[i];
                        break;
                    }
                }

                if (go == null)
                {
                    GameObject newObject = AddNewObjectToPool(objectName);
                    return newObject;
                }

                go.SetActive(true);
                return go;
            }
            else
            {
                Debug.Log("Given object name can not found in the collection " + objectName);
            }

            return null;
        }

        /// <summary>
        /// Returns Gameobject with specified key value from the PoolSystem.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject GetObject(string key)
        {
            return GetObjectFromPool(key);
        }

        /// <summary>
        /// Return Required component of the Object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetObject<T>(string key)
        {
            return GetObjectFromPool(key).GetComponent<T>();
        }

        /// <summary>
        /// Adds new Gameobject to PoolSystem. It requires object typeof(PoolObject) instance.
        /// </summary>
        /// <param name="poolObject"></param>
        public void AddObject(PoolObject poolObject)
        {
            if(!storageCollection.ContainsKey(poolObject.name))
            {
                objectsToPool.Add(poolObject);
                CreateObjects(objectsToPool[objectsToPool.Count - 1]);
            }
            else
            {
                Debug.LogError(string.Format("Given PoolObject has exist with same key {0}", poolObject.name));
            }
        }

        /// <summary>
        /// Returns List of Gameobject with type of specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<GameObject> GetObjectsTypeOf(string key)
        {
            if(storageCollection.ContainsKey(key))
            {
                return storageCollection[key];
            }
            else
                Debug.LogError(string.Format("Given key doesn't exist in the pool {0}", key));

            return null;
        }

        /// <summary>
        /// Throw gameobject to pool manually to use again.
        /// </summary>
        /// <param name="go"></param>
        public void GetBackToPool(GameObject go)
        {
            go.transform.SetParent(transform, false);
            go.SetActive(false);
        }
    }
}
