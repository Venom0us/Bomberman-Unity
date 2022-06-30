namespace Bomberman.SharedFiles.TilemapFiles
{
    public class TileLayerTypes
    {
        public enum WorldTiles
        {
            Empty = 0,
            Ground,
            Pillar,
        }

        public enum ObjectTiles
        {
            Empty = 0,
            Wall,
            Bomb,
            SpeedPowerUp,
            StrengthPowerUp,
            InvincibiltyPowerUp,
            ExtraBombsPowerUp
        }
    }
}
