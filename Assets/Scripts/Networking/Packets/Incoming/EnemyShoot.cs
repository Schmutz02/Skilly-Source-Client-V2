using Game;
using Networking.Packets.Outgoing;
using UnityEngine;

namespace Networking.Packets.Incoming
{
    public class EnemyShoot : IncomingPacket
    {
        public override PacketId Id => PacketId.EnemyShoot;
        public override IncomingPacket CreateInstance() => new EnemyShoot();

        private int _bulletId;
        private int _ownerId;
        private byte _bulletType;
        private Vector2 _startPos;
        private float _angle;
        private short _damage;
        private byte _numShots;
        private float _angleInc;
        
        public override void Read(PacketReader rdr)
        {
            _bulletId = rdr.ReadInt32();
            _ownerId = rdr.ReadInt32();
            _bulletType = rdr.ReadByte();
            _startPos = new Vector2()
            {
                x = rdr.ReadSingle(),
                y = rdr.ReadSingle()
            };
            _angle = rdr.ReadSingle();
            _damage = rdr.ReadInt16();
            if (rdr.PeekChar() != -1)
            {
                _numShots = rdr.ReadByte();
                _angleInc = rdr.ReadSingle();
            }
            else
            {
                _numShots = 1;
                _angleInc = 0;
            }
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            TcpTicker.Send(new ShootAck(GameTime.Time));
            //TODO
        }
    }
}