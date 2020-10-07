using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    internal class LocationRepository : MSSQLRepository<Location>
    {
        public LocationRepository(IAMContext context) : base(context) { }
    }
}
