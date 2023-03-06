using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBenefitQuantifierRepository
    {
        BenefitQuantifierDTO GetBenefitQuantifier(Guid networkId);

        void UpsertBenefitQuantifier(BenefitQuantifierDTO dto);

        void DeleteBenefitQuantifier(Guid networkId);
        void UpsertBenefitQuantifierAtomically(BenefitQuantifierDTO dto);
    }
}
