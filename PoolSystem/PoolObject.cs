using System;
using UnityEngine;

namespace Modules.PoolSystem
{
    [Serializable]
    public sealed class PoolObject
    {
        public string name;
        public int poolSize;
        public GameObject gameObject;
    }
}
