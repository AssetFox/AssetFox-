using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateScenarioSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId);

        void GetScenarioSelectableTreatments(Simulation simulation);

        List<TreatmentLibraryDTO> GetTreatmentLibraries();

        void UpsertTreatmentLibrary(TreatmentLibraryDTO dto);

        void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId);

        void DeleteTreatmentLibrary(Guid libraryId);

        List<TreatmentDTO> GetScenarioSelectableTreatments(Guid simulationId);

        void UpsertOrDeleteScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatments, Guid simulationId);
        TreatmentLibraryDTO GetTreatmentLibary(Guid libraryId);
        void HandleImportCompletion(List<TreatmentDTO> treatments, Guid libraryId);
    }
}
