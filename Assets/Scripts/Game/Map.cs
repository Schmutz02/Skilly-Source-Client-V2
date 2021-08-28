using System;
using System.Collections.Generic;
using System.Linq;
using Game.Entities;
using Game.EntityWrappers;
using Models;
using Networking;
using Networking.Packets.Incoming;
using Networking.Packets.Outgoing;
using UI.GameScreen.Panels;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using TileData = Models.TileData;

namespace Game
{
    public class Map : MonoBehaviour
    {
        public static Action<IInteractiveObject> UpdateInteractive;
        
        private const int _INTERACTIVE_UPDATE_INTERVAL = 100;
        
        [SerializeField]
        private Tilemap _tilemap;

        [SerializeField]
        private Transform _entityParentTransform;

        public MapOverlay Overlay;

        private Dictionary<string, EntityWrapper> _wrapperPrefabs;
        public Dictionary<int, EntityWrapper> Entities;
        
        private HashSet<EntityWrapper> _interactiveObjects;

        [HideInInspector]
        public int MovesRequested;

        public string WorldName { get; private set; }
        public int Width => _tilemap.size.x;
        public int Height => _tilemap.size.y;

        public Player MyPlayer { get; private set; }
        
        public EntityWrapperPool ObjectPool { get; private set; }

        private int _lastInteractiveUpdateTime;

        private void Awake()
        {
            _wrapperPrefabs = new Dictionary<string, EntityWrapper>();
            foreach (var wrapper in Resources.LoadAll<EntityWrapper>("Entities"))
            {
                _wrapperPrefabs[wrapper.name] = wrapper;
            }

            ObjectPool = new EntityWrapperPool(_entityParentTransform, _wrapperPrefabs);
            
            Entities = new Dictionary<int, EntityWrapper>();
            _interactiveObjects = new HashSet<EntityWrapper>();
            
            Update.OnMyPlayerJoined += OnMyPlayerJoined;
        }

        private void OnMyPlayerJoined(Player player)
        {
            MyPlayer = player;
        }

        public void Initialize(MapInfo mapInfo)
        {
            WorldName = mapInfo.Name;
            _tilemap.size = new Vector3Int(mapInfo.Width, mapInfo.Height, 0);
        }

        public void Tick()
        {
            foreach (var wrapper in Entities.Values.ToArray())
            {
                if (!wrapper.Tick())
                    RemoveObject(wrapper.Entity);
            }
            
            if (GameTime.Time - _lastInteractiveUpdateTime >= _INTERACTIVE_UPDATE_INTERVAL)
            {
                UpdateNearestInteractive();
                _lastInteractiveUpdateTime = GameTime.Time;
            }
            
            if (MyPlayer != null && MovesRequested > 0)
            {
                TcpTicker.Send(new Move(GameTime.Time, MyPlayer.Position));
                MyPlayer.OnMove();
                MovesRequested--;
            }
        }

        private void UpdateNearestInteractive()
        {
            if (MyPlayer == null)
                return;

            var minDistSqr = Settings.MAXIMUM_INTERACTION_DISTANCE * Settings.MAXIMUM_INTERACTION_DISTANCE;
            var playerX = MyPlayer.Position.x;
            var playerY = MyPlayer.Position.y;
            IInteractiveObject closestInteractive = null;
            foreach (var obj in _interactiveObjects)
            {
                var objX = obj.transform.position.x;
                var objY = obj.transform.position.y;
                if (Mathf.Abs(playerX - objX) < Settings.MAXIMUM_INTERACTION_DISTANCE &&
                    Mathf.Abs(playerY - objY) < Settings.MAXIMUM_INTERACTION_DISTANCE)
                {
                    var distSqr = MathUtils.DistanceSquared(MyPlayer.Position, obj.transform.position);
                    if (distSqr < minDistSqr)
                    {
                        minDistSqr = distSqr;
                        closestInteractive = obj as IInteractiveObject;
                    }
                }
            }
            
            UpdateInteractive?.Invoke(closestInteractive);
        }

        public void Dispose()
        {
            MovesRequested = 0;
            _tilemap.ClearAllTiles();

            foreach (var wrp in Entities.Values.ToArray())
                RemoveObject(wrp.Entity);
            
            Entities.Clear();
            _interactiveObjects.Clear();
        }

        public void AddTile(TileData tileData)
        {
            var tile = ScriptableObject.CreateInstance<Square>();
            var tileDesc = AssetLibrary.GetTileDesc(tileData.TileType);
            tile.Init(this, tileDesc, tileData.X, tileData.Y);
            _tilemap.SetTile(tile.Position, tile);
            var x = tileData.X;
            var y = tileData.Y;
            var xEnd = x < Width - 1 ? x + 1 : x;
            var yEnd = y < Height - 1 ? y + 1 : y;
            for (var xi = x > 0 ? x - 1 : x; xi <= xEnd; xi++)
            {
                for (var yi = y > 0 ? y - 1 : y; yi <= yEnd; yi++)
                {
                    var square = GetTile(xi, yi);
                    if (square != null && (square.Desc.HasEdge || square.Type != tile.Type))
                    {
                        square.Redraw();
                        _tilemap.RefreshTile(square.Position);
                    }
                }
            }
        }

        public void AddObject(Entity entity, Vector2 position)
        {
            // todo: please change this to be part of init
            entity.Position = position;
            // Fetch a valid EntityWrapper from the pool
            var wrapper = ObjectPool.Get(entity.Desc.Class);
            if (wrapper is null)
                return;
            // Assign our entity to the wrapper
            wrapper.Init(entity);
            // Add relevant entity data to map
            if (entity.Desc.Static)
            {
                var tile = GetTile(entity.Position);
                tile.StaticObject = entity;
            }

            if (wrapper is IInteractiveObject)
                _interactiveObjects.Add(wrapper);

            Entities[entity.ObjectId] = wrapper;
        }

        public Entity GetEntity(int id)
        {
            if (Entities.ContainsKey(id))
                return Entities[id].Entity;
            return null;
        }

        public void RemoveObject(Entity entity)
        {
            // Store the EntityWrapper
            var wrp = entity.Wrapper;
            // Return the EntityWrapper to the pool.
            ObjectPool.Return(wrp);
            // Remove the Entity from the list.
            Entities.Remove(entity.ObjectId);
            // Do any type-specific cleanup
            if (wrp is IInteractiveObject)
                _interactiveObjects.Remove(wrp);
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