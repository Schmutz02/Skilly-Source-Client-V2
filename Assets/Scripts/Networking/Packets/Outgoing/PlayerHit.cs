namespace Networking.Packets.Outgoing
{
    public class PlayerHit : OutgoingPacket
    {
        public override PacketId Id => PacketId.PlayerHit;

        private readonly int _bulletId;

        public PlayerHit(int bulletId)
        {
            _bulletId = bulletId;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_bulletId);
        }
    }
}