using Game;

namespace Networking.Packets.Incoming
{
    public class Text : IncomingPacket
    {
        public override PacketId Id => PacketId.Text;
        public override IncomingPacket CreateInstance() => new Text();

        private string _name;
        private int _objectId;
        private int _numStars;
        private byte _bubbleTime;
        private string _recipient;
        private string _text;
        
        public override void Read(PacketReader rdr)
        {
            _name = rdr.ReadString();
            _objectId = rdr.ReadInt32();
            _numStars = rdr.ReadInt32();
            _bubbleTime = rdr.ReadByte();
            _recipient = rdr.ReadString();
            _text = rdr.ReadString();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            //TODO
        }
    }
}