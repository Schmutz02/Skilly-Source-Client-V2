namespace Networking.Packets.Outgoing
{
    public class Escape : OutgoingPacket
    {
        public override PacketId Id => PacketId.Escape;

        public override void Write(PacketWriter wtr)
        {
        }
    }
}