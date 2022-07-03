using Bomberman.Libraries;
using UnityEngine;

namespace Bomberman.ClientFiles
{
    public class PrefabContainer : SingletonBehaviour<PrefabContainer>
    {
        [SerializeField]
        private Bomberman _bomberman;
        public Bomberman Bomberman { get { return _bomberman; } }
    }
}
