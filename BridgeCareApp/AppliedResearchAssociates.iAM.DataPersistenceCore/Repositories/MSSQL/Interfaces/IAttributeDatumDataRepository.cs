using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IAttributeDatumDataRepository
    {
        void AddAttributeDatum<T>(AttributeDatum<T> domain, string locationUniqueIdentifier);
    }
}
