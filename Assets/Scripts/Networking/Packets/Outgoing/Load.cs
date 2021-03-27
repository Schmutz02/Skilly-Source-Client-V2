namespace Networking.Packets.Outgoing
{
    public class Load : OutgoingPacket
    {
        public override PacketId Id => PacketId.Load;

        private readonly int _charId;

        public Load(int charId)
        {
            _charId = charId;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_charId);
        }
    }
}