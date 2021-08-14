using Game.Entities;
using Models.Static;
using UnityEngine;
using Utils;

namespace Game
{
    public class PlayerTextureProvider : ITextureProvider
    {
        private readonly Player _player;
        private float _facing;
        private Sprite _portrait;
        
        public PlayerTextureProvider(Player player)
        {
            _player = player;
        }
        
        public Sprite GetTexture(int time)
        {
            var action = Action.Stand;
            var p = 0f;
            if (time < _player.AttackStart + _player.AttackPeriod)
            {
                _facing = _player.AttackAngle;
                p = (time - _player.AttackStart) % (float)_player.AttackPeriod / _player.AttackPeriod;
                action = Action.Attack;
            }
            else if (_player.Direction != Vector2.zero)
            {
                var walkPer = (int)(3.5f / _player.GetMovementSpeed());
                _facing = Mathf.Atan2(_player.Direction.y, _player.Direction.x);
                p = time % walkPer / walkPer;
                action = Action.Walk;
            }

            var image = _player.Desc.TextureData.Animation.ImageFromFacing(_facing, action, p);
            return SpriteUtils.Redraw(image, _player.Size);
        }

        public Sprite GetPortrait()
        {
            if (_portrait == null)
            {
                var image = _player.Desc.TextureData.Animation.ImageFromDir(Facing.Right, Action.Stand, 0);
                var size = 4 / (int)image.rect.width * 100;
                _portrait = SpriteUtils.Redraw(image, size);
            }

            return _portrait;
        }
    }
}