using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Extensions.DependencyModel;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class CashFlowDistributionRuleRepositoryTests
    {
        [Fact]
        public void UpsertLibraryDistributionRule_CashFlowRuleInDb_Does()
        {
            // Arrange
            var library = CashFlowRuleLibraryDtos.Empty();
            var rule = CashFlowRuleDtos.Rule();
            var rules = new List<CashFlowRuleDTO> { rule };
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(library);
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(rules, library.Id);

            var distributionRule = CashFlowDistributionRuleDtos.Dto(costCeiling: 123456);
            var dictionary = new Dictionary<Guid, List<CashFlowDistributionRuleDTO>>();
            var distributionRules = new List<CashFlowDistributionRuleDTO> { distributionRule };
            dictionary[rule.Id] = distributionRules;

            TestHelper.UnitOfWork.CashFlowDistributionRuleRepo.UpsertOrDeleteCashFlowDistributionRules(
                dictionary, library.Id);

            var rulesAfter =
                TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRulesByLibraryId(library.Id);
            var ruleAfter = rulesAfter.Single();
            var distributionRuleAfter = ruleAfter.CashFlowDistributionRules.Single();
            ObjectAssertions.Equivalent(distributionRule, distributionRuleAfter);
        }

        [Fact]
        public void UpsertScenarioDistributionRule_CashFlowRuleInDb_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var ruleId = Guid.NewGuid();
            var scenarioRule = CashFlowRuleDtos.Rule(ruleId);
            var scenarioRules = new List<CashFlowRuleDTO> { scenarioRule };
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(scenarioRules, simulation.Id);

            var distributionRule = CashFlowDistributionRuleDtos.Dto(costCeiling: 123456);
            var dictionary = new Dictionary<Guid, List<CashFlowDistributionRuleDTO>>();
            var distributionRules = new List<CashFlowDistributionRuleDTO> { distributionRule };
            dictionary[scenarioRule.Id] = distributionRules;

            TestHelper.UnitOfWork.CashFlowDistributionRuleRepo.UpsertOrDeleteScenarioCashFlowDistributionRules(
                dictionary, simulation.Id);
            var rulesAfter =
                TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulation.Id);
            var ruleAfter = rulesAfter.Single();
            var distributionRuleAfter = ruleAfter.CashFlowDistributionRules.Single();
            ObjectAssertions.Equivalent(distributionRule, distributionRuleAfter);
        }
    }
}
