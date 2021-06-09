namespace Game.MovementControllers
{
    public interface IMovementController
    {
        public abstract void Tick(float deltaTime);
    }
}