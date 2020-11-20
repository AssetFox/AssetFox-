using System;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBenefitRepository
    {
        void CreateBenefit(Benefit benefit, Guid analysisMethodId);
    }
}
