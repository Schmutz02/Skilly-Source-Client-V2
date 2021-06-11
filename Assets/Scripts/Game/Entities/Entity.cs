using Game.MovementControllers;
using Models.Static;
using UnityEngine;

namespace Game.Entities
{
    public partial class Entity
    {
        private static int _nextFakeObjectId;
        
        private ConditionEffect _conditionEffects;
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Size { get; private set; }
        public string Name { get; private set; }
        public int AltTextureIndex { get; private set; }

        public readonly Map Map;
        public Square Square;
        public readonly int ObjectId;
        public readonly ObjectDesc Desc;
        public Vector2 Position;
        public float Rotation; // in radians
        public readonly bool IsMyPlayer;

        private readonly IMovementController _movementController;

        public Entity(ObjectDesc desc, int objectId, bool isMyPlayer, Map map)
        {
            Desc = desc;
            Map = map;
            ObjectId = objectId;
            Position = Vector2.zero;
            IsMyPlayer = isMyPlayer;

            if (isMyPlayer)
                _movementController = new PlayerMovementController(this as Player);
            else
                _movementController = new EntityMovementController(this);
        }

        public static int GetNextFakeObjectId()
        {
            return 2130706432 | _nextFakeObjectId++;
        }

        public virtual bool MoveTo(Vector2 position)
        {
            Map.MoveEntity(this, position);
            return true;
        }

        public void OnNewTick(Vector2 position)
        {
            // only called on things that aren't my player
            var movement = _movementController as EntityMovementController;
            if (movement!.TargetPosition == position)
                return;

            movement.TargetPosition = position;
            movement.Direction = (movement.TargetPosition - Position) / 127f;
        }

        public virtual bool Tick(int time, int dt, Camera camera)
        {
            _movementController?.Tick(dt);
            return true;
        }

        public bool HasConditionEffect(ConditionEffect conditionEffect)
        {
            return (_conditionEffects & conditionEffect) != 0;
        }

        public static Entity Resolve(ushort type, int objectId, bool isMyPlayer, Map map)
        {
            var desc = AssetLibrary.GetObjectDesc(type);
            
            Debug.Log($"Resolving entity with class {desc.Class}");

            switch (desc.Class)
            {
                case "Player":
                    return new Player(AssetLibrary.GetPlayerDesc(type), objectId, isMyPlayer, map);
            }

            return new Entity(desc, objectId, isMyPlayer, map);
        }
    }
}