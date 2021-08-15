namespace Networking.Packets.Outgoing
{
    public class ShootAck : OutgoingPacket
    {
        public override PacketId Id => PacketId.ShootAck;

        private int _time;

        public ShootAck(int time)
        {
            _time = time;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_time);
        }
    }
}