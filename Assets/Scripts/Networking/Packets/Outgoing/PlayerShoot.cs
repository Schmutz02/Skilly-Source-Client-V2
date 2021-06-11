using UnityEngine;

namespace Networking.Packets.Outgoing
{
    public class PlayerShoot : OutgoingPacket
    {
        public override PacketId Id => PacketId.PlayerShoot;

        private readonly int _time;
        private readonly Vector2 _startingPosition;
        private readonly float _angle;
        private readonly bool _ability;
        private readonly byte _numShots;

        public PlayerShoot(int time, Vector2 startingPosition, float angle, bool ability, int numShots)
        {
            _time = time;
            _startingPosition = startingPosition;
            _angle = angle;
            _ability = ability;
            _numShots = (byte)numShots;
        }

        public override void Write(PacketWriter wtr)
        {
            wtr.Write(_time);
            wtr.Write(_startingPosition.x);
            wtr.Write(_startingPosition.y);
            wtr.Write(_angle);
            wtr.Write(_ability);
            if (_numShots != 1)
            {
                wtr.Write(_numShots);
            }
        }
    }
}