namespace Networking.Packets.Outgoing
{
    public class SquareHit : OutgoingPacket
    {
        public override PacketId Id => PacketId.SquareHit;

        private readonly int _time;
        private readonly int _bulletId;

        public SquareHit(int time, int bulletId)
        {
            _time = time;
            _bulletId = bulletId;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_time);
            wtr.Write(_bulletId);
        }
    }
}