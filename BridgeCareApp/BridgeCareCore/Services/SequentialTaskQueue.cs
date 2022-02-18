using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BridgeCareCore.Services
{
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
