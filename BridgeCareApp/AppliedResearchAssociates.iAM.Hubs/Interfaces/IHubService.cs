using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Hubs.Interfaces
{
    public interface IHubService
    {
        Dictionary<string,string> errorList { get; }

        void SendRealTimeMessage(string username, string method, object arg);

        void SendRealTimeMessage(string username, string method, object arg1, object arg2);

        void SendRealTimeErrorMessage(string username, string arg, Exception e);

        void SendRealTimeErrorMessage(string username, string arg1, string arg2, Exception e);
    }
}
