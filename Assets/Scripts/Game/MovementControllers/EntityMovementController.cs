using Game.Entities;
using UnityEngine;

namespace Game.MovementControllers
{
    public class EntityMovementController : IMovementController
    {
        public Vector2 Direction { get; set; }
        public Vector2 TargetPosition;
        
        private readonly Entity _entity;
        
        public EntityMovementController(Entity entity)
        {
            _entity = entity;
            Direction = Vector2.zero;
            TargetPosition = Vector2.zero;
        }

        public void Tick(float deltaTime)
        {
            var moving = false;
            if (Direction != Vector2.zero)
            {
                var direction = Direction;
                var dx = direction.x * deltaTime;
                var dy = direction.y * deltaTime;
                var nextX = _entity.Position.x + dx;
                var nextY = _entity.Position.y + dy;

                if (direction.x > 0 && nextX > TargetPosition.x ||
                    direction.x < 0 && nextX < TargetPosition.x)
                {
                    nextX = TargetPosition.x;
                    direction.x = 0;
                }

                if (direction.y > 0 && nextY > TargetPosition.y ||
                    direction.y < 0 && nextY < TargetPosition.y)
                {
                    nextY = TargetPosition.y;
                    direction.y = 0;
                }

                Direction = direction;
                _entity.MoveTo(new Vector2(nextX, nextY));
                moving = true;
            }

            if (_entity.Desc.WhileMoving != null)
            {
                if (!moving)
                {
                    _entity.Position.z = _entity.Desc.Z;
                    _entity.Flying = _entity.Desc.Flying;
                }
                else
                {
                    _entity.Position.z = _entity.Desc.WhileMoving.Z;
                    _entity.Flying = _entity.Desc.WhileMoving.Flying;
                }
            }
        }
    }
}