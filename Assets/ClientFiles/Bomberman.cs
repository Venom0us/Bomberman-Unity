using Bomberman.SharedFiles.Others;
using Bomberman.SharedFiles.TilemapFiles;
using UnityEngine;

namespace Bomberman.ClientFiles
{
    public class Bomberman : MonoBehaviour
    {
        private TileGrid _grid;
        private TilemapStructure _worldLayer, _objectLayer;

        [HideInInspector]
        public Vector2Int GridPosition;

        private void Start()
        {
            _grid = TileGrid.Instance;
            _worldLayer = _grid.GetTilemap(TileGrid.Layer.World);
            _objectLayer = _grid.GetTilemap(TileGrid.Layer.Object);
        }

        public void RequestMove(int x, int y)
        {
            if (_worldLayer.GetTile(x, y) != (int)TileLayerTypes.WorldTiles.Pillar &&
                _objectLayer.GetTile(x, y) != (int)TileLayerTypes.ObjectTiles.Wall)
            {
                // Send movement request to the server
                Client.Instance.Notify(OpCodes.Move, x + ";" + y);
            }
        }

        public void Move(int x, int y)
        {
            GridPosition = new Vector2Int(x, y);
            transform.position = new Vector2(x + 0.5f, y + 0.5f);
        }
    }
}
