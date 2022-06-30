using System;

namespace Bomberman.SharedFiles.TilemapFiles
{
    internal class TileLayerConfigurationAttribute : Attribute
    {
        public TileGrid.Layer Layer;

        public TileLayerConfigurationAttribute(TileGrid.Layer layer)
        {
            Layer = layer;
        }
    }
}
