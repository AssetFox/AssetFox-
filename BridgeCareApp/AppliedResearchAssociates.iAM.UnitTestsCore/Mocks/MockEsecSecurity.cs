using System.Collections.Generic;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Mocks
{
    public class MockEsecSecurity : IEsecSecurity
    {
        public void RevokeToken(string idToken) => throw new System.NotImplementedException();

        public UserInfo GetUserInformation(HttpRequest request) =>
            new UserInfo
            {
                Name = "pdsystbamsusr02",
                Role = "PD-BAMS-Administrator",
                Email = "pdstseseca5@pa.gov"
            };

        public UserInfo GetUserInformation(Dictionary<string, string> userInformationDictionary) => throw new System.NotImplementedException();
    }
}
