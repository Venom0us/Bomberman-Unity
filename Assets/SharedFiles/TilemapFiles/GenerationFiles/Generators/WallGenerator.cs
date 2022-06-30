using Bomberman.SharedFiles.TilemapFiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bomberman.SharedFiles.GenerationFiles.Generators
{
    [CreateAssetMenu(menuName = "Map/Generators/WallGenerator")]
    public class WallGenerator : Generator
    {
        public TileLayerTypes.ObjectTiles WallTile;

        public override void Generate(TilemapStructure tilemapStructure)
        {
            var worldMap = tilemapStructure.Grid.GetTilemap(TileGrid.Layer.World);
            var spawnableRange = GetSpawnableRangePositions(tilemapStructure.Grid.Width, tilemapStructure.Grid.Height);
            for (int x = 0; x < tilemapStructure.Width; x++)
            {
                for (int y = 0; y < tilemapStructure.Height; y++)
                {
                    if (worldMap.GetTile(x, y) != (int)TileLayerTypes.WorldTiles.Ground)
                        continue;

                    if (!spawnableRange.Contains(new Vector2Int(x, y)))
                        tilemapStructure.SetTile(x, y, WallTile, false, false);
                }
            }
        }

        private HashSet<Vector2Int> GetSpawnableRangePositions(int width, int height)
        {
            // All corners
            var bottomLeft = new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(1, 2) };
            var bottomRight = new Vector2Int[] { new Vector2Int(width - 2, 1), new Vector2Int(width - 3, 1), new Vector2Int(width - 2, 2) };
            var topLeft = new Vector2Int[] { new Vector2Int(1, height - 2), new Vector2Int(2, height - 2), new Vector2Int(1, height - 3) };
            var topRight = new Vector2Int[] { new Vector2Int(width - 2, height - 2), new Vector2Int(width - 3, height - 2), new Vector2Int(width - 2, height - 3) };

            // Middle of all sides
            var bottom = new Vector2Int[] { new Vector2Int((width / 2) - 1, 1), new Vector2Int(width / 2, 1), new Vector2Int((width / 2) + 1, 1) };
            var top = new Vector2Int[] { new Vector2Int((width / 2) - 1, height - 2), new Vector2Int(width / 2, height - 2), new Vector2Int((width / 2) + 1, height - 2) };
            var left = new Vector2Int[] { new Vector2Int(1, (height / 2) - 1), new Vector2Int(1, height / 2), new Vector2Int(1, (height / 2) + 1) };
            var right = new Vector2Int[] { new Vector2Int(width - 2, (height / 2) - 1), new Vector2Int(width - 2, height / 2), new Vector2Int(width - 2, (height / 2) + 1) };

            return topLeft.Concat(topRight).Concat(bottomLeft).Concat(bottomRight)
                .Concat(top).Concat(bottom).Concat(left).Concat(right).ToHashSet();
        }
    }
}
