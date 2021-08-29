using Game;
using Models.Static;

namespace Networking.Packets.Incoming
{
    public class Damage : IncomingPacket
    {
        public override PacketId Id => PacketId.Damage;
        public override IncomingPacket CreateInstance() => new Damage();

        private int _targetId;
        private ConditionEffectDesc[] _effects;
        private short _damage;
        
        public override void Read(PacketReader rdr)
        {
            _targetId = rdr.ReadInt32();
            _effects = new ConditionEffectDesc[rdr.ReadByte()];
            for (var i = 0; i < _effects.Length; i++)
            {
                _effects[i] = new ConditionEffectDesc((ConditionEffect) (1L << rdr.ReadByte()), 0);
            }

            _damage = rdr.ReadInt16();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            var target = map.GetEntity(_targetId);
            if (target != null)
                target.Damage(_damage, _effects, null);
        }
    }
}