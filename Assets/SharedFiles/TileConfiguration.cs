using System;

namespace Bomberman.SharedFiles
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
