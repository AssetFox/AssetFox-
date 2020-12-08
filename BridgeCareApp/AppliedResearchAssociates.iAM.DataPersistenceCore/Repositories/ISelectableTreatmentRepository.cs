using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateTreatmentLibrary(string name, string simulationName);
        void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, string simulationName);
        void GetSimulationTreatments(Simulation simulation);
    }
}
