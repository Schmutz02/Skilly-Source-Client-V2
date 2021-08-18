using Game;
using UnityEngine;
using Utils;

namespace Networking.Packets.Incoming
{
    public class ShowEffect : IncomingPacket
    {
        public override PacketId Id => PacketId.ShowEffect;
        public override IncomingPacket CreateInstance() => new ShowEffect();

        private byte _effectType;
        private int _objectId;
        private Color _color;
        private Vector2 _pos1;
        private Vector2 _pos2;
        
        public override void Read(PacketReader rdr)
        {
            _effectType = rdr.ReadByte();
            _objectId = rdr.ReadInt32();
            _color = ParseUtils.ColorFromUInt((uint) rdr.ReadInt32());
            _pos1 = new Vector2()
            {
                x = rdr.ReadSingle(),
                y = rdr.ReadSingle()
            };

            if (rdr.PeekChar() != -1)
            {
                _pos2 = new Vector2()
                {
                    x = rdr.ReadSingle(),
                    y = rdr.ReadSingle()
                };
            }
            else
            {
                _pos2 = Vector2.zero;
            }
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            //TODO
        }
    }
}