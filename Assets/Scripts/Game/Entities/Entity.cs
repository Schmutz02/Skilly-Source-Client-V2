using Game.MovementControllers;
using Models.Static;
using UnityEngine;
using Utils;

namespace Game.Entities
{
    public partial class Entity
    {
        //TODO do animation things seperate
        public const int ATTACK_PERIOD = 500;
        
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
        
        public int AttackStart;
        public float AttackAngle;
        public float Facing;

        protected readonly IMovementController MovementController;

        public Entity(ObjectDesc desc, int objectId, bool isMyPlayer, Map map)
        {
            Desc = desc;
            Map = map;
            ObjectId = objectId;
            Position = Vector2.zero;
            IsMyPlayer = isMyPlayer;

            if (isMyPlayer)
                MovementController = new PlayerMovementController(this as Player);
            else
                MovementController = new EntityMovementController(this);
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

        public virtual void SetAttack(ItemDesc container, float attackAngle)
        {
            AttackAngle = attackAngle;
            AttackStart = GameTime.Time;
        }

        public void OnNewTick(Vector2 position)
        {
            // only called on things that aren't my player
            var movement = MovementController as EntityMovementController;
            if (movement!.TargetPosition == position)
                return;

            movement.TargetPosition = position;
            movement.Direction = (movement.TargetPosition - Position) / 127f;
        }

        public virtual bool Tick(int time, int dt, Camera camera)
        {
            MovementController?.Tick(dt);
            return true;
        }

        //TODO possibly extract out
        public virtual Sprite GetTexture(int time)
        {
            Sprite image;
            if (Desc.TextureData.Animation != null)
            {
                var p = 0;
                var action = Action.Stand;
                if (time < AttackStart + ATTACK_PERIOD)
                {
                    if (!Desc.DontFaceAttacks)
                    {
                        Facing = AttackAngle;
                    }

                    p = (time - AttackStart) % ATTACK_PERIOD / ATTACK_PERIOD;
                    action = Action.Attack;
                }
                else if (MovementController.Direction != Vector2.zero)
                {
                    var walkPer = (int)(0.5 / (MovementController.Direction.magnitude * 4));
                    walkPer = 400 - walkPer % 400;
                    Facing = Mathf.Atan2(MovementController.Direction.y, MovementController.Direction.x);
                    action = Action.Walk;
                    p = time % walkPer / walkPer;
                    Debug.LogError(p);
                }

                image = Desc.TextureData.Animation.ImageFromFacing(Facing, action, p);
            }
            else
            {
                image = Desc.TextureData.Texture;
            }

            return SpriteUtils.Redraw(image, 100);
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