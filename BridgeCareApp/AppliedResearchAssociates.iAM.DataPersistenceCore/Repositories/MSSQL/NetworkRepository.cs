using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository<NetworkEntity>
    {
        public NetworkRepository(IAMContext context) : base(context)
        {
        }

        public override NetworkEntity Add(NetworkEntity network)
        {
            context.Add(network);
            context.SaveChanges();

            return network;

        }
    }
}
