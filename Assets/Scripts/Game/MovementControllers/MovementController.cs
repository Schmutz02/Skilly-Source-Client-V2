using UnityEngine;

namespace Game.MovementControllers
{
    public interface IMovementController
    {
        Vector3 Direction { get; }

        public abstract void Tick(float deltaTime);
    }
}