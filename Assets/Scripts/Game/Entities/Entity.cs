using System;
using System.Collections.Generic;
using Models;
using Models.Static;
using UnityEngine;
using Action = Models.Static.Action;

namespace Game.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Entity : MonoBehaviour
    {
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Size { get; private set; }
        public string Name { get; private set; }
        public int AltTextureIndex { get; private set; }
        
        public int ObjectId { get; private set; }
        
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Init(ObjectDefinition def)
        {
            var desc = AssetLibrary.GetObjectDesc(def.ObjectType);
            _renderer.sprite = desc.TextureData.Texture ??
                               desc.TextureData.Animation.GetFrame(Direction.Down, Action.Stand, 0);

            ObjectId = def.ObjectStatus.Id;
        }

        public void UpdateObjectStats(Dictionary<StatType, object> stats)
        {
            foreach (var stat in stats)
            {
                var key = stat.Key;
                var value = stat.Value;

                switch (key)
                {
                    case StatType.MaxHp:
                        MaxHp = (int) value;
                        break;
                    case StatType.Hp:
                        Hp = (int) value;
                        break;
                    case StatType.Size:
                        Size = (int) value;
                        break;
                    case StatType.Name:
                        Name = (string) value;
                        name = (string) value;
                        break;
                    case StatType.AltTexture:
                        AltTextureIndex = (int) value;
                        break;
                }

                if (this is Player player)
                {
                    player.UpdateStat(key, value);
                }
            }
        }

        protected virtual void UpdateStat(StatType stat, object value)
        {
            
        }
    }
}