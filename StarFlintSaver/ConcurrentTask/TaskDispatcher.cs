using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StarFlintSaver.Windows.ConcurrentTask
{
    public sealed class TaskDispatcher : ITaskDispatcher
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ChannelWriter<Func<Task>> _channelWriter;
        private readonly int _throttlingTimeInSeconds;
        private Func<Task> _taskToExecute;

        public event EventHandler<Exception> OnError;

        public TaskDispatcher(int throttlingTimeInSeconds = 1)
        {
            _throttlingTimeInSeconds = throttlingTimeInSeconds;

            var channel = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions { SingleReader = true });
            ChannelReader<Func<Task>> reader = channel.Reader;
            _channelWriter = channel.Writer;

            Task.Run(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    await _semaphoreSlim.WaitAsync();
                    try
                    {
                        while (reader.TryRead(out Func<Task> taskAction))
                        {
                            Volatile.Write(ref _taskToExecute, taskAction);
                        }
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

                    try
                    {
                        await ExecuteTaskWithThrottlingAsync();
                    }
                    catch (Exception exception)
                    {
                        RaiseOnError(exception);
                    }
                }
            });
        }

        public void ExecuteTask(Func<Task> TaskAction)
        {
            _channelWriter?.TryWrite(TaskAction);
        }

        private async Task ExecuteTaskWithThrottlingAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var task = Volatile.Read(ref _taskToExecute);
                if (task != null)
                {
                    await task.Invoke();
                    await Task.Delay(_throttlingTimeInSeconds * 1000); // throttling time   
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private void RaiseOnError(Exception exception)
        {
            OnError.Invoke(this, exception);
        }

        public void Dispose()
        {
            _channelWriter?.Complete();
        }
    }
}
