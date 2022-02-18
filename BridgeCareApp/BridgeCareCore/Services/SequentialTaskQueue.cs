using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BridgeCareCore.Services
{
    /// <summary>
    ///     This queue does not support "hot" (i.e. started) tasks. If a task is added to this queue
    ///     and is started prior to this queue's consumer task reaching it and starting it, this
    ///     queue's consumer task will fail.
    /// </summary>
    public class SequentialTaskQueue : BlockingCollection<Task>
    {
        private bool Disposed;

        public SequentialTaskQueue() => Consumer = Task.Factory.StartNew(Consume, TaskCreationOptions.LongRunning);

        public Task Consumer { get; }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Consumer?.Dispose();
                }

                Disposed = true;
            }

            base.Dispose(disposing);
        }

        private void Consume()
        {
            foreach (var task in GetConsumingEnumerable())
            {
                task.RunSynchronously();
            }
        }
    }
}
