using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AppliedResearchAssociates.iAM.Hubs.Services
{
    public class HubService : IHubService
    {
        private readonly IHubContext<BridgeCareHub> _hubContext;

        public Dictionary<string, string> _errorList;

        //public HubService(IHubContext<BridgeCareHub> hubContext) => _hubContext = hubContext;
        public HubService(IHubContext<BridgeCareHub> hubContext)
        {
            _hubContext = hubContext;
            _errorList = new Dictionary<string, string>()
            {
                {"Exception", "An exception has occurred." },
                {"SimulationException", "An error has occurred during the simulation." },
                {"InvalidAttributeException", "An invalid attribute upsert occurred." },
                {"InvalidAttributeUpsertException", "An invalid attribute with aggregation rule." },
                {"InvalidOperationException", "An invalid operation occurred." },
                {"CalculateEvaluateException", "Expression could not be compiled." },
                {"Unauthorized", "Unauthorized to access this component." },
                {"AnalysisDefaultData", "Configuration read error." },
                {"InvestmentDefaultData", "Configuration read error." },
                {"RowNotInTable", "Invalid entry into the database." }
            };
        }

        Dictionary<string, string> IHubService.errorList => _errorList;//throw new System.NotImplementedException();

        public void SendRealTimeErrorMessage(string username, string arg, Exception e)
        {
            if (string.IsNullOrEmpty(username))
            {
                _hubContext?.Clients?.All?.SendAsync(HubConstant.BroadcastError, arg, e.StackTrace);
            }
            else
            {
                _hubContext?.Clients?.Group(username)?.SendAsync(HubConstant.BroadcastError, arg, e.StackTrace);
            }
            var edi = ExceptionDispatchInfo.Capture(e);
            edi.Throw();
        }
        public void SendRealTimeErrorMessage(string username, string arg1, string arg2, Exception e)
        {
            if (string.IsNullOrEmpty(username))
            {
                _hubContext?.Clients?.All?.SendAsync(HubConstant.BroadcastError, arg1, e.StackTrace, arg2);               
            }
            else
            {
                _hubContext?.Clients?.Group(username)?.SendAsync(HubConstant.BroadcastError, arg1, e.StackTrace, arg2);
            }
            var edi = ExceptionDispatchInfo.Capture(e);
            edi.Throw();
        }

        public void SendRealTimeMessage(string username, string method, object arg)
        {
            if (string.IsNullOrEmpty(username))
            {
                _hubContext?.Clients?.All?.SendAsync(method, arg);
            }
            else
            {
                _hubContext?.Clients?.Group(username)?.SendAsync(method, arg);
            }
        }

        public void SendRealTimeMessage(string username, string method, object arg1, object arg2)
        {
            if (string.IsNullOrEmpty(username))
            {
                _hubContext?.Clients?.All?.SendAsync(method, arg1, arg2);
            }
            else
            {
                _hubContext?.Clients?.Group(username)?.SendAsync(method, arg1, arg2);
            }
        }
    }
}
