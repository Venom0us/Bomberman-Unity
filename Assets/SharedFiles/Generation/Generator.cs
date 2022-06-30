using System;
using UnityEngine;

namespace Bomberman.SharedFiles.Generation
{
    public abstract class Generator : ScriptableObject
    {
        public abstract void Generate(TilemapStructure tilemapStructure);
    }
}
