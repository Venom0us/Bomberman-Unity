using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bomberman.SharedFiles.TilemapFiles
{
    [Serializable]
    public class TileLayerConfig<T> : TileLayerConfig
        where T : Enum
    {
        public T Tile;
        public override int Id { get { return Convert.ToInt32(Tile); } }
    }

    public abstract class TileLayerConfig
    {
        public virtual int Id { get; }
        public Color Color;
        public Sprite Sprite;
        public Tile.ColliderType ColliderType;
    }
}
