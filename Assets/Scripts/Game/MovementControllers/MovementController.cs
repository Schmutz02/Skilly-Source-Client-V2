using UnityEngine;

namespace Game.MovementControllers
{
    public interface IMovementController
    {
        Vector2 Direction { get; }

        public abstract void Tick(float deltaTime);
    }
}