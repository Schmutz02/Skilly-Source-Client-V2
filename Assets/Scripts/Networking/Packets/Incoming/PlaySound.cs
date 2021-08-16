using Game;

namespace Networking.Packets.Incoming
{
    public class PlaySound : IncomingPacket
    {
        public override PacketId Id => PacketId.PlaySound;
        public override IncomingPacket CreateInstance() => new PlaySound();

        private string _sound;
        
        public override void Read(PacketReader rdr)
        {
            _sound = rdr.ReadString();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            //TODO
        }
    }
}