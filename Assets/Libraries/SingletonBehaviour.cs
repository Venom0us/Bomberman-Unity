using UnityEngine;

namespace Bomberman.Libraries
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
                Instance = (T)this;
            }
            else
            {
                Instance = (T)this;
            }
        }
    }
}
