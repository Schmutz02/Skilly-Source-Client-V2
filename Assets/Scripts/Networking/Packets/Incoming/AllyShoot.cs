using Game;
using Game.Entities;
using Models.Static;
using UnityEngine;

namespace Networking.Packets.Incoming
{
    public class AllyShoot : IncomingPacket
    {
        public override PacketId Id => PacketId.AllyShoot;
        public override IncomingPacket CreateInstance() => new AllyShoot();

        private int _ownerId;
        private int _ownerType;
        private float _angle;
        
        public override void Read(PacketReader rdr)
        {
            _ownerId = rdr.ReadInt32();
            _ownerType = rdr.ReadInt16();
            _angle = rdr.ReadSingle();
        }

        public override void Handle(PacketHandler handler, Map map)
        {
            var owner = map.GetEntity(_ownerId);
            var weaponXml = AssetLibrary.GetItemDesc(_ownerType);
            var numShots = weaponXml.NumProjectiles;
            var arcGap = weaponXml.ArcGap * Mathf.Deg2Rad;
            var totalArc = arcGap * (numShots - 1);
            var angle = _angle - totalArc / 2;
            var startId = Projectile.NextFakeBulletId;
            Projectile.NextFakeBulletId += numShots;
            
            for (var i = 0; i < numShots; i++)
            {
                var projectile = new Projectile(owner, weaponXml.Projectile, startId + i, GameTime.Time, angle,
                    owner.Position, 0, map);

                map.AddObject(projectile, projectile.StartPosition);
                angle += arcGap;
            }

            owner.SetAttack(weaponXml, _angle);
        }
    }
}