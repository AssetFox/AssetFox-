using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentSchedulingRepository
    {
        void CreateTreatmentSchedulings(Dictionary<Guid, List<TreatmentScheduling>> treatmentSchedulingsPerTreatmentId);
    }
}
