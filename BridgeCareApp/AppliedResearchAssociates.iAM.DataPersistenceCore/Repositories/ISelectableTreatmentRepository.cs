using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateTreatmentLibrary(string name, Guid simulationId);

        void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId);

        void GetSimulationTreatments(Simulation simulation);

        List<TreatmentLibraryDTO> GetTreatmentLibrariesWithTreatments();

        void UpsertTreatmentLibrary(TreatmentLibraryDTO dto);

        void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId);

        void DeleteTreatmentLibrary(Guid libraryId);

        List<TreatmentDTO> GetScenarioSelectableTreatments(Guid simulationId);

        void UpsertOrDeleteScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatment, Guid simulationId);
    }
}
