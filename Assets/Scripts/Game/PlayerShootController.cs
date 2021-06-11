using Game.Entities;
using Models.Static;
using UnityEngine;

namespace Game
{
    public class PlayerShootController
    {
        private readonly Player _player;

        private float _attackPeriod;
        private float _attackStart;
        private float _time;
        
        private int _nextProjectileId;

        public PlayerShootController(Player player)
        {
            _player = player;
        }

        public void Tick(float time, Camera camera)
        {
            _time = time;
            
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                var playerPosition = camera.WorldToScreenPoint(_player.Position);
                var angle = Mathf.Atan2(mousePosition.y - playerPosition.y, mousePosition.x - playerPosition.x);
                TryShoot(angle + _player.Rotation);
            }
        }

        private void TryShoot(float attackAngle)
        {
            if (_player.HasConditionEffect(ConditionEffect.Stunned))
                return;

            var weaponType = _player.Equipment[0];
            var itemData = _player.ItemDatas[0];
            if (weaponType == -1)
                return;

            var weaponXml = AssetLibrary.GetItemDesc(weaponType);
            var rateOfFireMod = ItemDesc.GetStat(itemData, ItemData.RateOfFire, ItemDesc.RATE_OF_FIRE_MULTIPLIER);
            var rateOfFire = weaponXml.RateOfFire;
            
            rateOfFire *= 1 + rateOfFireMod;
            _attackPeriod = 1 / _player.GetAttackFrequency() * (1 / rateOfFire);
            if (_time < _attackStart + _attackPeriod)
                return;
            
            _attackStart = _time;
            Shoot(_attackStart, weaponType, itemData, weaponXml, attackAngle, false);
        }

        private void Shoot(float time, int weaponType, int itemData, ItemDesc weaponXml, float attackAngle,
            bool isAbility)
        {
            var numShots = weaponXml.NumProjectiles;
            var arcGap = weaponXml.ArcGap * Mathf.Deg2Rad;
            var totalArc = arcGap * (numShots - 1);
            var angle = attackAngle - totalArc / 2;
            var damageMod = ItemDesc.GetStat(itemData, ItemData.Damage, ItemDesc.DAMAGE_MULTIPLIER);
            var startId = _nextProjectileId;
            _nextProjectileId -= numShots;
            
            for (var i = 0; i < numShots; i++)
            {
                var minDamage = weaponXml.Projectile.MinDamage + (int)(weaponXml.Projectile.MinDamage * damageMod);
                var maxDamage = weaponXml.Projectile.MaxDamage + (int)(weaponXml.Projectile.MaxDamage * damageMod);
                var damage = (int)(_player.Random.NextIntRange((uint) minDamage, (uint) maxDamage) *
                             _player.GetAttackMultiplier());
                var projectile = new Projectile(_player, weaponXml.Projectile, startId - i, time, angle,
                    _player.Position, damage, _player.Map);

                _player.Map.AddObject(projectile, projectile.StartPosition);
                angle += arcGap;
            }
            
            // TODO TcpTicker.Send(new PlayerShoot());
        }
    }
}