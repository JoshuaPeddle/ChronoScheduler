
namespace ChronoScheduler
{
    public interface ITask<TArgs> where TArgs : class
    {
        void Execute(TArgs args);
    }
}
