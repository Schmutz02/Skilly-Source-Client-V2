using System;
using System.Collections.Generic;
using Game.EntityWrappers;
using Game.MovementControllers;
using Models.Static;
using UnityEngine;
using Utils;

namespace Game.Entities
{
    public partial class Entity
    {
        public const float _HITBOX_RADIUS = 0.5f;
        
        private static int _nextFakeObjectId;
        
        private ConditionEffect _conditionEffects;
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Defense { get; protected set; }
        public int Size { get; protected set; } = 100;
        public string Name { get; private set; }
        public int AltTextureIndex { get; private set; }
        public int SinkLevel { get; protected set; }

        public EntityWrapper Wrapper;
        
        public readonly Map Map;
        public Square Square;
        public readonly int ObjectId;
        public readonly ObjectDesc Desc;
        public Vector2 Position;
        public float Z;
        public float Rotation; // in radians
        public bool Flying;
        public readonly bool IsMyPlayer;
        
        public readonly int SizeMult = 1;

        public readonly GameObject Model;
        
        public int AttackStart;
        public float AttackAngle;
        public Vector2 Direction => _movementController.Direction;

        private readonly IMovementController _movementController;
        public readonly ITextureProvider TextureProvider;

        public Entity(ObjectDesc desc, int objectId, bool isMyPlayer, Map map)
        {
            Desc = desc;
            Map = map;
            ObjectId = objectId;
            Position = Vector3.zero;
            IsMyPlayer = isMyPlayer;
            Defense = desc.Defense;
            Z = desc.Z;

            if (isMyPlayer)
            {
                _movementController = new PlayerMovementController(this as Player);
                TextureProvider = new PlayerTextureProvider(this as Player);
            }
            else
            {
                _movementController = new EntityMovementController(this);
                TextureProvider = new EntityTextureProvider(this);
            }

            if (Desc.TextureData.Texture)
                SizeMult = (int) Desc.TextureData.Texture.rect.height / 8;

            if (Desc.Model != null)
            {
                Model = AssetLibrary.GetModel(Desc.Model);
            }
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
            var movement = _movementController as EntityMovementController;
            if (movement!.TargetPosition == position)
                return;

            movement.TargetPosition = position;
            movement.Direction = (movement.TargetPosition - (Vector2)Position) / 127f;
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

            switch (desc.Class)
            {
                case "Player":
                    return new Player(AssetLibrary.GetPlayerDesc(type), objectId, isMyPlayer, map);
                case "Portal":
                    return new Portal(AssetLibrary.GetObjectDesc(type), objectId, map);
            }

            return new Entity(desc, objectId, isMyPlayer, map);
        }
    }
}