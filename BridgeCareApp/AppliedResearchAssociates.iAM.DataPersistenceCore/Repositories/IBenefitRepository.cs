using System;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBenefitRepository
    {
        void UpsertBenefit(BenefitDTO dto, Guid analysisMethodId);
    }
}
