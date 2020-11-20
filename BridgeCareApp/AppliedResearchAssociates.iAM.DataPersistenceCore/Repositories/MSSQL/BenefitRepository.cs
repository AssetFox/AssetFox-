using System;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BenefitRepository : MSSQLRepository, IBenefitRepository
    {
        public BenefitRepository(IAMContext context) : base(context) { }

        public void CreateBenefit(Benefit benefit, Guid analysisMethodId) => throw new NotImplementedException();
    }
}
