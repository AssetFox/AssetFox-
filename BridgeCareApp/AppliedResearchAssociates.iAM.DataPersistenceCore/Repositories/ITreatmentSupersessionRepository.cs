using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentSupersessionRepository
    {
        void CreateTreatmentSupersessions(
            Dictionary<Guid, List<TreatmentSupersession>> treatmentSupersessionsPerTreatmentId, string simulationName);
    }
}
