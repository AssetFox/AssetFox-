using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateTreatmentLibrary(string name, Guid simulationId);
        void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId);
        void GetSimulationTreatments(Simulation simulation);
    }
}
