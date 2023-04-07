using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.WorkQueue;
using Microsoft.SqlServer.Dac.Model;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using BridgeCareCore.Services.Aggregation;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Hubs;
using BridgeCareCore.Models;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Reporting.Logging;

namespace BridgeCareCore.Services
{
    public record aggregationWorkitem(Guid NetworkId, string userId, string networkName, List<AttributeDTO> attributes) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => NetworkId.ToString();

        public DateTime StartTime { get; set; }

        public string UserId => userId;

        public string WorkDescription => "Network Aggregation";

        public string WorkName => networkName;

        public WorkQueueMetadata Metadata => new WorkQueueMetadata() { DomainType = DomainType.Network, WorkType = WorkType.Aggregation };

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            string AggregationError = "Aggregation Error";
            using var scope = serviceProvider.CreateScope(); 
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _aggregationService = scope.ServiceProvider.GetRequiredService<IAggregationService>();
            var _log = scope.ServiceProvider.GetRequiredService<ILog>();
            var channel = Channel.CreateUnbounded<AggregationStatusMemo>();
            var state = new AggregationState();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle);
            var timer = KeepUserInformedOfState(state);
            var readTask = Task.Run(() => ReadMessages(channel.Reader));
            
            try
            {
                _aggregationService.AggregateNetworkData(channel.Writer, NetworkId, state, attributes, cancellationToken).Wait();
            }
            catch (Exception e)
            {
                state.Status = "Aggregation failed";
                _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(NetworkId, state.Status);
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO { NetworkId = NetworkId, Status = state.Status }, 0.0);
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"{AggregationError}::AggregateNetworkData - {e.Message}");
                throw;
            }
            finally
            {
                NotifyUserOfState(state);
                timer.Stop();
                timer.Close();
                channel.Writer.Complete();
                //readTask.Dispose();
            }

            void DoublyBroadcastError(string broadcastError)
            {
                
                _log.Error(broadcastError);
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, broadcastError);
            }

            async Task ReadMessages(ChannelReader<AggregationStatusMemo> reader)
            {
                while (await reader.WaitToReadAsync())
                {
                    while (reader.TryRead(out AggregationStatusMemo status))
                    {
                        var error = status.ErrorMessage;
                        if (error != null)
                        {
                            DoublyBroadcastError(error);
                        }
                    }
                }
            }

            void NotifyUserOfState(AggregationState state)
            {
                try
                {
                    SendCurrentStatusMessage(state);
                }
                catch
                {
                    // ignore the error
                }
            }

            System.Timers.Timer KeepUserInformedOfState(AggregationState state)
            {
                var timer = new System.Timers.Timer(3000);
                timer.Elapsed += delegate
                {
                    SendCurrentStatusMessage(state);
                };
                timer.Start();
                return timer;
            }
            void SendCurrentStatusMessage(AggregationState state)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                var count = state.Count % 10;
                var periods = StringWithPeriods(count + 1);
                var message = $"{state.Status}{periods}";

                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO { NetworkId = state.NetworkId, Status = message }, state.Percentage);

                var queueMessage = state.Status;
                if (state.Percentage > 0)
                    queueMessage = queueMessage + $": {state.Percentage}%";
                _queueLogger.UpdateWorkQueueStatus(NetworkId, queueMessage);
                state.Count++;
            }

            static string StringWithPeriods(int count)
            {
                var builder = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    builder.Append('.');
                }
                return builder.ToString();
            }
        }
    }
}

