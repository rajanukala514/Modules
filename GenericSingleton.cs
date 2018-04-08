using System;
using UnityEngine;

namespace Singleton
{
    public class GenericSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        [SerializeField]
        private bool dontDestroyOnLoad;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;

                if(dontDestroyOnLoad)
                    DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
