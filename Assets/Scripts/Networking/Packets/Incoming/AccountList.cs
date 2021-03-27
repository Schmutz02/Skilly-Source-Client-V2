using Game;

namespace Networking.Packets.Incoming
{
    public class AccountList : IncomingPacket
    {
        public override PacketId Id => PacketId.AccountList;
        public override IncomingPacket CreateInstance() => new AccountList();

        public override void Read(PacketReader rdr)
        {
            
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            
        }
    }
}