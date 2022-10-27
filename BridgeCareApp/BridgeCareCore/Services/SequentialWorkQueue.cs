using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class SequentialWorkQueue
    {
        public IReadOnlyList<IQueuedWorkHandle> Snapshot => IncompleteElements.Values.ToList();

        public async Task<IWorkItem> Dequeue(CancellationToken cancellationToken) => await ElementChannel.Reader.ReadAsync(cancellationToken);

        public Task Enqueue(IWorkItem workItem, out IQueuedWorkHandle workHandle)
        {
            lock (ElementChannel)
            {
                if (!EntryTimestampPerId.TryAdd(workItem.WorkId, DateTime.Now))
                {
                    throw new ArgumentException($"Work with ID \"{workItem.WorkId}\" is already queued or running.", nameof(workItem));
                }

                var queueElement = new QueueElement(workItem, this);
                workHandle = queueElement;
                return ElementChannel.Writer.WriteAsync(queueElement).AsTask();
            }
        }

        private readonly Channel<QueueElement> ElementChannel = Channel.CreateUnbounded<QueueElement>();

        private readonly ConcurrentDictionary<string, DateTime> EntryTimestampPerId = new();

        private readonly ConcurrentDictionary<string, QueueElement> IncompleteElements = new();

        private class QueueElement : IWorkItem, IQueuedWorkHandle
        {
            public QueueElement(IWorkItem workItem, SequentialWorkQueue workQueue)
            {
                WorkItem = workItem ?? throw new ArgumentNullException(nameof(workItem));
                WorkQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));

                QueueEntryTimestamp = WorkQueue.EntryTimestampPerId[WorkId];

                _ = WorkQueue.IncompleteElements.TryAdd(WorkId, this);
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

            public UserInfo UserInfo => WorkItem.UserInfo;

            public Task WorkCompletion => WorkCompletionSource.Task;

            public string WorkId => WorkItem.WorkId;

            public DateTime? WorkStartTimestamp { get; private set; }

            public void DoWork(IServiceProvider serviceProvider)
            {
                if (WorkQueue.EntryTimestampPerId.ContainsKey(WorkId))
                {
                    WorkStartTimestamp = DateTime.Now;

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
                    else
                    {
                        // Send a message
                        using var scope = serviceProvider.CreateScope();
                        var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var message = new SimulationAnalysisDetailDTO()
                        {
                            SimulationId = Guid.Parse(WorkItem.WorkId),
                            Status = $"Run Failed. {WorkCompletion.Exception.InnerException.Message}"
                        };
                        _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, message);
                    }

                    RemoveFromQueue();
                }
                else
                {
                    WorkCompletionSource.SetCanceled();
                }
            }

            public void RemoveFromQueue()
            {
                _ = WorkQueue.IncompleteElements.TryRemove(WorkId, out _);
                _ = WorkQueue.EntryTimestampPerId.TryRemove(WorkId, out _);
            }

            private readonly TaskCompletionSource WorkCompletionSource = new();

            private readonly IWorkItem WorkItem;

            private readonly SequentialWorkQueue WorkQueue;
        }
    }
}
