using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    public class ProjectileWrapper : EntityWrapper
    {
        private Projectile _projectile;
        
        public override void Init(Entity projectile)
        {
            base.Init(projectile);

            _projectile = projectile as Projectile;

            transform.position = _projectile!.StartPosition;
        }

        protected override void Update()
        {
            if (!_projectile.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera))
            {
                //TODO sfield map instead??
                _projectile.Map.RemoveProjectile(_projectile);
                Destroy(gameObject);
            }
            transform.position = _projectile.Position;
            
            SetRotation();
        }

        private void SetRotation()
        {
            var rotation = _projectile.Desc.Rotation == 0 ? 0 : GameTime.Time / _projectile.Desc.Rotation;
            //TODO - camera angle
            rotation += _projectile.Angle + _projectile.Desc.AngleCorrection;
            transform.rotation = Quaternion.Euler(0, 0, rotation * Mathf.Rad2Deg);
        }
    }
}