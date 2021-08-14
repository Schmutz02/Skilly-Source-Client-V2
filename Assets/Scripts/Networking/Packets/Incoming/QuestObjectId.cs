using Game;

namespace Networking.Packets.Incoming
{
    public class QuestObjectId : IncomingPacket
    {
        public override PacketId Id => PacketId.QuestObjId;
        public override IncomingPacket CreateInstance() => new QuestObjectId();

        private int _objectId;
        
        public override void Read(PacketReader rdr)
        {
            _objectId = rdr.ReadInt32();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            //TODO
        }
    }
}