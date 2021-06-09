using System;
using System.Collections.Generic;
using Game.MovementControllers;
using Models;
using Models.Static;
using UnityEngine;
using Action = Models.Static.Action;

namespace Game.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Entity : MonoBehaviour
    {
        [NonSerialized]
        private ConditionEffect _conditionEffects;
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Size { get; private set; }
        public string Name { get; private set; }
        public int AltTextureIndex { get; private set; }

        public Map Owner { get; private set; }
        public Square Square;
        public int ObjectId { get; private set; }
        public ObjectDesc Desc { get; private set; }

        private IMovementController _movementController;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Init(ObjectDefinition def, Map map)
        {
            var desc = AssetLibrary.GetObjectDesc(def.ObjectType);
            Desc = desc;
            
            _renderer.sprite = desc.TextureData.Texture ??
                               desc.TextureData.Animation.GetFrame(Facing.Down, Action.Stand, 0);

            Owner = map;
            ObjectId = def.ObjectStatus.Id;
        }

        public void AddMovementController(bool isMyPlayer)
        {
            if (isMyPlayer)
                _movementController = new PlayerMovementController(this as Player);
            else
                _movementController = new EntityMovementController(this);
        }

        public void UpdateObjectStats(Dictionary<StatType, object> stats)
        {
            foreach (var stat in stats)
            {
                var key = stat.Key;
                var value = stat.Value;

                switch (key)
                {
                    case StatType.Condition:
                        _conditionEffects = (ConditionEffect) (int) value;
                        continue;
                    case StatType.MaxHp:
                        MaxHp = (int) value;
                        continue;
                    case StatType.Hp:
                        Hp = (int) value;
                        continue;
                    case StatType.Size:
                        Size = (int) value;
                        continue;
                    case StatType.Name:
                        Name = (string) value;
                        name = (string) value;
                        continue;
                    case StatType.AltTexture:
                        AltTextureIndex = (int) value;
                        continue;
                }

                if (this is Player player)
                {
                    player.UpdateStat(key, value);
                }
            }
        }
        
        protected virtual void UpdateStat(StatType stat, object value)
        {
            //TODO probably change
        }

        public void MoveTo(Vector2 position)
        {
            Owner.MoveEntity(this, position);
        }

        public void OnNewTick(Vector2 position)
        {
            // only called on things that aren't my player
            var movement = _movementController as EntityMovementController;
            if (movement!.TargetPosition == position)
                return;

            movement.TargetPosition = position;
            movement.Direction = (movement.TargetPosition - (Vector2)transform.position) / 127f;
        }

        private void Update()
        {
            _movementController?.Tick(Time.deltaTime * 1000);
        }

        public bool HasConditionEffect(ConditionEffect conditionEffect)
        {
            return (_conditionEffects & conditionEffect) != 0;
        }
    }
}