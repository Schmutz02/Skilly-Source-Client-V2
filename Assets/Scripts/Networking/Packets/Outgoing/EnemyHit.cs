namespace Networking.Packets.Outgoing
{
    public class EnemyHit : OutgoingPacket
    {
        public override PacketId Id => PacketId.EnemyHit;

        private readonly int _time;
        private readonly int _bulletId;
        private readonly int _targetId;

        public EnemyHit(int time, int bulletId, int targetId)
        {
            _time = time;
            _bulletId = bulletId;
            _targetId = targetId;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_time);
            wtr.Write(_bulletId);
            wtr.Write(_targetId);
        }
    }
}