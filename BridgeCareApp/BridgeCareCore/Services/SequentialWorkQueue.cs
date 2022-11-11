using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Text;
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

        public async Task<IWorkItem> Dequeue(CancellationToken cancellationToken)
        {
            var workItem = await ElementChannel.Reader.ReadAsync(cancellationToken);
            while (workItem != null && workItem.WorkCompletion.IsCanceled)
            {
                workItem = await ElementChannel.Reader.ReadAsync(cancellationToken);
            }
            return workItem;
        }

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

        public bool Cancel(Guid workId)
        {
            IQueuedWorkHandle queuedWorkHandle = IncompleteElements.Values.SingleOrDefault(_ => Guid.Parse(_.WorkId) == workId);

            if (queuedWorkHandle != null)
            {
                if (!queuedWorkHandle.WorkHasStarted)
                {
                    queuedWorkHandle.RemoveFromQueue(true);
                    return true;
                }
                else
                {
                    queuedWorkHandle.WorkCancellationTokenSource.Cancel();
                    return false;
                }
            }
            return true;
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

            public CancellationTokenSource WorkCancellationTokenSource { get; private set; }

            public void DoWork(IServiceProvider serviceProvider, CancellationToken cancellationToken)
            {
                if (WorkQueue.EntryTimestampPerId.ContainsKey(WorkId))
                {
                    WorkStartTimestamp = DateTime.Now;
                    WorkCancellationTokenSource = new CancellationTokenSource();

                    try
                    {
                        WorkItem.DoWork(serviceProvider, WorkCancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        WorkCompletionSource.SetException(e);
                    }
                    finally
                    {
                        WorkCancellationTokenSource.Dispose();
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
                            Status = $"Run Failed. {WorkCompletion.Exception.InnerException.Message}",
                            LastRun = DateTime.Now
                        };
                        _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastSimulationAnalysisDetail, message);
                        _unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(message);
                    }

                    RemoveFromQueue();
                }
                else
                {
                    WorkCompletionSource.SetCanceled();
                }
            }

            public void RemoveFromQueue(bool setCanceled = false)
            {
                _ = WorkQueue.IncompleteElements.TryRemove(WorkId, out _);
                _ = WorkQueue.EntryTimestampPerId.TryRemove(WorkId, out _);
                if (setCanceled)
                {
                    WorkCompletionSource.SetCanceled();
                }
            }

            private readonly TaskCompletionSource WorkCompletionSource = new();

            private readonly IWorkItem WorkItem;

            private readonly SequentialWorkQueue WorkQueue;
        }
    }
}
