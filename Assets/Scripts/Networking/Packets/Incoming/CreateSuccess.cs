using Game;

namespace Networking.Packets.Incoming
{
    public class CreateSuccess : IncomingPacket
    {
        public override PacketId Id => PacketId.CreateSuccess;
        public override IncomingPacket CreateInstance() => new CreateSuccess();

        public override void Read(PacketReader rdr)
        {
            
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            
        }
    }
}