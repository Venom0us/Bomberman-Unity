using Bomberman.SharedFiles.TilemapFiles;
using UnityEngine;

namespace Bomberman.SharedFiles.GenerationFiles
{
    public abstract class Generator : ScriptableObject
    {
        public abstract void Generate(TilemapStructure tilemapStructure);
    }
}
