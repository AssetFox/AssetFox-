using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IFacilityRepository
    {
        void CreateFacilities(List<Facility> facilities, Guid networkId);
    }
}
