using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bomberman.SharedFiles.TilemapFiles
{
    public class TileGrid : MonoBehaviour
    {
        public int Width, Height, TileSize;
        public TileConfiguration TileConfiguration;

        private Dictionary<Layer, TilemapStructure> _tilemaps;
        private Dictionary<Layer, Dictionary<int, Tile>> _tileCache;

        private void Awake()
        {
            InitializeTilemaps();
            InitializeTileCache();

            // Initialize the tilemaps in order by the layer numbers
            foreach (var tilemap in _tilemaps.OrderBy(a => a.Key))
                tilemap.Value.Initialize();
        }

        private void InitializeTileCache()
        {
            _tileCache = new Dictionary<Layer, Dictionary<int, Tile>>();
            foreach (var layer in _tilemaps.Keys)
            {
                var tileCache = new Dictionary<int, Tile>();
                PopulateTileCache(layer, tileCache);
                _tileCache.Add(layer, tileCache);
            }
        }

        private void PopulateTileCache(Layer layer, Dictionary<int, Tile> tileCache)
        {
            var tileConfigType = TileConfiguration.GetType();
            var fields = tileConfigType.GetFields();
            var layerField = fields.FirstOrDefault(x => x.GetCustomAttribute<TileLayerConfigurationAttribute>(false).Layer == layer);
            var tileConfigurations = (TileLayerConfig[])layerField.GetValue(TileConfiguration);
            Type genericType = null;
            foreach (var configuration in tileConfigurations)
            {
                var tile = ScriptableObject.CreateInstance<Tile>();
                if (configuration.Sprite == null)
                {
                    tile.sprite = Sprite.Create(new Texture2D(TileSize, TileSize), new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
                    tile.color = new Color(configuration.Color.r, configuration.Color.g, configuration.Color.b, 1);
                }
                else
                {
                    tile.sprite = configuration.Sprite;
                }
                
                tile.colliderType = configuration.ColliderType;
                if (genericType == null)
                    genericType = configuration.GetType().GenericTypeArguments[0];
                tile.name = Enum.ToObject(genericType, configuration.Id).ToString();
                tileCache.Add(configuration.Id, tile);
            }
        }

        private void InitializeTilemaps()
        {
            _tilemaps = new Dictionary<Layer, TilemapStructure>();

            foreach (Transform child in transform)
            {
                var tilemap = child.GetComponent<TilemapStructure>();
                if (tilemap != null)
                    _tilemaps.Add(tilemap.Layer, tilemap);
            }
        }

        /// <summary>
        /// Returns the tilemap layer or null if not defined.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        internal TilemapStructure GetTilemap(Layer layer)
        {
            _tilemaps.TryGetValue(layer, out TilemapStructure structure);
            return structure;
        }

        internal Tile GetTileGraphic(Layer layer, int tileId)
        {
            if (!_tileCache.TryGetValue(layer, out var tiles)) return null;
            tiles.TryGetValue(tileId, out var tileGraphic);
            return tileGraphic;
        }

        public enum Layer
        {
            World,
            Object
        }
    }
}
