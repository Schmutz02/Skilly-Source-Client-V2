using System.Collections.Generic;
using Models.Static;
using Networking;
using Networking.Packets.Outgoing;
using UnityEngine;
using MathUtils = Utils.MathUtils;

namespace Game.Entities
{
    public class Projectile : Entity
    {
        public static int NextFakeBulletId;
        
        public readonly Entity Owner;
        public readonly ProjectileDesc ProjectileDesc;
        public readonly int BulletId;
        public readonly float Angle;
        public readonly Vector2 StartPosition;
        private readonly int _damage;
        public readonly HashSet<int> Hit;
        public readonly bool DamagesPlayers;
        public readonly bool DamagesEnemies;

        public float StartTime;

        public Projectile(Entity owner, ProjectileDesc projectileDesc, int bulletId, float startTime, float angle, 
            Vector2 startPos, int damage, Map map) 
            : base(AssetLibrary.GetObjectDesc(projectileDesc.ObjectId), GetNextFakeObjectId(), false, map)
        {
            Owner = owner;
            ProjectileDesc = projectileDesc;
            BulletId = bulletId;
            StartTime = startTime;
            Angle = MathUtils.BoundToPI(angle);
            StartPosition = Position = startPos;
            _damage = damage;
            DamagesPlayers = owner.Desc.Enemy;
            DamagesEnemies = !DamagesPlayers;
            var size = projectileDesc.Size;
            if (size < 0)
            {
                size = Desc.Size;
            }
            Size = 8 * (size / 100);
            Hit = new HashSet<int>();
        }

        public bool CanHit(Entity en)
        {
            if (en.HasConditionEffect(ConditionEffect.Invincible) || en.HasConditionEffect(ConditionEffect.Stasis))
                return false;

            return !Hit.Contains(en.ObjectId);
        }

        public override bool Tick(int time, int dt, Camera camera)
        {
            base.Tick(time, dt, camera);
            
            var elapsed = time - StartTime;
            if (elapsed > ProjectileDesc.LifetimeMS)
            {
                return false;
            }

            if (!MoveTo(PositionAt(elapsed)) || Square.Type == 255)
            {
                if (DamagesPlayers)
                {
                    TcpTicker.Send(new SquareHit(GameTime.Time, BulletId));
                }
                else if (Square.StaticObject != null)
                {
                    //TODO square hit effect
                }
                return false;
            }

            if (Square.StaticObject != null && (!Square.StaticObject.Desc.Enemy || !DamagesEnemies) &&
                (Square.StaticObject.Desc.EnemyOccupySquare ||
                 !ProjectileDesc.PassesCover && Square.StaticObject.Desc.OccupySquare))
            {
                if (DamagesPlayers)
                {
                    TcpTicker.Send(new SquareHit(GameTime.Time, BulletId));
                }
                else
                {
                    //TODO square hit effect
                }
                return false;
            }

            var target = GetHit();
            if (target == null)
                return true;

            var player = Map.MyPlayer;
            var playerExists = player != null;
            var isTargetEnemy = target.Desc.Enemy;
            var sendMessage = playerExists && (DamagesPlayers || isTargetEnemy && Owner.ObjectId == player.ObjectId);
            if (sendMessage)
            {
                var damage = DamageWithDefense(target, _damage, ProjectileDesc.ArmorPiercing);
                target.Damage(damage, ProjectileDesc.Effects, this);
                if (target == player)
                {
                    TcpTicker.Send(new PlayerHit(BulletId));
                }
                else if (isTargetEnemy)
                {
                    TcpTicker.Send(new EnemyHit(GameTime.Time, BulletId, target.ObjectId));
                }
            }

            if (ProjectileDesc.MultiHit)
            {
                Hit.Add(target.ObjectId);
            }
            else
            {
                return false;
            }
            return true;
        }

        public override bool MoveTo(Vector2 position)
        {
            var square = Map.GetTile(position);
            if (square is null)
                return false;

            Position = position;
            Square = square;
            return true;
        }

        private Vector2 PositionAt(float elapsed)
        {
            var p = new Vector2(StartPosition.x, StartPosition.y);
            var speed = ProjectileDesc.Speed;
            if (ProjectileDesc.Accelerate) speed *= elapsed / ProjectileDesc.LifetimeMS;
            if (ProjectileDesc.Decelerate) speed *= 2 - elapsed / ProjectileDesc.LifetimeMS;
            var dist = elapsed * (speed / 10000f);
            var phase = BulletId % 2 == 0 ? 0 : Mathf.PI;
            if (ProjectileDesc.Wavy)
            {
                const float periodFactor = 6 * Mathf.PI;
                const float amplitudeFactor = Mathf.PI / 64.0f;
                var theta = Angle + amplitudeFactor * Mathf.Sin(phase + periodFactor * elapsed / 1000.0f);
                p.x += dist * Mathf.Cos(theta);
                p.y += dist * Mathf.Sin(theta);
            }
            else if (ProjectileDesc.Parametric)
            {
                var t = elapsed / ProjectileDesc.LifetimeMS * 2 * Mathf.PI;
                var x = Mathf.Sin(t) * (BulletId % 2 == 1 ? 1 : -1);
                var y = Mathf.Sin(2 * t) * (BulletId % 4 < 2 ? 1 : -1);
                var sin = Mathf.Sin(Angle);
                var cos = Mathf.Cos(Angle);
                p.x += (x * cos - y * sin) * ProjectileDesc.Magnitude;
                p.y += (x * sin + y * cos) * ProjectileDesc.Magnitude;
            }
            else
            {
                if (ProjectileDesc.Boomerang)
                {
                    var halfway = ProjectileDesc.LifetimeMS * (ProjectileDesc.Speed / 10000) / 2;
                    if (dist > halfway)
                    {
                        dist = halfway - (dist - halfway);
                    }
                }
                p.x += dist * Mathf.Cos(Angle);
                p.y += dist * Mathf.Sin(Angle);
                if (ProjectileDesc.Amplitude != 0)
                {
                    var deflection = ProjectileDesc.Amplitude * Mathf.Sin(phase + elapsed / ProjectileDesc.LifetimeMS * ProjectileDesc.Frequency * 2 * Mathf.PI);
                    p.x += deflection * Mathf.Cos(Angle + Mathf.PI / 2);
                    p.y += deflection * Mathf.Sin(Angle + Mathf.PI / 2);
                }
            }

            return p;
        }

        private Entity GetHit()
        {
            var minDistSquared = float.MaxValue;
            Entity minEn = null;
            
            if (DamagesEnemies)
            {
                foreach (var wrapper in Map.Entities.Values)
                {
                    var entity = wrapper?.Entity;
                    if (entity == null || !entity.Desc.Enemy || !CanHit(entity))
                        continue;
                    
                    if (Mathf.Abs(Position.x - entity.Position.x) <= _HITBOX_RADIUS &&
                        Mathf.Abs(Position.y - entity.Position.y) <= _HITBOX_RADIUS)
                    {
                        var distSquared = MathUtils.DistanceSquared(Position, entity.Position);
                        if (distSquared < minDistSquared)
                        {
                            minDistSquared = distSquared;
                            minEn = entity;
                        }
                    }
                }
            }
            else if (DamagesPlayers)
            {
                var player = Map.MyPlayer;
                if (CanHit(player))
                {
                    if (Mathf.Abs(Position.x - player.Position.x) <= _HITBOX_RADIUS &&
                        Mathf.Abs(Position.y - player.Position.y) <= _HITBOX_RADIUS)
                    {
                        return player;
                    }
                }
            }

            return minEn;
        }
    }
}