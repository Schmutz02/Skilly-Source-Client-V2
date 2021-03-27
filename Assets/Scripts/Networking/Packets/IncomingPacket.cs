using Game;

namespace Networking.Packets
{
    public abstract class IncomingPacket
    {
        public abstract PacketId Id { get; }
        
        public abstract IncomingPacket CreateInstance();
        
        public abstract void Read(PacketReader rdr);
        public abstract void Handle(PacketHandler handler, Map map);
    }
}