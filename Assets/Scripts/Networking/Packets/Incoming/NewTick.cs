using Game;

namespace Networking.Packets.Incoming
{
    public class NewTick : IncomingPacket
    {
        public override PacketId Id => PacketId.NewTick;
        public override IncomingPacket CreateInstance() => new NewTick();

        public override void Read(PacketReader rdr)
        {
            
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            
        }
    }
}