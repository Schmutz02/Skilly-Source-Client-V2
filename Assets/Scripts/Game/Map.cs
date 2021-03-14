using Game.Entities;
using Models;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Models.TileData;

namespace Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private Tilemap _tilemap;

        [SerializeField]
        private GameObject _entity;

        [SerializeField]
        private Transform _entityParentTransform;
        
        public void AddTile(TileData tileData)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            var tileDesc = AssetLibrary.GetTileDesc(tileData.TileType);
            tile.sprite = tileDesc.TextureData.Texture;
            _tilemap.SetTile(new Vector3Int(tileData.X, tileData.Y, 0), tile);
        }

        public void AddObject(ObjectDefinition objDef)
        {
            //TODO add pooling
            var obj = Instantiate(_entity, _entityParentTransform).GetComponent<Entity>();
            obj.Init(objDef.ObjectType);

            obj.transform.position = objDef.ObjectStatus.Position;
        }

        public void RemoveObject(ObjectDrop obj)
        {
            
        }
    }
}