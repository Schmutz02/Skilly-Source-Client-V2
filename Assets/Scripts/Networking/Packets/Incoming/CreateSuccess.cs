using Game;

namespace Networking.Packets.Incoming
{
    public class CreateSuccess : IncomingPacket
    {
        public override PacketId Id => PacketId.CreateSuccess;
        public override IncomingPacket CreateInstance() => new CreateSuccess();

        private int _objectId;
        private int _charId;

        public override void Read(PacketReader rdr)
        {
            _objectId = rdr.ReadInt32();
            _charId = rdr.ReadInt32();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            handler.PlayerId = _objectId;
        }
    }
}