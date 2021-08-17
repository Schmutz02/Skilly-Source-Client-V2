using System;
using System.Collections.Generic;
using Game.Entities;
using Game.EntityWrappers;
using Models;
using Networking;
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
        // private Dictionary<string, Queue<EntityWrapper>> _entityPool;
        private HashSet<EntityWrapper> _interactiveObjects;

        [HideInInspector]
        public int MovesRequested;
        [HideInInspector]
        public string WorldName;

        public Player MyPlayer { get; private set; }

        private int _lastInteractiveUpdateTime;
        
        private bool _inUpdate;
        private List<int> _idsToRemove;
        private List<Entity> _objsToAdd;

        private void Awake()
        {
            _wrapperPrefabs = new Dictionary<string, EntityWrapper>();
            foreach (var wrapper in Resources.LoadAll<EntityWrapper>("Entities"))
            {
                _wrapperPrefabs[wrapper.name] = wrapper;
            }
            
            Entities = new Dictionary<int, EntityWrapper>();
            // _entityPool = new Dictionary<string, Queue<EntityWrapper>>();
            _interactiveObjects = new HashSet<EntityWrapper>();
            _idsToRemove = new List<int>();
            _objsToAdd = new List<Entity>();
            
            Networking.Packets.Incoming.Update.OnMyPlayerJoined += OnMyPlayerJoined;
        }

        private void OnMyPlayerJoined(Player player)
        {
            MyPlayer = player;
        }

        public void Tick()
        {
            _inUpdate = true;
            foreach (var wrapper in Entities.Values)
            { 
                if (!wrapper.Tick())
                    _idsToRemove.Add(wrapper.Entity.ObjectId);
            }

            _inUpdate = false;

            foreach (var obj in _objsToAdd)
            {
                InternalAddObj(obj);
            }
            _objsToAdd.Clear();

            foreach (var id in _idsToRemove)
            {
                InternalRemoveObj(id);
            }
            _idsToRemove.Clear();
            
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
            Entities.Clear();
            _interactiveObjects.Clear();

            foreach (Transform child in _entityParentTransform)
            {
                // var wrapper = child.GetComponent<EntityWrapper>();
                // var type = wrapper.Entity.Desc.Class;
                // if (!_entityPool.ContainsKey(type))
                //     _entityPool[type] = new Queue<EntityWrapper>();
                // _entityPool[type].Enqueue(wrapper);

                Destroy(child.gameObject);
                // child.gameObject.SetActive(false);
            }
        }

        public void AddTile(TileData tileData)
        {
            var tile = ScriptableObject.CreateInstance<Square>();
            var tileDesc = AssetLibrary.GetTileDesc(tileData.TileType);
            tile.Init(tileDesc, tileData.X, tileData.Y);
            _tilemap.SetTile(tile.Position, tile);
        }

        public void AddObject(Entity entity, Vector2 position)
        {
            entity.Position = position;
            if (_inUpdate)
            {
                _objsToAdd.Add(entity);
            }
            else
            {
                InternalAddObj(entity);
            }
        }

        private void InternalAddObj(Entity entity)
        {
            EntityWrapper wrapper = null;
            try
            {
                var wrapperPrefab = _wrapperPrefabs[entity.Desc.Class];
                wrapper = Instantiate(wrapperPrefab, _entityParentTransform);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"No wrapper found for class {entity.Desc.Class}");
                Debug.LogError(e.Message);
            }
            // if (_entityPool.ContainsKey(entity.Desc.Class) && _entityPool[entity.Desc.Class].Count > 0)
            // {
            //     wrapper = _entityPool[entity.Desc.Class].Dequeue();
            //     wrapper.gameObject.SetActive(true);
            // }
            // else
            // {
            //     
            // }
            wrapper?.Init(entity); // always rotates unless overridden

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
            return Entities[id].Entity;
        }

        public void RemoveObject(int objectId)
        {
            if (_inUpdate)
            {
                _idsToRemove.Add(objectId);
            }
            else
            {
                InternalRemoveObj(objectId);
            }
        }

        private void InternalRemoveObj(int objectId)
        {
            var en = Entities[objectId];
            var type = en.Entity.Desc.Class;
            // if (!_entityPool.ContainsKey(type))
            //     _entityPool[type] = new Queue<EntityWrapper>();
            // _entityPool[type].Enqueue(en);
            
            Destroy(en.gameObject);
            // en.gameObject.SetActive(false);
            Entities.Remove(objectId);
            
            if (en is IInteractiveObject)
                _interactiveObjects.Remove(en);
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