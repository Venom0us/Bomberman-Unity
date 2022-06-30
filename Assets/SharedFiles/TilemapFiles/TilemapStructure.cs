using Bomberman.SharedFiles.GenerationFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bomberman.SharedFiles.TilemapFiles
{
    public class TilemapStructure : MonoBehaviour
    {
        public TileGrid Grid { get; set; }
        internal TileGrid.Layer Layer
        {
            get { return _layer; }
        }

        public int Width { get { return Grid.Width; } }
        public int Height { get { return Grid.Height; } }

        private int[] _cells;
        private Tilemap _mapGraphic;

        private HashSet<Vector3Int> _dirtyCells;

        [SerializeField]
        private TileGrid.Layer _layer;
        [SerializeField]
        private Generator[] _generators;

        public void Initialize()
        {
            Grid = transform.parent.GetComponent<TileGrid>();
            _mapGraphic = GetComponent<Tilemap>();
            _cells = new int[Width * Height];
            _dirtyCells = new HashSet<Vector3Int>();

            foreach (var generator in _generators)
                generator.Generate(this);
            UpdateTiles(false);
        }

        internal void UpdateTile(int x, int y)
        {
            if (!InBounds(x, y)) return;

            var pos = new Vector3Int(x, y);
            
            if (_dirtyCells.Contains(pos))
                _dirtyCells.Remove(pos);

            _mapGraphic.SetTile(pos, Grid.GetTileGraphic(Layer, GetTile(x, y)));
        }

        internal void UpdateTile(Vector2Int pos)
        {
            UpdateTile(pos.x, pos.y);
        }

        internal void UpdateTiles(bool onlyDirtyCells = true)
        {
            Vector3Int[] positionsArray;
            Tile[] tilesArray;

            if (onlyDirtyCells)
            {
                positionsArray = new Vector3Int[_dirtyCells.Count];
                tilesArray = new Tile[_dirtyCells.Count];

                int i = 0;
                foreach (var pos in _dirtyCells)
                {
                    positionsArray[i] = pos;
                    tilesArray[i] = Grid.GetTileGraphic(Layer, GetTile((Vector2Int)pos));
                    i++;
                }
            }
            else
            {
                positionsArray = new Vector3Int[Width * Height];
                tilesArray = new Tile[Width * Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        positionsArray[x * Width + y] = new Vector3Int(x, y, 0);
                        tilesArray[x * Width + y] = Grid.GetTileGraphic(Layer, GetTile(x, y));
                    }
                }
            }

            // Clear all dirty coordinates
            _dirtyCells.Clear();
            _mapGraphic.SetTiles(positionsArray, tilesArray);
        }

        internal void SetTiles(Vector2Int[] positions, int[] tiles, bool updateGraphic = false, bool setDirty = true)
        {
            for (int i=0; i < positions.Length; i++)
                SetTile(positions[i], tiles[i], false, false);

            if (!updateGraphic && setDirty)
            {
                foreach (var pos in positions)
                    _dirtyCells.Add(new Vector3Int(pos.x, pos.y));
            }
            else if (updateGraphic)
            {
                foreach (var pos in positions)
                    _dirtyCells.Remove(new Vector3Int(pos.x, pos.y));

                _mapGraphic.SetTiles(positions
                    .Select(a => new Vector3Int(a.x, a.y))
                    .ToArray(), tiles
                    .Select(a => Grid.GetTileGraphic(Layer, a))
                    .ToArray());
            }
        }

        internal void SetTile(int x, int y, int tileId, bool updateGraphic = false, bool setDirty = true)
        {
            if (!InBounds(x, y)) return;

            _cells[y * Width + x] = tileId;

            if (!updateGraphic && setDirty)
            {
                _dirtyCells.Add(new Vector3Int(x, y));
            }
            else if (updateGraphic)
            {
                _dirtyCells.Remove(new Vector3Int(x, y));
                _mapGraphic.SetTile(new Vector3Int(x, y), Grid.GetTileGraphic(Layer, tileId));
            }
        }

        internal void SetTile(Vector2Int pos, int tileId, bool updateGraphic = false, bool setDirty = true)
        {
            SetTile(pos.x, pos.y, tileId, updateGraphic, setDirty);
        }

        internal void SetTile<T>(int x, int y, T tile, bool updateGraphic = false, bool setDirty = true) where T : Enum
        {
            SetTile(x, y, Convert.ToInt32(tile), updateGraphic, setDirty);
        }

        internal void SetTile<T>(Vector2Int pos, T tile, bool updateGraphic = false, bool setDirty = true) where T : Enum
        {
            SetTile(pos.x, pos.y, Convert.ToInt32(tile), updateGraphic, setDirty);
        }

        internal int GetTile(int x, int y)
        {
            if (!InBounds(x, y)) return 0;
            return _cells[y * Width + x];
        }

        internal int GetTile(Vector2Int pos)
        {
            return GetTile(pos.x, pos.y);
        }

        internal bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        internal bool InBounds(Vector2Int pos)
        {
            return InBounds(pos.x, pos.y);
        }
    }
}
