using Models.Static;

namespace Game.Entities
{
    public class Portal : Entity
    {
        public readonly bool NexusPortal;
        public readonly bool LockedPortal;
        
        public bool Active { get; private set; }
        
        public Portal(ObjectDesc desc, int objectId, Map map) : base(desc, objectId, false, map)
        {
            NexusPortal = desc.NexusPortal;
            LockedPortal = desc.LockedPortal;
        }

        protected override void UpdateStat(StatType stat, object value)
        {
            base.UpdateStat(stat, value);

            switch (stat)
            {
                case StatType.Active:
                    Active = (int) value != 0;
                    break;
            }
        }
    }
}