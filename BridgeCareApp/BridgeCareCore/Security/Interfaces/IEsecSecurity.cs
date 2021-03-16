using System.Collections.Generic;
using BridgeCareCore.Models;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Security.Interfaces
{
    public interface IEsecSecurity
    {
        void RevokeToken(string idToken);
        UserInfo GetUserInformation(HttpRequest request);
        UserInfo GetUserInformation(Dictionary<string, string> userInformationDictionary);
    }
}
