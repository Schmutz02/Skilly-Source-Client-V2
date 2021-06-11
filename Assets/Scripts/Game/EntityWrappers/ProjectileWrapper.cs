using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    public class ProjectileWrapper : EntityWrapper
    {
        private Projectile _projectile;
        
        public override void Init(Entity projectile, bool rotating)
        {
            base.Init(projectile, false);

            _projectile = projectile as Projectile;

            transform.position = _projectile!.StartPosition;
            SetRotation();
        }

        protected override void Update()
        {
            if (!_projectile.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera))
            {
                //TODO sfield map instead??
                _projectile.Map.RemoveObject(_projectile.ObjectId);
                Destroy(gameObject);
            }
            transform.position = _projectile.Position;
            
            SetRotation();
        }

        private void SetRotation()
        {
            var spin = _projectile.Desc.Rotation == 0 ? 0 : GameTime.Time / _projectile.Desc.Rotation;
            spin += _projectile.Angle + _projectile.Desc.AngleCorrection;
            transform.rotation = Quaternion.Euler(0, 0, spin * Mathf.Rad2Deg);
        }
    }
}