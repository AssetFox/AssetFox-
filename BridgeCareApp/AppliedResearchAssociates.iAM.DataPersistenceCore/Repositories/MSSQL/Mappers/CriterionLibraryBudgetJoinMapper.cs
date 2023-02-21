using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CriterionLibraryBudgetJoinMapper
    {
        public static CriterionLibraryBudgetEntity ToEntity(
            this CriterionLibraryBudgetDTO dto)
        {
            var entity = new CriterionLibraryBudgetEntity
            {
                CriterionLibraryId = dto.CriterionLibraryId,
                BudgetId = dto.BudgetId,
            };
            return entity;
        }
    }
}
