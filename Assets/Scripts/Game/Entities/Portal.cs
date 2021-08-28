using Models.Static;
using UI.GameScreen.Panels;

namespace Game.Entities
{
    public partial class Portal : Entity, IInteractiveObject
    {
        public bool NexusPortal { get; private set; }
        public bool LockedPortal { get; private set; }
        public bool Active { get; private set; } = true;

        protected override void Init(ObjectDesc desc, int objectId, bool isMyPlayer, Map map, bool rotating = true)
        {
            base.Init(desc, objectId, false, map, rotating);
            
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