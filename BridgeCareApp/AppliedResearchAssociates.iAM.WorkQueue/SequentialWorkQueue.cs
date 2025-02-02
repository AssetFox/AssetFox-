﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Channels;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.WorkQueue.Logging;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;

namespace AppliedResearchAssociates.iAM.WorkQueue;

public class SequentialWorkQueue<T>
{
    public IReadOnlyList<IQueuedWorkHandle<T>> Snapshot => IncompleteElements.Values.ToList();

    public async Task<IWorkStarter?> Dequeue(CancellationToken cancellationToken)
    {
        var workItem = await ElementChannel.Reader.ReadAsync(cancellationToken);
        while (workItem != null && workItem.WorkCompletion.IsCanceled)
        {
            workItem = await ElementChannel.Reader.ReadAsync(cancellationToken);
        }
        return workItem;
    }

    public Task Enqueue(IWorkSpecification<T> workItem, out IQueuedWorkHandle<T> workHandle)
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

    public bool Cancel(string workId)
    {
        IQueuedWorkHandle<T>? queuedWorkHandle = IncompleteElements.Values.SingleOrDefault(_ => _.WorkId == workId);

        if (queuedWorkHandle != null)
        {
            if (!queuedWorkHandle.WorkHasStarted)
            {
                queuedWorkHandle.RemoveFromQueue(true);
                return true;
            }
            else
            {
                queuedWorkHandle.WorkCancellationTokenSource?.Cancel();
                return false;
            }
        }

        return true;
    }

    private readonly Channel<QueueElement> ElementChannel = Channel.CreateUnbounded<QueueElement>();

    private readonly ConcurrentDictionary<string, DateTime> EntryTimestampPerId = new();

    private readonly ConcurrentDictionary<string, QueueElement> IncompleteElements = new();

    internal class QueueElement : IQueuedWorkHandle<T>, IWorkStarter
    {
        public QueueElement(IWorkSpecification<T> workSpec, SequentialWorkQueue<T> workQueue)
        {
            WorkSpec = workSpec ?? throw new ArgumentNullException(nameof(workSpec));
            WorkQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));

            QueueEntryTimestamp = WorkQueue.EntryTimestampPerId[WorkId];

            _log = new WorkQueueNLog();

            _ = WorkQueue.IncompleteElements.TryAdd(WorkId, this);
        }

        public string MostRecentStatusMessage { get; private set; } = "";

        public DateTime QueueEntryTimestamp { get; }

        private ILog _log;

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

        public string UserId => WorkSpec.UserId;

        public string WorkDescription => WorkSpec.WorkDescription;

        public T Metadata => WorkSpec.Metadata;

        public Task WorkCompletion => WorkCompletionSource.Task;

        public string WorkId => WorkSpec.WorkId;

        public string WorkName => WorkSpec.WorkName;

        public DateTime? WorkStartTimestamp { get; private set; }

        public CancellationTokenSource? WorkCancellationTokenSource { get; private set; }

        public void StartWork(IServiceProvider serviceProvider)
        {
            _log = new WorkQueueNLog();
            if (WorkQueue.EntryTimestampPerId.ContainsKey(WorkId))
            {
                WorkStartTimestamp = DateTime.Now;
                WorkCancellationTokenSource = new CancellationTokenSource();
                var isCanceled = false;
                try
                {
                    WorkSpec.DoWork(
                        serviceProvider,
                        message => MostRecentStatusMessage = message,
                        WorkCancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    WorkCompletionSource.SetException(e);
                }
                finally
                {
                    isCanceled = WorkCancellationTokenSource.IsCancellationRequested;
                    WorkCancellationTokenSource.Dispose();
                }

                if (!WorkCompletion.IsFaulted)
                {
                    WorkCompletionSource.SetResult();
                    WorkSpec.OnCompletion(serviceProvider);
                }
                else
                {
                    _log.Error($"{WorkCompletion.Exception?.InnerException?.Message ?? ""}\r\n{WorkCompletion.Exception?.InnerException?.StackTrace ?? ""}");
                    WorkSpec.OnFault(serviceProvider, WorkCompletion.Exception?.InnerException?.Message ?? "");
                }

                RemoveFromQueue(false);
                if(serviceProvider != null)
                {
                    using var scope = serviceProvider.CreateScope();
                    var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
                    if (isCanceled)
                        _hubService.SendRealTimeMessage(WorkSpec.UserId, HubConstant.BroadcastTaskCompleted, $"Work queue operation '{WorkSpec.WorkDescription}' canceled");
                    WorkSpec.OnUpdate(serviceProvider);
                }             
            }
            else
            {
                WorkCompletionSource.SetCanceled();
            }
        }

        

        public void RemoveFromQueue(bool setCanceled)
        {
            _ = WorkQueue.IncompleteElements.TryRemove(WorkId, out _);
            _ = WorkQueue.EntryTimestampPerId.TryRemove(WorkId, out _);
            if (setCanceled)
            {
                WorkCompletionSource.SetCanceled();
            }
        }

        private readonly TaskCompletionSource WorkCompletionSource = new();

        private readonly IWorkSpecification<T> WorkSpec;

        private readonly SequentialWorkQueue<T> WorkQueue;
    }
}
