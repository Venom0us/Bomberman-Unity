using UnityEngine;

namespace Bomberman.SharedFiles.Generation
{
    [CreateAssetMenu(menuName = "Map/Generators/ArenaGenerator")]
    public class ArenaGenerator : Generator
    {
        public TileLayerTypes.WorldTiles WalkableTile;
        public TileLayerTypes.WorldTiles WallTile;
        public TileLayerTypes.WorldTiles BorderTile;

        public override void Generate(TilemapStructure tilemapStructure)
        {
            for (int x=0; x < tilemapStructure.Width; x++)
            {
                for (int y = 0; y < tilemapStructure.Height; y++)
                {
                    if (x == 0 || y == 0 || x == tilemapStructure.Width - 1 || y == tilemapStructure.Height - 1)
                        tilemapStructure.SetTile(x, y, BorderTile, false, false);
                    else if (x % 2 == 0 && y % 2 == 0)
                        tilemapStructure.SetTile(x, y, WallTile, false, false);
                    else
                        tilemapStructure.SetTile(x, y, WalkableTile, false, false);
                }
            }
        }
    }
}
