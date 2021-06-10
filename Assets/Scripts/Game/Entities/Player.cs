using Game.MovementControllers;
using Models;
using Models.Static;
using UnityEngine;

namespace Game.Entities
{
    public class Player : Entity
    {
        private const float _MIN_MOVE_SPEED = 0.004f;
        private const float _MAX_MOVE_SPEED = 0.0096f;
        private const float _MIN_ATTACK_FREQ = 0.0015f;
        private const float _MAX_ATTACK_FREQ = 0.008f;
        private const float _MIN_ATTACK_MULT = 0.5f;
        private const float _MAX_ATTACK_MULT = 2f;
        private const float _MAX_SINK_LEVEL = 18f;

        public float MoveMultiplier = 1f;

        public new PlayerDesc Desc { get; private set; }
        public int AccountId { get; private set; }
        public int Exp { get; private set; }
        public int NextLevelExp { get; private set; }
        public int Level { get; private set; }
        public int Fame { get; private set; }
        public int NextClassQuestFame { get; private set; }
        public int NumStars { get; private set; }
        public string GuildName { get; private set; }
        public GuildRank GuildRank { get; private set; }
        public int Credits { get; private set; }
        public int DyeLarge { get; private set; }
        public int DyeSmall { get; private set; }
        public bool HasBackpack { get; private set; }
        public int Mp { get; private set; }
        public int MaxMp { get; private set; }
        public int Oxygen { get; private set; }
        public int HealthPotions { get; private set; }
        public int MagicPotions { get; private set; }
        public int SinkLevel { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }
        public int Dexterity { get; private set; }
        public int Vitality { get; private set; }
        public int Wisdom { get; private set; }
        public float PushX { get; private set; }
        public float PushY { get; private set; }
        public ItemType[] SlotTypes { get; private set; }
        public int[] Equipment { get; private set; }
        public int[] ItemDatas { get; private set; }
        public wRandom Random;

        public override void Init(ObjectDefinition def, Map map)
        {
            base.Init(def, map);

            Desc = AssetLibrary.GetPlayerDesc(def.ObjectType);

            SlotTypes = Desc.SlotTypes;
            Equipment = new int[SlotTypes.Length];
            ItemDatas = new int[SlotTypes.Length];
            for (var i = 0; i < Equipment.Length; i++)
            {
                Equipment[i] = -1;
                ItemDatas[i] = -1;
            }
        }

        protected override void UpdateStat(StatType statType, object value)
        {
            switch (statType)
            {
                case StatType.AccountId:
                    AccountId = (int) value;
                    break;
                case StatType.Exp:
                    Exp = (int) value;
                    break;
                case StatType.NextLevelExp:
                    NextLevelExp = (int) value;
                    break;
                case StatType.Level:
                    Level = (int) value;
                    break;
                case StatType.Fame:
                    Fame = (int) value;
                    break;
                case StatType.NextClassQuestFame:
                    NextClassQuestFame = (int) value;
                    break;
                case StatType.NumStars:
                    NumStars = (int) value;
                    break;
                case StatType.GuildName:
                    GuildName = (string) value;
                    break;
                case StatType.GuildRank:
                    GuildRank = (GuildRank) value;
                    break;
                case StatType.Credits:
                    Credits = (int) value;
                    break;
                case StatType.DyeLarge:
                    DyeLarge = (int) value;
                    break;
                case StatType.DyeSmall:
                    DyeSmall = (int) value;
                    break;
                case StatType.HasBackpack:
                    HasBackpack = (bool) value;
                    break;
                case StatType.Mp:
                    Mp = (int) value;
                    break;
                case StatType.MaxMp:
                    MaxMp = (int) value;
                    break;
                case StatType.Oxygen:
                    Oxygen = (int) value;
                    break;
                case StatType.HealthPotionStack:
                    HealthPotions = (int) value;
                    break;
                case StatType.MagicPotionStack:
                    MagicPotions = (int) value;
                    break;
                case StatType.SinkLevel:
                    SinkLevel = (int) value;
                    break;
                case StatType.Attack:
                    Attack = (int) value;
                    break;
                case StatType.Defense:
                    Defense = (int) value;
                    break;
                case StatType.Speed:
                    Speed = (int) value;
                    break;
                case StatType.Dexterity:
                    Dexterity = (int) value;
                    break;
                case StatType.Vitality:
                    Vitality = (int) value;
                    break;
                case StatType.Wisdom:
                    Wisdom = (int) value;
                    break;
                case StatType.Inventory0:
                case StatType.Inventory1:
                case StatType.Inventory2:
                case StatType.Inventory3:
                case StatType.Inventory4:
                case StatType.Inventory5:
                case StatType.Inventory6:
                case StatType.Inventory7:
                case StatType.Inventory8:
                case StatType.Inventory9:
                case StatType.Inventory10:
                case StatType.Inventory11:
                    Equipment[statType - StatType.Inventory0] = (int) value;
                    break;
                case StatType.ItemData0:
                case StatType.ItemData1:
                case StatType.ItemData2:
                case StatType.ItemData3:
                case StatType.ItemData4:
                case StatType.ItemData5:
                case StatType.ItemData6:
                case StatType.ItemData7:
                case StatType.ItemData8:
                case StatType.ItemData9:
                case StatType.ItemData10:
                case StatType.ItemData11:
                case StatType.ItemData12:
                case StatType.ItemData13:
                case StatType.ItemData14:
                case StatType.ItemData15:
                case StatType.ItemData16:
                case StatType.ItemData17:
                case StatType.ItemData18:
                case StatType.ItemData19:
                    ItemDatas[statType - StatType.ItemData0] = (int) value;
                    break;
            }
        }

        public float GetMovementSpeed()
        {
            if (HasConditionEffect(ConditionEffect.Paralyzed))
                return 0;
            
            if (HasConditionEffect(ConditionEffect.Slowed))
                return _MIN_MOVE_SPEED * MoveMultiplier;

            var ret = _MIN_MOVE_SPEED + Speed / 75f * (_MAX_MOVE_SPEED - _MIN_MOVE_SPEED);
            if (HasConditionEffect(ConditionEffect.Speedy))
            {
                ret *= 1.5f;
            }
            ret *= MoveMultiplier;
            return ret;
        }
        
        public float GetAttackFrequency()
        {
            if (HasConditionEffect(ConditionEffect.Dazed))
                return _MIN_ATTACK_FREQ;

            var ret = _MIN_ATTACK_FREQ + Dexterity / 75f * (_MAX_ATTACK_FREQ - _MIN_ATTACK_FREQ);
            if (HasConditionEffect(ConditionEffect.Berserk))
            {
                ret *= 1.5f;
            }
            return ret;
        }
        
        public float GetAttackMultiplier()
        {
            if (HasConditionEffect(ConditionEffect.Weak))
                return _MIN_ATTACK_MULT;

            var ret = _MIN_ATTACK_MULT + Attack / 75f * (_MAX_ATTACK_MULT - _MIN_ATTACK_MULT);
            if (HasConditionEffect(ConditionEffect.Damaging))
                ret *= 1.5f;
            return ret;
        }
    }
}