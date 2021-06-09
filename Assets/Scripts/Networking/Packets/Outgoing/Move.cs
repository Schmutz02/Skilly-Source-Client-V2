using UnityEngine;

namespace Networking.Packets.Outgoing
{
    public class Move : OutgoingPacket
    {
        public override PacketId Id => PacketId.Move;

        private readonly int _time;
        private readonly Vector2 _position;

        public Move(int time, Vector2 position)
        {
            _time = time;
            _position = position;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_time);
            wtr.Write(_position.x);
            wtr.Write(_position.y);
        }
    }
}