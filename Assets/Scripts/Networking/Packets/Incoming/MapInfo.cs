using System;
using Game;
using Models;
using Networking.Packets.Outgoing;

namespace Networking.Packets.Incoming
{
    public class MapInfo : IncomingPacket
    {
        public override PacketId Id => PacketId.MapInfo;
        public override IncomingPacket CreateInstance() => new MapInfo();

        private int _width;
        private int _height;
        private string _name;
        private string _displayName;
        private uint _seed;
        private int _background;
        private bool _showDisplays;
        private bool _allowTeleport;
        private string _music;

        public override void Read(PacketReader rdr)
        {
            _width = rdr.ReadInt32();
            _height = rdr.ReadInt32();
            _name = rdr.ReadString();
            _displayName = rdr.ReadString();
            _seed = rdr.ReadUInt32();
            _background = rdr.ReadInt32();
            _showDisplays = rdr.ReadBoolean();
            _allowTeleport = rdr.ReadBoolean();
            _music = rdr.ReadString();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            handler.Random = new wRandom(_seed);
            
            if (handler.NewCharacter)
            {
                        
            }
            else
            {
                TcpTicker.Send(new Load(handler.CharId)); 
            }
        }
    }
}