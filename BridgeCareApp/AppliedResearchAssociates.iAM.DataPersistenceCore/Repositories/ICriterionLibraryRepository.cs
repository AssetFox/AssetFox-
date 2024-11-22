using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICriterionLibraryRepository
    {
        Task<List<CriterionLibraryDTO>> CriterionLibraries();

        Task<CriterionLibraryDTO> CriteriaLibrary(Guid libraryId);

        Guid UpsertCriterionLibrary(CriterionLibraryDTO dto);

        void DeleteCriterionLibrary(Guid libraryId);
        void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation(Guid simulationId, List<string> budgetNames);
        void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary(Guid budgetLibraryId, List<string> budgetNames);
        void AddLibraries(List<CriterionLibraryDTO> criteria);
        void AddLibraryScenarioBudgetJoins(List<CriterionLibraryScenarioBudgetDTO> criteriaJoins);
        void AddLibraryBudgetJoins(List<CriterionLibraryBudgetDTO> criteriaJoins);
    }
}
