using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    public class ProjectileWrapper : EntityWrapper
    {
        private Projectile _projectile;
        
        public override void Init(Entity entity, bool rotating = true)
        {
            base.Init(entity, false);

            _projectile = entity as Projectile;

            transform.position = _projectile!.StartPosition;
            Renderer.sprite = Entity.Desc.TextureData.GetTexture(entity.ObjectId);
            
            SetRotation();
        }

        public override bool Tick()
        {
            if (!_projectile.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera))
            {
                return false;
            }
            transform.position = new Vector3(_projectile.Position.x, _projectile.Position.y, -_projectile.Z);
            
            SetRotation();
            return true;
        }

        private void SetRotation()
        {
            var spin = _projectile.Desc.Rotation == 0 ? 0 : GameTime.Time / _projectile.Desc.Rotation;
            spin += _projectile.Angle + _projectile.Desc.AngleCorrection;
            transform.rotation = Quaternion.Euler(0, 0, spin * Mathf.Rad2Deg);
        }
    }
}