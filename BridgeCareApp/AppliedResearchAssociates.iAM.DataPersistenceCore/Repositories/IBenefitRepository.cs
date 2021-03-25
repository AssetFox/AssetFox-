using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBenefitRepository
    {
        void CreateBenefit(Benefit benefit, Guid analysisMethodId);

        void UpsertBenefit(BenefitDTO dto, Guid analysisMethodId);
    }
}
