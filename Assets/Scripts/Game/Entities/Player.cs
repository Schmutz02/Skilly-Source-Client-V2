using Models;
using Models.Static;

namespace Game.Entities
{
    public class Player : Entity
    {
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

        protected void UpdateObjectStat(StatType statType, object value)
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
            }
        }
    }
}