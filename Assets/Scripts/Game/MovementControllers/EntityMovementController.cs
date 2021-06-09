using Game.Entities;
using UnityEngine;

namespace Game.MovementControllers
{
    public class EntityMovementController : IMovementController
    {
        public Vector2 Direction;
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
            if (Direction == Vector2.zero)
                return;

            var dx = Direction.x * deltaTime;
            var dy = Direction.y * deltaTime;
            var nextX = _entity.transform.position.x + dx;
            var nextY = _entity.transform.position.y + dy;

            if (Direction.x > 0 && nextX > TargetPosition.x ||
                Direction.x < 0 && nextX < TargetPosition.x)
            {
                nextX = TargetPosition.x;
                Direction.x = 0;
            }
            
            if (Direction.y > 0 && nextY > TargetPosition.y ||
                Direction.y < 0 && nextY < TargetPosition.y)
            {
                nextY = TargetPosition.y;
                Direction.y = 0;
            }
            
            _entity.MoveTo(new Vector2(nextX, nextY));

            //TODO add flying thing with z
        }
    }
}