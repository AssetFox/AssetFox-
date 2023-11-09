using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Benefit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BenefitRepositoryTests
    {
        [Fact]
        public void UpsertBenefitSameAttribute_AnalysisMethodInDbWithBenefit_Updates()
        {
            var analysisMethodId = Guid.NewGuid();
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var analysisMethod = AnalysisMethodDtos.Default(analysisMethodId, TestAttributeNames.Age);
            TestHelper.UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulation.Id, analysisMethod);
            var benefitDto = analysisMethod.Benefit;
            benefitDto.Limit = 66;

            TestHelper.UnitOfWork.BenefitRepo.UpsertBenefit(benefitDto, analysisMethodId);

            var analysisMethodAfter = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);
            var benefitAfter = analysisMethodAfter.Benefit;
            ObjectAssertions.Equivalent(benefitDto, benefitAfter);
        }

        [Fact]
        public void UpsertBenefitDifferentAttribute_AnalysisMethodInDbWithBenefit_Updates()
        {
            var analysisMethodId = Guid.NewGuid();
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var analysisMethod = AnalysisMethodDtos.Default(analysisMethodId, TestAttributeNames.Age);
            TestHelper.UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulation.Id, analysisMethod);
            var benefitId2 = Guid.NewGuid();
            var benefitDto2 = BenefitDtos.Dto(TestAttributeNames.DeckDurationN, benefitId2);

            TestHelper.UnitOfWork.BenefitRepo.UpsertBenefit(benefitDto2, analysisMethodId);

            var analysisMethodAfter = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);
            var benefitAfter = analysisMethodAfter.Benefit;
            ObjectAssertions.Equivalent(benefitDto2, benefitAfter);
        }

        [Fact]
        public void UpsertBenefit_NoAttribute_AnalysisMethodInDbWithBenefit_Throws()
        {
            var analysisMethodId = Guid.NewGuid();
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var analysisMethod = AnalysisMethodDtos.Default(analysisMethodId, TestAttributeNames.Age);
            var benefitDto = analysisMethod.Benefit;
            TestHelper.UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulation.Id, analysisMethod);
            var benefitId2 = Guid.NewGuid();
            var benefitDto2 = BenefitDtos.Dto("", benefitId2);

            var exception = Assert.Throws<InvalidOperationException>(
                () => TestHelper.UnitOfWork.BenefitRepo.UpsertBenefit(benefitDto2, analysisMethodId));

            var analysisMethodAfter = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);
            var benefitAfter = analysisMethodAfter.Benefit;
            ObjectAssertions.Equivalent(benefitDto, benefitAfter);
        }

        [Fact]
        public void UpsertBenefit_NonexistentAttribute_AnalysisMethodInDbWithBenefit_Throws()
        {
            var analysisMethodId = Guid.NewGuid();
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var analysisMethod = AnalysisMethodDtos.Default(analysisMethodId, TestAttributeNames.Age);
            var benefitDto = analysisMethod.Benefit;
            TestHelper.UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulation.Id, analysisMethod);
            var benefitId2 = Guid.NewGuid();
            var benefitDto2 = BenefitDtos.Dto("", benefitId2);

            var exception = Assert.Throws<InvalidOperationException>(
                () => TestHelper.UnitOfWork.BenefitRepo.UpsertBenefit(benefitDto2, analysisMethodId));

            var analysisMethodAfter = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);
            var benefitAfter = analysisMethodAfter.Benefit;
            ObjectAssertions.Equivalent(benefitDto, benefitAfter);
        }

        [Fact]
        public void UpsertBenefit_AnalysisMethodDoesNotExist_Throws()
        {
            var nonexistentAnalysisMethodId = Guid.NewGuid();
            var benefit = BenefitDtos.Dto(TestAttributeNames.Age);

            var exception = Assert.Throws<RowNotInTableException>(() =>
            TestHelper.UnitOfWork.BenefitRepo.UpsertBenefit(benefit, nonexistentAnalysisMethodId));

        }
    }
}
