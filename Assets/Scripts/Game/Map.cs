using System;
using System.Collections.Generic;
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
        private Transform _entityParentTransform;

        private Dictionary<int, Entity> _entities;

        private void Awake()
        {
            _entities = new Dictionary<int, Entity>();
        }

        private void OnEnable()
        {
            foreach (Transform child in _entityParentTransform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public void AddTile(TileData tileData)
        {
            // TODO convert to objects for pooling
            var tile = ScriptableObject.CreateInstance<Tile>();
            var tileDesc = AssetLibrary.GetTileDesc(tileData.TileType);
            tile.sprite = tileDesc.TextureData.Texture;
            _tilemap.SetTile(new Vector3Int(tileData.X, tileData.Y, 0), tile);
        }

        public Entity AddObject(GameObject defaultEntity, ObjectStatus status)
        {
            //TODO optimize (make a data structure)
            foreach (Transform child in _entityParentTransform.transform)
            {
                if (child.gameObject.activeSelf)
                    continue;

                //TODO is this even pooling??
                child.transform.position = status.Position;
                child.gameObject.SetActive(true);
                Destroy(child.GetComponent<Entity>());
                var type = defaultEntity.GetComponent<Entity>();
                return child.gameObject.AddComponent(type.GetType()) as Entity;
            }
            
            var entity = Instantiate(defaultEntity, _entityParentTransform).GetComponent<Entity>();
            entity.transform.position = status.Position;

            _entities[status.Id] = entity;
            return entity;
        }

        public Entity GetEntity(int id)
        {
            return _entities[id];
        }

        public void RemoveObject(ObjectDrop obj)
        {
            
        }
    }
}