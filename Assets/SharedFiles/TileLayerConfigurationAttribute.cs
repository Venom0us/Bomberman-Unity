using System;

namespace Bomberman.SharedFiles
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
