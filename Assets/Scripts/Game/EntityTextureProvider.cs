using Game.Entities;
using Models.Static;
using UnityEngine;
using Utils;

namespace Game
{
    public class EntityTextureProvider : ITextureProvider
    {
        private const int _ATTACK_PERIOD = 500;
        private readonly Entity _entity;

        private float _facing;

        private Sprite _portrait;

        private readonly Sprite _texture;
        private readonly CharacterAnimation _animation;

        public EntityTextureProvider(Entity entity)
        {
            _entity = entity;
            _texture = entity.Desc.TextureData.Texture;
            _animation = entity.Desc.TextureData.Animation;
            var randomTextureData = entity.Desc.TextureData.RandomTextureData;
            if (randomTextureData != null)
            {
                var textureData = randomTextureData[entity.ObjectId % randomTextureData.Length];
                _texture = textureData.Texture;
                _animation = textureData.Animation;
            }
        }

        public Sprite GetTexture(int time)
        {
            Sprite image;
            if (_animation != null)
            {
                var p = 0f;
                var action = Action.Stand;
                if (time < _entity.AttackStart + _ATTACK_PERIOD)
                {
                    if (!_entity.Desc.DontFaceAttacks)
                    {
                        _facing = _entity.AttackAngle;
                    }

                    p = (time - _entity.AttackStart) % (float)_ATTACK_PERIOD / _ATTACK_PERIOD;
                    action = Action.Attack;
                }
                else if (_entity.Direction != Vector2.zero)
                {
                    var walkPer = 0.5f / (_entity.Direction.magnitude * 4);
                    walkPer = 400 - walkPer % 400;
                    _facing = Mathf.Atan2(_entity.Direction.y, _entity.Direction.x);
                    action = Action.Walk;
                    p = time % walkPer / walkPer;
                }

                image = _animation.ImageFromFacing(_facing, action, p);
            }
            else
            {
                image = _texture;
            }

            return SpriteUtils.Redraw(image, _entity.Size);
        }

        public Sprite GetPortrait()
        {
            if (_portrait == null)
            {
                var portraitTexture = _entity.Desc.Portrait != null
                    ? _entity.Desc.Portrait.Texture
                    : _entity.Desc.TextureData.Texture;
                var size = 4 / (int)portraitTexture.rect.width * 100;
                _portrait = SpriteUtils.Redraw(portraitTexture, size);
            }

            return _portrait;
        }
    }
}