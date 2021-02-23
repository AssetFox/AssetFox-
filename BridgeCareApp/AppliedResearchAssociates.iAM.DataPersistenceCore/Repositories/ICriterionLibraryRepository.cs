using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICriterionLibraryRepository
    {
        void JoinEntitiesWithCriteria(Dictionary<string, List<Guid>> entityIdsPerExpression, string joinEntity,
            string simulationName);

        void CreateCriterionLibraries(List<CriterionLibraryEntity> criterionLibraryEntities);

        void JoinSelectableTreatmentEntitiesWithCriteria(Dictionary<Guid, List<string>> expressionsPerSelectableTreatmentEntityId, string prependName);

        Task<List<CriterionLibraryDTO>> CriterionLibraries();
        void AddOrUpdateCriterionLibrary(CriterionLibraryDTO dto);
        void DeleteCriterionLibrary(Guid libraryId);
    }
}
