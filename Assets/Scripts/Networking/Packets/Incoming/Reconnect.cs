using System;
using Game;

namespace Networking.Packets.Incoming
{
    public class Reconnect : IncomingPacket
    {
        public static Action<int, int, bool> OnReconnect;
        
        public override PacketId Id => PacketId.Reconnect;
        public override IncomingPacket CreateInstance() => new Reconnect();

        private int _worldId;

        public override void Read(PacketReader rdr)
        {
            _worldId = rdr.ReadInt32();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            OnReconnect?.Invoke(_worldId, handler.CharId, handler.NewCharacter);
        }
    }
}