using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.GraphQL;
using BridgeCareCore.Security;
using HotChocolate.Types;
using HotChocolate.Authorization;
using HotChocolate.Types.Pagination;
using LiteDB;
using static BridgeCareCore.Security.SecurityConstants;

namespace BridgeCareCore.GraphQL
{
    /*public class QueryObjectType : HotChocolate.Types.ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(_ => _.GetSimulations()).Name("Welcome").Type<StringType>();
        }
    }

    [Authorize(Roles = new[] { "Administrator", "Editor", "SimulationPowerUser", "ReadOnly", "Default" })]
    [Authorize(Policy = "claim-policy-1")]
    [Authorize(Policy = Policy.ViewGraphQL)]
    public class QueryObjectType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Authorize();

            descriptor.Field(f => f.GetSimulations).Authorize();

            descriptor.Field(_ => _.GetSimulations).Name("GetSimulations").Type<StringType>().Authorize(Policy.ViewGraphQL);
        }
    }*/

    public class QueryObjectType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            //https://stackoverflow.com/questions/73413756/custom-authorization-attribute-on-asp-net-core-graphql-hot-chocolate-mutation
            //https://chillicream.com/docs/hotchocolate/v12/defining-a-schema/object-types
            descriptor.Field(x => x.GetSimulations(default!, default!))
                .Name("GetSimulations")
                .Authorize(Policy.ViewGraphQL);
        }
    }
}
