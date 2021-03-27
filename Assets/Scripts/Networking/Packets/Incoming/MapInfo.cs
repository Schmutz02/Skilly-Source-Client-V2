using Game;
using Models;
using Networking.Packets.Outgoing;

namespace Networking.Packets.Incoming
{
    public class MapInfo : IncomingPacket
    {
        public override PacketId Id => PacketId.MapInfo;
        
        public override IncomingPacket CreateInstance() => new MapInfo();

        public override void Read(PacketReader rdr)
        {
            
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            if (Account.GameInitData.NewCharacter)
            {
                        
            }
            else
            {
                TcpTicker.Send(new Load(Account.GameInitData.CharId)); 
            }
        }
    }
}