using Game;
using Models;
using Networking.WebRequestHandlers;
using UI;

namespace Networking.Packets.Incoming
{
    public class Death : IncomingPacket
    {
        public override PacketId Id => PacketId.Death;
        public override IncomingPacket CreateInstance() => new Death();

        private int _accountId;
        private int _charId;
        private string _killer;
        
        public override void Read(PacketReader rdr)
        {
            _accountId = rdr.ReadInt32();
            _charId = rdr.ReadInt32();
            _killer = rdr.ReadString();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            ViewManager.Instance.ChangeView(View.Death, this);
        }
    }
}