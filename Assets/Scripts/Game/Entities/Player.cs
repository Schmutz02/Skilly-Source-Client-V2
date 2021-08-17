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

        public new PlayerDesc Desc { get; }
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
        public int Speed { get; private set; }
        public int Dexterity { get; private set; }
        public int Vitality { get; private set; }
        public int Wisdom { get; private set; }
        public float PushX { get; private set; }
        public float PushY { get; private set; }
        public int MaxHpBoost { get; private set; }
        public int MaxMpBoost { get; private set; }
        public int AttackBoost { get; private set; }
        public int DefenseBoost { get; private set; }
        public int SpeedBoost { get; private set; }
        public int DexterityBoost { get; private set; }
        public int VitalityBoost { get; private set; }
        public int WisdomBoost { get; private set; }
        public ItemType[] SlotTypes { get; }
        public int[] Equipment { get; }
        public int[] ItemDatas { get; }
        public wRandom Random;
        private readonly PlayerShootController _shootController;
        public int AttackPeriod;

        public Player(PlayerDesc desc, int objectId, bool isMyPlayer, Map map) : base(desc, objectId, isMyPlayer, map)
        {
            Desc = desc;
            
            if (isMyPlayer)
                _shootController = new PlayerShootController(this);
            
            SlotTypes = Desc.SlotTypes;
            Equipment = new int[SlotTypes.Length];
            ItemDatas = new int[SlotTypes.Length];
            for (var i = 0; i < Equipment.Length; i++)
            {
                Equipment[i] = -1;
                ItemDatas[i] = -1;
            }
        }

        public override bool Tick(int time, int dt, Camera camera)
        {
            base.Tick(time, dt, camera);
            
            _shootController?.Tick(time, camera);

            return true;
        }

        public void OnMove()
        {
            var tile = Map.GetTile(Position);
            if (tile.Desc.Sinking)
            {
                SinkLevel = (int) Mathf.Min(SinkLevel + 1, _MAX_SINK_LEVEL);
                MoveMultiplier = 0.1f + (1 - SinkLevel / _MAX_SINK_LEVEL) * (tile.Desc.Speed - 0.1f);
            }
            else
            {
                SinkLevel = 0;
                MoveMultiplier = tile.Desc.Speed;
            }

            if (tile.Desc.Damage > 0 && !HasConditionEffect(ConditionEffect.Invincible))
            {
                if (tile.StaticObject == null || !tile.StaticObject.Desc.ProtectFromGroundDamage)
                {
                    //TODO damage player
                }
            }

            if (tile.Desc.Push)
            {
                PushX = tile.Desc.DX;
                PushX = tile.Desc.DY;
            }
            else
            {
                PushX = 0;
                PushY = 0;
            }
        }

        public override void SetAttack(ItemDesc container, float attackAngle)
        {
            var itemData = ItemDatas[0];
            var rateOfFireMod = ItemDesc.GetStat(itemData, ItemData.RateOfFire, ItemDesc.RATE_OF_FIRE_MULTIPLIER);
            var rateOfFire = container.RateOfFire;
            
            rateOfFire *= 1 + rateOfFireMod;
            AttackPeriod = (int)(1 / GetAttackFrequency() * (1 / rateOfFire));
            base.SetAttack(container, attackAngle);
        }

        protected override void UpdateStat(StatType statType, object value)
        {
            base.UpdateStat(statType, value);
            
            switch (statType)
            {
                case StatType.AccountId:
                    AccountId = (int) value;
                    return;
                case StatType.Exp:
                    Exp = (int) value;
                    return;
                case StatType.NextLevelExp:
                    NextLevelExp = (int) value;
                    return;
                case StatType.Level:
                    Level = (int) value;
                    return;
                case StatType.Fame:
                    Fame = (int) value;
                    return;
                case StatType.NextClassQuestFame:
                    NextClassQuestFame = (int) value;
                    return;
                case StatType.NumStars:
                    NumStars = (int) value;
                    return;
                case StatType.GuildName:
                    GuildName = (string) value;
                    return;
                case StatType.GuildRank:
                    GuildRank = (GuildRank) value;
                    return;
                case StatType.Credits:
                    Credits = (int) value;
                    return;
                case StatType.DyeLarge:
                    DyeLarge = (int) value;
                    return;
                case StatType.DyeSmall:
                    DyeSmall = (int) value;
                    return;
                case StatType.HasBackpack:
                    HasBackpack = (bool) value;
                    return;
                case StatType.Mp:
                    Mp = (int) value;
                    return;
                case StatType.MaxMp:
                    MaxMp = (int) value;
                    return;
                case StatType.Oxygen:
                    Oxygen = (int) value;
                    return;
                case StatType.HealthPotionStack:
                    HealthPotions = (int) value;
                    return;
                case StatType.MagicPotionStack:
                    MagicPotions = (int) value;
                    return;
                case StatType.SinkLevel:
                    SinkLevel = (int) value;
                    return;
                case StatType.Attack:
                    Attack = (int) value;
                    return;
                case StatType.Defense:
                    Defense = (int) value;
                    return;
                case StatType.Speed:
                    Speed = (int) value;
                    return;
                case StatType.Dexterity:
                    Dexterity = (int) value;
                    return;
                case StatType.Vitality:
                    Vitality = (int) value;
                    return;
                case StatType.Wisdom:
                    Wisdom = (int) value;
                    return;
                case StatType.MaxHpBoost:
                    MaxHpBoost = (int) value;
                    return;
                case StatType.MaxMpBoost:
                    MaxMpBoost = (int) value;
                    return;
                case StatType.AttackBoost:
                    AttackBoost = (int) value;
                    return;
                case StatType.DefenseBoost:
                    DefenseBoost = (int) value;
                    return;
                case StatType.SpeedBoost:
                    SpeedBoost = (int) value;
                    return;
                case StatType.DexterityBoost:
                    DexterityBoost = (int) value;
                    return;
                case StatType.VitalityBoost:
                    VitalityBoost = (int) value;
                    return;
                case StatType.WisdomBoost:
                    WisdomBoost = (int) value;
                    return;
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
                    return;
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
                    return;
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