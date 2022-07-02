using Bomberman.Libraries;
using System;
using System.Threading;

namespace Bomberman.ClientFiles
{
    public class UnityThread : SingletonBehaviour<UnityThread>
    {
        private SynchronizationContext _current;

        protected override void Awake()
        {
            base.Awake();
            _current = SynchronizationContext.Current;
            DontDestroyOnLoad(gameObject);
        }

        public void Execute(Action action)
        {
            _current.Post((object state) => { action(); }, this);
        }
    }
}
