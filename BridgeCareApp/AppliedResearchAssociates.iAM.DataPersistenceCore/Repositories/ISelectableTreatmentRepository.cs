using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void GetScenarioSelectableTreatments(Simulation simulation);

        DateTime GetLibraryModifiedDate(Guid treatmentLibraryId);

        List<TreatmentLibraryDTO> GetAllTreatmentLibraries();

        void UpsertTreatmentLibrary(TreatmentLibraryDTO dto);

        void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId);

        void DeleteTreatmentLibrary(Guid libraryId);

        void AddLibraryTreatments(List<TreatmentDTO> treatments, Guid libraryId);
        void AddScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatments,
           Guid simulationId);

        List<TreatmentDTO> GetScenarioSelectableTreatments(Guid simulationId);
        List<TreatmentDTO> GetSelectableTreatments(Guid libraryId);

        void UpsertOrDeleteScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatments, Guid simulationId);
        TreatmentLibraryDTO GetSingleTreatmentLibary(Guid libraryId);
        void ReplaceTreatmentLibrary(Guid libraryId, List<TreatmentDTO> treatments);

        public void DeleteTreatment(TreatmentDTO treatment, Guid libraryId);

        public void DeleteScenarioSelectableTreatment(TreatmentDTO scenarioSelectableTreatment, Guid simulationId);

        public void GetScenarioSelectableTreatmentsNoChildren(Simulation simulation);   // unused as of 12/21/23, but was called from 5 different places as of 4/2023. Might be a candidate for deletion if uses do not re-appear.

        TreatmentLibraryDTO GetSingleTreatmentLibaryNoChildren(Guid libraryId);

        List<TreatmentLibraryDTO> GetAllTreatmentLibrariesNoChildren();

        List<TreatmentLibraryDTO> GetTreatmentLibrariesNoChildrenAccessibleToUser(Guid userId);

        List<SimpleTreatmentDTO> GetSimpleTreatmentsBySimulationId(Guid simulationId);
        List<SimpleTreatmentDTO> GetSimpleTreatmentsByLibraryId(Guid simulationId);
        TreatmentDTOWithSimulationId GetScenarioSelectableTreatmentById(Guid id);
        
        ScenarioSelectableTreatmentEntity GetDefaultTreatment(Guid simulationId);
        TreatmentDTO GetDefaultNoTreatment(Guid simulationId);
        TreatmentLibraryDTO GetTreatmentLibraryWithSingleTreatmentByTreatmentId(Guid treatmentId);
        TreatmentDTO GetSelectableTreatmentByLibraryIdAndName(Guid treatmentLibraryId, string treatmentName);
        void UpsertOrDeleteTreatmentLibraryTreatmentsAndPossiblyUsers(TreatmentLibraryDTO dto, bool isNewLibrary, Guid userId);
        void AddDefaultPerformanceFactors(Guid scenarioId, List<TreatmentDTO> treatments);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        public List<TreatmentDTO> GetScenarioSelectableTreatmentsWithCriterionLibrary(Guid simulationId);

        public void GetScenarioSelectableTreatmentsForReport(Simulation simulation);
    }
}
