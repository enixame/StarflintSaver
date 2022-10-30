using System;
using System.Threading.Tasks;

namespace StarFlintSaver.Windows.ConcurrentTask
{
    public interface ITaskDispatcher : IDisposable
    {
        event EventHandler<Exception> OnError;
        void ExecuteTask(Func<Task> taskFunc);
    }
}