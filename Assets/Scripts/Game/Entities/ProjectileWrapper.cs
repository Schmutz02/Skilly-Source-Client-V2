using System;
using UnityEngine;

namespace Game.Entities
{
    public class ProjectileWrapper : MonoBehaviour
    {
        private Projectile _projectile;

        [SerializeField]
        private SpriteRenderer _renderer;

        public void Init(Projectile projectile)
        {
            _projectile = projectile;
            _renderer.sprite = projectile.ExtraDesc.TextureData.Texture;
            SetRotation();
        }

        private void Update()
        {
            if (!_projectile.Tick(Time.time * 1000, out var newPos))
            {
                //TODO sfield map instead??
                _projectile.Owner.Owner.RemoveProjectile(_projectile);
                Destroy(gameObject);
            }
            transform.position = newPos;
            
            SetRotation();
        }

        private void SetRotation()
        {
            var rotation = _projectile.ExtraDesc.Rotation == 0 ? 0 : Time.time * 1000 / _projectile.ExtraDesc.Rotation;
            //TODO - camera angle
            rotation += _projectile.Angle + _projectile.ExtraDesc.AngleCorrection;
            transform.rotation = Quaternion.Euler(0, 0, rotation * Mathf.Rad2Deg);
        }
    }
}