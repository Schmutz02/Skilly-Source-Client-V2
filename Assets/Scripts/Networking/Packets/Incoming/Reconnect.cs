using System;
using Game;
using Models;

namespace Networking.Packets.Incoming
{
    public class Reconnect : IncomingPacket
    {
        public static Action<GameInitData> OnReconnect;
        
        public override PacketId Id => PacketId.Reconnect;
        public override IncomingPacket CreateInstance() => new Reconnect();

        private int _worldId;

        public override void Read(PacketReader rdr)
        {
            _worldId = rdr.ReadInt32();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            var newInitData = new GameInitData(
                _worldId,
                handler.InitData.CharId,
                false,
                handler.InitData.ClassType,
                handler.InitData.SkinType);
            
            OnReconnect?.Invoke(newInitData);
        }
    }
}