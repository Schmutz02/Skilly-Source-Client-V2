namespace Networking.Packets.Outgoing
{
    public class GotoAck : OutgoingPacket
    {
        public override PacketId Id => PacketId.GotoAck;

        private readonly int _time;

        public GotoAck(int time)
        {
            _time = time;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_time);
        }
    }
}