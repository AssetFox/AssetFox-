using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.GraphQL
{
    public class Query
    {
        [UseFiltering]
        [UseSorting]
        //https://chillicream.com/docs/hotchocolate/v13/security/authorization/#policies
        //https://www.learmoreseekmore.com/2021/03/part3-hotchocolate-graphql-auth-series-purecodefirst-validatingjwt-different-authorization-techniques.html
        //[Authorize(Roles = new[] { "Administrator", "Editor", "SimulationPowerUser", "ReadOnly", "Default" })]
        //[Authorize(Policy = "claim-policy-1")]
        //[Authorize(Policy = BridgeCareCore.Security.SecurityConstants.Policy.ViewGraphQL)]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = BridgeCareCore.Security.SecurityConstants.Policy.ViewGraphQL)]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, [Service(ServiceKind.Synchronized)] IHttpContextAccessor contextAccessor)
        {
            //Check if user has claim, because AspNetCore.Authorize() is skipped
            //https://stackoverflow.com/questions/66160935/hotchocolate-with-authorize-attribute-how-to-get-currently-logged-on-user
            foreach (var i in contextAccessor.HttpContext.User.Identities.Last().Claims)
            {
                if (i.Value == "ViewAnyGraphQLAccess" ||  i.Value == "ViewPermittedGraphQLAccess" ||  i.Value == "ModifyAnyGraphQLAccess" || i.Value == "ModifyPermittedGraphQLAccess")
                {
                    return _unitOfWork.SimulationRepo.GetAllScenario();
                }
            }

            return new List<SimulationDTO>();
        }
    }
}
