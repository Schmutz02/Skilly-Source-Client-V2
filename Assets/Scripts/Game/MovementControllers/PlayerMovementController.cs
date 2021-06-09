using System;
using Game.Entities;
using Models;
using Models.Static;
using UnityEngine;

namespace Game.MovementControllers
{
    public class PlayerMovementController : IMovementController
    {
        private const float _MOVE_THRESHOLD = 0.4f;
        
        private readonly Player _player;
        private readonly Transform _entityTransform;
        
        public PlayerMovementController(Player player)
        {
            _player = player;
            _entityTransform = player.transform;
        }
        
        public void Tick(float deltaTime)
        {
            var rotate = KeyToInt(KeyCode.Q) - KeyToInt(KeyCode.E);
            var xVelocity = KeyToInt(KeyCode.D) - KeyToInt(KeyCode.A);
            var yVelocity = KeyToInt(KeyCode.W) - KeyToInt(KeyCode.S);

            if (_player.HasConditionEffect(ConditionEffect.Confused))
            {
                var temp = xVelocity;
                xVelocity = -yVelocity;
                yVelocity = -temp;
                rotate = -rotate;
            }
            
            var playerAngle = _entityTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            if (rotate != 0)
            {
                playerAngle += deltaTime * Settings.PLAYER_ROTATE_SPEED * rotate;
                _entityTransform.rotation = Quaternion.Euler(0, 0, playerAngle * Mathf.Rad2Deg);
            }

            var direction = Vector2.zero;
            if (xVelocity != 0 || yVelocity != 0)
            {
                var moveSpeed = _player.GetMovementSpeed();
                var moveVecAngle = Mathf.Atan2(yVelocity, xVelocity);
                direction.x = moveSpeed * Mathf.Cos(playerAngle + moveVecAngle);
                direction.y = moveSpeed * Mathf.Sin(playerAngle + moveVecAngle);
            }

            if (_player.PushX != 0 || _player.PushY != 0)
            {
                direction.x -= _player.PushX;
                direction.y -= _player.PushY;
            }

            ValidateAndMove((Vector2)_entityTransform.position + deltaTime * direction);
        }

        private void ValidateAndMove(Vector2 pos)
        {
            pos = ResolveNewLocation(pos);
            _player.MoveTo(pos);
        }
        
        private Vector2 ResolveNewLocation(Vector2 pos)
        {
            if (_player.HasConditionEffect(ConditionEffect.Paralyzed))
                return pos;

            var dx = pos.x - _entityTransform.position.x;
            var dy = pos.y - _entityTransform.position.y;

            if (dx < _MOVE_THRESHOLD && 
                dx > -_MOVE_THRESHOLD && 
                dy < _MOVE_THRESHOLD &&
                dy > -_MOVE_THRESHOLD)
            {
                return CalcNewLocation(pos);
            }

            var ds = _MOVE_THRESHOLD / Math.Max(Math.Abs(dx), Math.Abs(dy));
            var tds = 0f;

            pos = _entityTransform.position;
            var done = false;
            while (!done)
            {
                if (tds + ds >= 1)
                {
                    ds = 1 - tds;
                    done = true;
                }

                pos = CalcNewLocation(new Vector2(pos.x + dx * ds, pos.y + dy * ds));
                tds += ds;
            }

            return pos;
        }
        
        private Vector2 CalcNewLocation(Vector2 pos)
        {
            var fx = 0f;
            var fy = 0f;

            var isFarX = _entityTransform.position.x % .5f == 0 && pos.x != _entityTransform.position.x || (int)(_entityTransform.position.x / .5f) != (int)(pos.x / .5f);
            var isFarY = _entityTransform.position.y % .5f == 0 && pos.y != _entityTransform.position.y || (int)(_entityTransform.position.y / .5f) != (int)(pos.y / .5f);

            if (!isFarX && !isFarY || _player.Owner.RegionUnblocked(pos.x, pos.y))
            {
                return pos;
            }

            if (isFarX)
            {
                fx = pos.x > _entityTransform.position.x ? (int)(pos.x * 2) / 2f : (int)(_entityTransform.position.x * 2) / 2f;
                if ((int)fx > (int)_entityTransform.position.x)
                    fx -= 0.01f;
            }

            if (isFarY)
            {
                fy = pos.y > _entityTransform.position.y ? (int)(pos.y * 2) / 2f : (int)(_entityTransform.position.y * 2) / 2f;
                if ((int)fy > (int)_entityTransform.position.y)
                    fy -= 0.01f;
            }

            if (!isFarX)
            {
                pos.y = fy;
                return pos;
            }

            if (!isFarY)
            {
                pos.x = fx;
                return pos;
            }

            var ax = pos.x > _entityTransform.position.x ? pos.x - fx : fx - pos.x;
            var ay = pos.y > _entityTransform.position.y ? pos.y - fy : fy - pos.y;
            if (ax > ay)
            {
                if (_player.Owner.RegionUnblocked(pos.x, fy))
                {
                    pos.y = fy;
                    return pos;
                }

                if (_player.Owner.RegionUnblocked(fx, pos.y))
                {
                    pos.x = fx;
                    return pos;
                }
            }
            else
            {
                if (_player.Owner.RegionUnblocked(fx, pos.y))
                {
                    pos.x = fx;
                    return pos;
                }
               
                if (_player.Owner.RegionUnblocked(pos.x, fy))
                {
                    pos.y = fy;
                    return pos;
                }
            }

            pos.x = fx;
            pos.y = fy;
            return pos;
        }

        private int KeyToInt(KeyCode keyCode)
        {
            return Input.GetKey(keyCode) ? 1 : 0;
        }
    }
}