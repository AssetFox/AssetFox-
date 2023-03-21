using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BenefitQuantifier
{
    public static class BenefitQuantifierRepositoryTests
    {
        [Fact]
        public static void DeleteBenefitQuantifier_BenefitQuantifierInDbWithEquation_Deletes()
        {
            var networkId = Guid.NewGuid();
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var network = NetworkTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, new List<MaintainableAsset>(), networkId);
            var dto = BenefitQuantifierDtos.Dto(networkId);
            TestHelper.UnitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifierNonAtomic(dto);

            var benefitQuantifierInDb = TestHelper.UnitOfWork.Context.BenefitQuantifier
                .SingleOrDefault(bc => bc.NetworkId == networkId);
            Assert.NotNull(benefitQuantifierInDb);

            TestHelper.UnitOfWork.BenefitQuantifierRepo.DeleteBenefitQuantifier(networkId);

            var benefitQuantifierInDbAfter = TestHelper.UnitOfWork.Context.BenefitQuantifier
                .SingleOrDefault(bc => bc.NetworkId == networkId);
            Assert.Null(benefitQuantifierInDbAfter);
        }


        [Fact]
        public static void DeleteBenefitQuantifier_BenefitQuantifierInDbWithoutEquation_Deletes()
        {
            var networkId = Guid.NewGuid();
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var network = NetworkTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, new List<MaintainableAsset>(), networkId);
            var dto = BenefitQuantifierDtos.Dto(networkId);
            TestHelper.UnitOfWork.BenefitQuantifierRepo.UpsertBenefitQuantifierNonAtomic(dto);

            var benefitQuantifierInDb = TestHelper.UnitOfWork.Context.BenefitQuantifier
                .SingleOrDefault(bc => bc.NetworkId == networkId);
            Assert.NotNull(benefitQuantifierInDb);
            var equationInDb = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.BenefitQuantifier.NetworkId == networkId);
            Assert.NotNull(equationInDb);
            TestHelper.UnitOfWork.Context.Equation.Remove(equationInDb);
            TestHelper.UnitOfWork.Context.SaveChanges();
            var equationInDbMid = TestHelper.UnitOfWork.Context.Equation
                .SingleOrDefault(e => e.BenefitQuantifier.NetworkId == networkId);
            Assert.Null(equationInDbMid);

            TestHelper.UnitOfWork.BenefitQuantifierRepo.DeleteBenefitQuantifier(networkId);

            var benefitQuantifierInDbAfter = TestHelper.UnitOfWork.Context.BenefitQuantifier
                .SingleOrDefault(bc => bc.NetworkId == networkId);
            Assert.Null(benefitQuantifierInDbAfter);
            var equationInDbAfter = TestHelper.UnitOfWork.Context.Equation
                 .SingleOrDefault(e => e.BenefitQuantifier.NetworkId == networkId);
            Assert.Null(equationInDbAfter);
        }
    }
}
