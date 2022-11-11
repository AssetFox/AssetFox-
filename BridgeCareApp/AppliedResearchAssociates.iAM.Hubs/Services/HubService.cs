using System.Collections.Generic;
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
                {"InvestmentDefaultData", "Configuration read error." }
            };
        }

        Dictionary<string, string> IHubService.errorList => _errorList;//throw new System.NotImplementedException();

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
