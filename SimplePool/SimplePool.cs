using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.PoolSystem
{
    public interface ISimplePool
    {
        GameObject GetObject();
        T GetObject<T>();
        void AddObject();
        List<GameObject> GetAllObjects();
        void GetBackToPool(GameObject go);
    }

    public class SimplePool : ISimplePool
    {
        private Transform parent;
        private int poolSize;
        private GameObject prefab;
        private List<GameObject> storage;

        public SimplePool(GameObject prefab, int poolSize, Transform parent)
        {
            this.prefab = prefab;
            this.poolSize = poolSize;
            this.parent = parent;
            Init();
        }

        /// <summary>
        /// Pool Initialization
        /// </summary>
        private void Init()
        {
            storage = new List<GameObject>();

            if (poolSize <= 0) poolSize = 1;

            if (prefab != null && parent != null)
                CreateObjects();
            else
                Debug.LogError("Prefab or Parent should not be null.");
        }

        /// <summary>
        /// Creates Objects to Pool.
        /// </summary>
        private void CreateObjects()
        {
            for (int i = 0; i < poolSize; i++)
            {
                AddObject().SetActive(false);
            }
        }

        /// <summary>
        /// Adds new gameobject to Pool.
        /// </summary>
        /// <returns></returns>
        private GameObject AddObject()
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.SetParent(parent, false);
            storage.Add(go);
            return go;
        }

        /// <summary>
        /// Returns inactive gameobject from the Pool.
        /// </summary>
        /// <returns></returns>
        private GameObject GetObjectFromPool()
        {
            GameObject go = null;
            
            int count = storage.Count;

            for (int i = 0; i < count; i++)
            {
                if (!storage[i].activeInHierarchy)
                {
                    go = storage[i];
                    break;
                }
            }

            if (go == null)
            {
                GameObject newObject = AddObject();
                newObject.SetActive(true);
                return newObject;
            }

            go.SetActive(true);
            return go;
        }

        /// <summary>
        /// Returns Gameobject from Pool.
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            return GetObjectFromPool();
        }

        /// <summary>
        /// Returns Component of the object from the Pool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObject<T>()
        {
            return GetObjectFromPool().GetComponent<T>();
        }

        /// <summary>
        /// Adds new Gameobject to Pool.
        /// </summary>
        void ISimplePool.AddObject()
        {
            AddObject();
        }

        /// <summary>
        /// Return All the Pool Objects.
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetAllObjects()
        {
            return storage;
        }

        /// <summary>
        /// Throw gameobject to pool manually to use again.
        /// </summary>
        /// <param name="go"></param>
        public void GetBackToPool(GameObject go)
        {
            go.transform.SetParent(parent, false);
            go.SetActive(false);
        }
    }
}
