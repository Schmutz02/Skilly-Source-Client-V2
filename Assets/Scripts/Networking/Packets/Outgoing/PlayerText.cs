namespace Networking.Packets.Outgoing
{
    public class PlayerText : OutgoingPacket
    {
        public override PacketId Id => PacketId.PlayerText;

        private readonly string _text;

        public PlayerText(string text)
        {
            _text = text;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_text);
        }
    }
}