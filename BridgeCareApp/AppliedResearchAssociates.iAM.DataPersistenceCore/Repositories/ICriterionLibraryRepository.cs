using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICriterionLibraryRepository
    {
        void JoinEntitiesWithCriteria(Dictionary<string, List<Guid>> entityIdsPerExpression, string joinEntity,
            string simulationName);

        void CreateCriterionLibraries(List<CriterionLibraryEntity> criterionLibraryEntities);

        void JoinSelectableTreatmentEntitiesWithCriteria(Dictionary<Guid, List<string>> expressionsPerSelectableTreatmentEntityId, string prependName);

        Task<List<CriterionLibraryDTO>> CriterionLibraries();

        void UpsertCriterionLibrary(CriterionLibraryDTO dto, UserInfoDTO userInfo);

        void DeleteCriterionLibrary(Guid libraryId);
    }
}
