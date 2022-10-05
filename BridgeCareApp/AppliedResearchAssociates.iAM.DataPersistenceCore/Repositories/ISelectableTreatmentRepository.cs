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

        List<TreatmentLibraryDTO> GetAllTreatmentLibraries();

        void UpsertTreatmentLibrary(TreatmentLibraryDTO dto);

        void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId);

        void DeleteTreatmentLibrary(Guid libraryId);

        List<TreatmentDTO> GetScenarioSelectableTreatments(Guid simulationId);

        void UpsertOrDeleteScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatments, Guid simulationId);
        TreatmentLibraryDTO GetSingleTreatmentLibary(Guid libraryId);
        void ReplaceTreatmentLibrary(Guid libraryId, List<TreatmentDTO> treatments);

        public void DeleteTreatment(TreatmentDTO treatment, Guid libraryId);

        public void DeleteScenarioSelectableTreatment(TreatmentDTO scenarioSelectableTreatment, Guid simulationId);

        public void GetScenarioSelectableTreatmentsNoChildren(Simulation simulation);

        TreatmentLibraryDTO GetSingleTreatmentLibaryNoChildren(Guid libraryId);

        List<TreatmentLibraryDTO> GetAllTreatmentLibrariesNoChildren();

        List<TreatmentDTO> GetScenarioSelectableTreatmentsNoChildren(Guid simulationId);
    }
}
