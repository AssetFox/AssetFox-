using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeCareCore.Security
{
    public class GraphApiClientService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GraphApiClientService(IConfiguration configuration)
        {
            var azureAdB2CSection = configuration.GetSection("AzureAdB2C");
            if (azureAdB2CSection == null)
            {
                return;
            }

            string[] scopes = azureAdB2CSection.GetValue<string>("GraphApi:Scopes")?.Split(' ');
            var tenantId = azureAdB2CSection.GetValue<string>("GraphApi:TenantId");

            // Values from app registration
            var clientId = azureAdB2CSection.GetValue<string>("GraphApi:ClientId");
            var clientSecret = azureAdB2CSection.GetValue<string>("GraphApi:ClientSecret");

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            _graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);
        }

        public async Task<List<string>> GetGraphApiUserMemberGroup(string userId)
        {
            var groupNames = new List<string>();

            var test = await _graphServiceClient.Users[userId].MemberOf.GetAsync();
            // TODO groupNames
            return groupNames;
        }
    }
}
