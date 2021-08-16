namespace Networking.Packets.Outgoing
{
    public class Create : OutgoingPacket
    {
        public override PacketId Id => PacketId.Create;

        private readonly int _classType;
        private readonly int _skinType;
        
        public Create(int classType, int skinType)
        {
            _classType = classType;
            _skinType = skinType;
        }
        
        public override void Write(PacketWriter wtr)
        {
            wtr.Write((short) _classType);
            wtr.Write((short) _skinType);
        }
    }
}