namespace Networking.Packets
{
    public abstract class OutgoingPacket
    {
        public abstract PacketId Id { get; }
        public abstract void Write(PacketWriter wtr);
    }
}