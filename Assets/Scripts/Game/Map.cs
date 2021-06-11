using System.Collections.Generic;
using Game.Entities;
using Game.EntityWrappers;
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

        public void Clear()
        {
            _tilemap.ClearAllTiles();
            _entities.Clear();

            foreach (Transform child in _entityParentTransform)
            {
                //TODO pooling instead
                Destroy(child.gameObject);
            }
        }

        public void AddTile(TileData tileData)
        {
            // TODO convert to objects for pooling
            var tile = ScriptableObject.CreateInstance<Square>();
            var tileDesc = AssetLibrary.GetTileDesc(tileData.TileType);
            tile.Init(tileDesc);
            _tilemap.SetTile(new Vector3Int(tileData.X, tileData.Y, 0), tile);
        }

        public bool AddObject(Entity entity, Vector2 position)
        {
            var wrapperPrefab = Resources.Load<EntityWrapper>($"Entities/{entity.Desc.Class}");
            var wrapper = Instantiate(wrapperPrefab, _entityParentTransform);
            wrapper.Init(entity, true); // always rotates unless overridden
            
            entity.Position = position;

            if (entity.Desc.Static)
            {
                var tile = GetTile(position);
                tile.StaticObject = entity;
            }

            _entities[entity.ObjectId] = entity;
            return true;
        }

        public Entity GetEntity(int id)
        {
            return _entities[id];
        }

        public void RemoveObject(int objectId)
        {
            _entities.Remove(objectId);
        }

        public void MoveEntity(Entity entity, Vector2 position)
        {
            var tile = GetTile(position);
            entity.Position = position;

            if (entity.Desc.Static)
            {
                if (entity.Square != null)
                {
                    entity.Square.StaticObject = null;
                }

                tile.StaticObject = entity;
            }

            entity.Square = tile;
        }
        
        public bool RegionUnblocked(float x, float y)
        {
            if (TileIsWalkable(x, y))
                return false;

            var xFrac = x - (int)x;
            var yFrac = y - (int)y;

            if (xFrac < 0.5f)
            {
                if (TileFullOccupied(x - 1, y))
                    return false;

                if (yFrac < 0.5f)
                {
                    if (TileFullOccupied(x, y - 1) || TileFullOccupied(x - 1, y - 1))
                        return false;
                }
                else
                {
                    if (yFrac > 0.5f)
                        if (TileFullOccupied(x, y + 1) || TileFullOccupied(x - 1, y + 1))
                            return false;
                }

                return true;
            }

            if (xFrac > 0.5f)
            {
                if (TileFullOccupied(x + 1, y))
                    return false;

                if (yFrac < 0.5)
                {
                    if (TileFullOccupied(x, y - 1) || TileFullOccupied(x + 1, y - 1))
                        return false;
                }
                else
                {
                    if (yFrac > 0.5)
                        if (TileFullOccupied(x, y + 1) || TileFullOccupied(x + 1, y + 1))
                            return false;
                }

                return true;
            }

            if (yFrac < 0.5)
            {
                if (TileFullOccupied(x, y - 1))
                    return false;

                return true;
            }

            if (yFrac > 0.5)
                if (TileFullOccupied(x, y + 1))
                    return false;

            return true;
        }

        private bool TileIsWalkable(float x, float y)
        {
            var tile = GetTile(x, y);
            if (tile == null)
                return true;
            
            if (tile.Desc.NoWalk)
                return true;

            if (tile.StaticObject != null)
            {
                if (tile.StaticObject.Desc.OccupySquare)
                    return true;
            }

            return false;
        }

        private bool TileFullOccupied(float x, float y)
        {
            var tile = GetTile(x, y);
            if (tile == null)
                return true;

            if (tile.Type == 255)
                return true;

            if (tile.StaticObject != null)
            {
                if (tile.StaticObject.Desc.FullOccupy)
                    return true;
            }

            return false;
        }

        public Square GetTile(float x, float y)
        {
            return _tilemap.GetTile<Square>(new Vector3Int((int) x, (int) y, 0));
        }

        public Square GetTile(Vector2 pos)
        {
            return GetTile(pos.x, pos.y);
        }
    }
}