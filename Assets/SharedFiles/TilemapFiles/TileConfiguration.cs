using System;

namespace Bomberman.SharedFiles.TilemapFiles
{
    [Serializable]
    public class TileConfiguration
    {
        [TileLayerConfiguration(TileGrid.Layer.World)]
        public TileLayerConfig<TileLayerTypes.WorldTiles>[] WorldTiles;

        [TileLayerConfiguration(TileGrid.Layer.Object)]
        public TileLayerConfig<TileLayerTypes.ObjectTiles>[] ObjectTiles;
    }
}
