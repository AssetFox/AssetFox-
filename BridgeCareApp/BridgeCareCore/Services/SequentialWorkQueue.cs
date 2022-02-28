using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BridgeCareCore.Services
{
    public class SequentialWorkQueue
    {
        public async Task<IWorkItem> Dequeue(CancellationToken cancellationToken) => await Elements.Reader.ReadAsync(cancellationToken);

        public Task Enqueue(IWorkItem workItem, out IQueuedWorkHandle workHandle)
        {
            if (!EntryTimestampPerId.TryAdd(workItem.WorkId, DateTime.Now))
            {
                throw new ArgumentException($"Work with ID \"{workItem.WorkId}\" is already queued or running.", nameof(workItem));
            }

            var queueElement = new QueueElement(workItem, this);
            workHandle = queueElement;
            return Elements.Writer.WriteAsync(queueElement).AsTask();
        }

        private readonly Channel<QueueElement> Elements = Channel.CreateUnbounded<QueueElement>();

        private readonly ConcurrentDictionary<string, DateTime> EntryTimestampPerId = new();

        private class QueueElement : IWorkItem, IQueuedWorkHandle
        {
            public QueueElement(IWorkItem workItem, SequentialWorkQueue workQueue)
            {
                WorkItem = workItem ?? throw new ArgumentNullException(nameof(workItem));
                WorkQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));

                QueueEntryTimestamp = WorkQueue.EntryTimestampPerId[WorkItem.WorkId];
            }

            public DateTime QueueEntryTimestamp { get; }

            public int QueueIndex
            {
                get
                {
                    var queueSnapshot = WorkQueue.EntryTimestampPerId.ToArray();
                    Array.Sort(queueSnapshot, (kv1, kv2) => kv1.Value.CompareTo(kv2.Value));
                    var index = Array.FindIndex(queueSnapshot, kv => kv.Key == WorkId);
                    return index;
                }
            }

            public Task WorkCompletion => WorkCompletionSource.Task;

            public bool WorkHasStarted { get; private set; }

            public string WorkId => WorkItem.WorkId;

            public void DoWork(IServiceProvider serviceProvider)
            {
                if (WorkQueue.EntryTimestampPerId.ContainsKey(WorkId))
                {
                    WorkHasStarted = true;

                    try
                    {
                        WorkItem.DoWork(serviceProvider);
                    }
                    catch (Exception e)
                    {
                        WorkCompletionSource.SetException(e);
                    }

                    if (!WorkCompletion.IsFaulted)
                    {
                        WorkCompletionSource.SetResult();
                    }

                    RemoveFromQueue();
                }
                else
                {
                    WorkCompletionSource.SetCanceled();
                }
            }

            public void RemoveFromQueue() => _ = WorkQueue.EntryTimestampPerId.TryRemove(WorkId, out _);

            private readonly TaskCompletionSource WorkCompletionSource = new();

            private readonly IWorkItem WorkItem;

            private readonly SequentialWorkQueue WorkQueue;
        }
    }
}
