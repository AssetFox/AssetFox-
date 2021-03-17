using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBenefitQuantifierRepository
    {
        BenefitQuantifierDTO GetBenefitQuantifier(Guid networkId);

        void UpsertBenefitQuantifier(BenefitQuantifierDTO dto, UserInfoDTO userInfo);

        void DeleteBenefitQuantifier(BenefitQuantifierDTO dto);
    }
}
