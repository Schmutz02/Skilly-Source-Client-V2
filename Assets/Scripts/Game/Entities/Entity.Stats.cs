using System.Collections.Generic;
using Models.Static;

namespace Game.Entities
{
    public partial class Entity
    {
        public void UpdateObjectStats(Dictionary<StatType, object> stats)
        {
            foreach (var stat in stats)
            {
                var key = stat.Key;
                var value = stat.Value;
                
                UpdateStat(key, value);
            }
        }
        
        protected virtual void UpdateStat(StatType stat, object value)
        {
            switch (stat)
            {
                case StatType.Condition:
                    _conditionEffects = (ConditionEffect) (int) value;
                    return;
                case StatType.MaxHp:
                    MaxHp = (int) value;
                    return;
                case StatType.Hp:
                    Hp = (int) value;
                    return;
                case StatType.Size:
                    Size = (int) value;
                    return;
                case StatType.Name:
                    Name = (string) value;
                    return;
                case StatType.AltTexture:
                    AltTextureIndex = (int) value;
                    return;
                case StatType.SinkLevel:
                    SinkLevel = (int) value;
                    return;
            }
        }
    }
}