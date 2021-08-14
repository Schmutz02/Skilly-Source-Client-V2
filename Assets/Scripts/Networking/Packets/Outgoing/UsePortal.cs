namespace Networking.Packets.Outgoing
{
    public class UsePortal : OutgoingPacket
    {
        public override PacketId Id => PacketId.UsePortal;

        private readonly int _portalId;
        
        public UsePortal(int portalId)
        {
            _portalId = portalId;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_portalId);
        }
    }
}