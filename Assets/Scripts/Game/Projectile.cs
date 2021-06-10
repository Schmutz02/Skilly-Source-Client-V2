using System.Collections.Generic;
using Game.Entities;
using Models.Static;
using UnityEngine;
using MathUtils = Utils.MathUtils;

namespace Game
{
    public class Projectile
    {
        public readonly Entity Owner;
        public readonly ProjectileDesc Desc;
        public readonly ObjectDesc ExtraDesc;
        public readonly int BulletId;
        public readonly float Angle;
        public readonly Vector2 StartPosition;
        public readonly int Damage;
        public readonly HashSet<int> Hit;

        public float StartTime;

        public Projectile(Entity owner, ProjectileDesc desc, int bulletId, float startTime, float angle, Vector2 startPos, int damage)
        {
            Owner = owner;
            Desc = desc;
            ExtraDesc = AssetLibrary.GetObjectDesc(desc.ObjectId);
            BulletId = bulletId;
            StartTime = startTime;
            Angle = MathUtils.BoundToPI(angle);
            StartPosition = startPos;
            Damage = damage;
            Hit = new HashSet<int>();
        }

        public bool CanHit(Entity en)
        {
            if (en.HasConditionEffect(ConditionEffect.Invincible) || en.HasConditionEffect(ConditionEffect.Stasis))
                return false;

            if (!Hit.Contains(en.ObjectId))
            {
                Hit.Add(en.ObjectId);
                return true;
            }
            return false;
        }

        public bool Tick(float time, out Vector2 position)
        {
            position = Vector2.zero;
            var elapsed = time - StartTime;
            if (elapsed > Desc.LifetimeMS)
            {
                return false;
            }

            position = PositionAt(elapsed);
            return true;
        }

        private Vector2 PositionAt(float elapsed)
        {
            var p = new Vector2(StartPosition.x, StartPosition.y);
            var speed = Desc.Speed;
            if (Desc.Accelerate) speed *= elapsed / Desc.LifetimeMS;
            if (Desc.Decelerate) speed *= 2 - elapsed / Desc.LifetimeMS;
            var dist = elapsed * (speed / 10000f);
            var phase = BulletId % 2 == 0 ? 0 : Mathf.PI;
            if (Desc.Wavy)
            {
                const float periodFactor = 6 * Mathf.PI;
                const float amplitudeFactor = Mathf.PI / 64.0f;
                var theta = Angle + amplitudeFactor * Mathf.Sin(phase + periodFactor * elapsed / 1000.0f);
                p.x += dist * Mathf.Cos(theta);
                p.y += dist * Mathf.Sin(theta);
            }
            else if (Desc.Parametric)
            {
                var t = elapsed / Desc.LifetimeMS * 2 * Mathf.PI;
                var x = Mathf.Sin(t) * (BulletId % 2 == 1 ? 1 : -1);
                var y = Mathf.Sin(2 * t) * (BulletId % 4 < 2 ? 1 : -1);
                var sin = Mathf.Sin(Angle);
                var cos = Mathf.Cos(Angle);
                p.x += (x * cos - y * sin) * Desc.Magnitude;
                p.y += (x * sin + y * cos) * Desc.Magnitude;
            }
            else
            {
                if (Desc.Boomerang)
                {
                    var halfway = Desc.LifetimeMS * (Desc.Speed / 10000) / 2;
                    if (dist > halfway)
                    {
                        dist = halfway - (dist - halfway);
                    }
                }
                p.x += dist * Mathf.Cos(Angle);
                p.y += dist * Mathf.Sin(Angle);
                if (Desc.Amplitude != 0)
                {
                    var deflection = Desc.Amplitude * Mathf.Sin(phase + elapsed / Desc.LifetimeMS * Desc.Frequency * 2 * Mathf.PI);
                    p.x += deflection * Mathf.Cos(Angle + Mathf.PI / 2);
                    p.y += deflection * Mathf.Sin(Angle + Mathf.PI / 2);
                }
            }

            return p;
        }
    }
}