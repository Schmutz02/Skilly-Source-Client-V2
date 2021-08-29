using Game;
using UnityEngine;

namespace Networking.Packets.Incoming
{
    public class Goto : IncomingPacket
    {
        public override PacketId Id => PacketId.Goto;
        public override IncomingPacket CreateInstance() => new Goto();

        private int _objectId;
        private Vector3 _pos;
        
        public override void Read(PacketReader rdr)
        {
            _objectId = rdr.ReadInt32();
            _pos = new Vector3()
            {
                x = rdr.ReadSingle(),
                y = rdr.ReadSingle()
            };
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            var entity = map.GetEntity(_objectId);
            if (entity == null)
                return;
            
            if (_objectId == handler.PlayerId)
                map.GotosRequested++;
            
            entity.OnGoto(_pos);
        }
    }
}