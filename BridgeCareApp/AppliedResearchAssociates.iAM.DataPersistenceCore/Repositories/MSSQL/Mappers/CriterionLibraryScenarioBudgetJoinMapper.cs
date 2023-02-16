using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CriterionLibraryScenarioBudgetJoinMapper
    {
        public static CriterionLibraryScenarioBudgetEntity ToEntity(
            this CriterionLibraryScenarioBudgetDTO dto)
        {
            var entity = new CriterionLibraryScenarioBudgetEntity
            {
                CriterionLibraryId = dto.CriterionLibraryId,
                ScenarioBudgetId = dto.ScenarioBudgetId,
            };
            return entity;
        }
    }
}
