using Azure.Identity;
using BridgeCareCore.Security.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BridgeCareCore.Security
{
    public class GraphApiClientService: IGraphApiClientService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GraphApiClientService(IConfiguration configuration)
        {
            var azureAdB2CSection = configuration.GetSection("AzureAdB2C");
            if (azureAdB2CSection.Value == null)
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
            var objectCollectionResponse = await _graphServiceClient.Users[userId].MemberOf.GetAsync();
            var result = objectCollectionResponse.Value;

            foreach (var group in result.Cast<Group>())
            {
                groupNames.Add(group.DisplayName);
            }

            return groupNames;
        }
    }
}
