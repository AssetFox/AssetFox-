using HotChocolate.Types;
using HotChocolate.Authorization;
using static BridgeCareCore.Security.SecurityConstants;

namespace BridgeCareCore.GraphQL
{
    public class QueryObjectType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            // See https://stackoverflow.com/questions/73413756/custom-authorization-attribute-on-asp-net-core-graphql-hot-chocolate-mutation
            // See https://chillicream.com/docs/hotchocolate/v12/defining-a-schema/object-types
            descriptor.Field(x => x.GetSimulations(default!))
                .Name("GetSimulations")
                .Authorize(Policy.UseGraphQL);

            descriptor.Field(x => x.GetSimulation(default!))
               .Name("GetSimulation")
               .Authorize(Policy.UseGraphQL);
        }
    }
}
