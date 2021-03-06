namespace Tasks
{
    public abstract class BaseTask
    {
        public bool IsComplete { get; protected set; }

        public abstract void StartAsync();
    }
}