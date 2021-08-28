using System;
using Game.MovementControllers;
using Models.Static;
using UnityEngine;
using Utils;

namespace Game.Entities
{
    public partial class Entity : MonoBehaviour
    {
        public const float _HITBOX_RADIUS = 0.5f;
        
        private static int _nextFakeObjectId;
        
        [NonSerialized]
        private ConditionEffect _conditionEffects;
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Defense { get; protected set; }
        public int Size { get; protected set; } = 100;
        public string Name { get; private set; }
        public int AltTextureIndex { get; private set; }
        public int SinkLevel { get; protected set; }

        public Map Map { get; private set; }
        public Square Square { get; set; }
        public int ObjectId { get; private set; }
        public ObjectDesc Desc { get; private set; }

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                var yOffset = Desc.DrawOnGround ? -0.5f : 0;
                transform.position = new Vector3(value.x, value.y + yOffset, -Z); //TODO check z value
            }
        }

        public float Rotation // in radians
        {
            get => transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            set => transform.rotation = Quaternion.Euler(0, 0, value * Mathf.Rad2Deg);
        }
        
        public float Z { get; set; }
        public bool Flying { get; set; }
        public bool IsMyPlayer { get; private set; }

        public int SizeMult { get; private set; } = 1; 

        public int AttackStart { get; set; }
        public float AttackAngle { get; set; }
        public Vector2 Direction => _movementController.Direction;

        private IMovementController _movementController;
        public ITextureProvider TextureProvider { get; private set; }

        protected virtual void Init(ObjectDesc desc, int objectId, bool isMyPlayer, Map map, bool rotating = true)
        {
            Desc = desc;
            Map = map;
            ObjectId = objectId;
            Position = new Vector3(0, 0);
            Z = desc.Z;
            IsMyPlayer = isMyPlayer;
            Defense = desc.Defense;

            if (IsMyPlayer)
            {
                _movementController = new PlayerMovementController(this as Player);
                TextureProvider = new PlayerTextureProvider(this as Player);
                CameraManager.SetFocus(gameObject);
            }
            else
            {
                _movementController = new EntityMovementController(this);
                TextureProvider = new EntityTextureProvider(this);
            }

            Size = 100;
            SizeMult = 1;
            if (Desc.TextureData.Texture)
                SizeMult = (int) Desc.TextureData.Texture.rect.height / 8;

            if (Desc.Model != null)
            {
                AddModel();
                Renderer.sprite = null;
            }
            
            Renderer.sortingLayerName = Desc.DrawUnder ? "DrawUnder" : "Visible";
            ShadowRenderer.gameObject.SetActive(!Desc.DrawOnGround);

            SetPositionAndRotation();
            RedrawShadow();

            if (rotating && !Desc.DrawOnGround)
                CameraManager.AddRotatingEntity(this);

            gameObject.SetActive(true);
        }

        public static int GetNextFakeObjectId()
        {
            return 2130706432 | _nextFakeObjectId++;
        }

        public virtual bool MoveTo(Vector3 position)
        {
            Map.MoveEntity(this, position);
            return true;
        }

        public virtual void SetAttack(ItemDesc container, float attackAngle)
        {
            AttackAngle = attackAngle;
            AttackStart = GameTime.Time;
        }

        // only called on things that aren't my player
        public void OnNewTick(Vector3 position)
        {
            var movement = _movementController as EntityMovementController;
            if (movement!.TargetPosition == position)
                return;

            movement.TargetPosition = position;
            movement.Direction = (movement.TargetPosition - Position) / 127f;
        }
        
        public virtual bool Tick()
        {
            _movementController?.Tick(GameTime.DeltaTime);
            return true;
        }

        public bool HasConditionEffect(ConditionEffect conditionEffect)
        {
            return (_conditionEffects & conditionEffect) != 0;
        }

        public virtual void Damage(int damage, ConditionEffectDesc[] effects, Projectile projectile)
        {
            if (effects != null)
            {
                var offsetTime = 0;
                foreach (var effectDesc in effects)
                {
                    var effect = effectDesc.Effect;
                    if (effect == ConditionEffect.Nothing)
                        continue;
                    
                    switch (effect)
                    {
                        case ConditionEffect.Stunned:
                            if (HasConditionEffect(ConditionEffect.StunImmune))
                            {
                                Map.Overlay.AddStatusText(this, "Immune", Color.red, 3000);
                                continue;
                            }
                            break;
                    }
                    
                    Map.Overlay.AddStatusText(this, effect.ToString(), Color.red, 3000, offsetTime);
                    offsetTime += 500;
                }
            }

            if (damage > 0)
            {
                var pierced = HasConditionEffect(ConditionEffect.ArmorBroken) ||
                              projectile != null && projectile.ProjectileDesc.ArmorPiercing;
                
                Map.Overlay.AddStatusText(this, "-" + damage, pierced ? Color.magenta : Color.red, 1000);
            }
        }

        public static int DamageWithDefense(Entity target, int damage, bool armorPiercing)
        {
            var def = target.Defense;
            if (armorPiercing || target.HasConditionEffect(ConditionEffect.ArmorBroken))
                def = 0;
            else if (target.HasConditionEffect(ConditionEffect.Armored))
                def *= 2;

            var min = damage * 3 / 20;
            var d = Math.Max(min, damage - def);
            if (target.HasConditionEffect(ConditionEffect.Invulnerable))
                d = 0;
            
            return d;
        }

        public static Entity Resolve(ushort type, int objectId, bool isMyPlayer, Map map)
        {
            var desc = AssetLibrary.GetObjectDesc(type);
            var en = map.EntityPool.Get(desc.Class);
            if (desc.Class == "Player")
            {
                desc = AssetLibrary.GetPlayerDesc(type);
            }
            en.Init(desc, objectId, isMyPlayer, map);
            return en;
        }
    }
}