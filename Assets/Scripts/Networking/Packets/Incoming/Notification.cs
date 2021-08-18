using Game;
using UnityEngine;
using Utils;

namespace Networking.Packets.Incoming
{
    public class Notification : IncomingPacket
    {
        public override PacketId Id => PacketId.Notification;
        public override IncomingPacket CreateInstance() => new Notification();

        private int _objectId;
        private string _text;
        private Color _color;
        
        public override void Read(PacketReader rdr)
        {
            _objectId = rdr.ReadInt32();
            _text = rdr.ReadString();
            _color = ParseUtils.ColorFromUInt((uint) rdr.ReadInt32());
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            var entity = map.GetEntity(_objectId);
            if (entity == null)
                return;
            
            map.Overlay.AddStatusText(entity, _text, _color, 2000);
            //TODO quest complete
        }
    }
}