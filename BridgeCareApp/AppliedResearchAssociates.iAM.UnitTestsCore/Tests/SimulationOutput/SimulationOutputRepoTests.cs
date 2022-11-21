using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.TestHelpers.Assertions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationOutputRepoTests
    {
        [Fact]
        public void SaveSimulationOutput_Does()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
        }

        [Fact]
        public void SaveSimulationOutput_ThenLoad_Same()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            ObjectAssertions.Equivalent(simulationOutput, loadedOutput);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
      //  [InlineData(1000)] // 2 seconds or so on 8/10 when part of a full run
      //  [InlineData(10000)] // typically passes. Was 18.5 sec 8/10 on WJ machine.
      //  [InlineData(100000)] // typically fails on a TimeOutException
        public void SaveMultiYearSimulationOutput_ThenLoad_Same(int numberOfYears)
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork, numberOfYears);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            ObjectAssertions.Equivalent(simulationOutput, loadedOutput);
        }

        [Fact (Skip = "May be slow, depending on the batch size")]
        public void SaveSimulationOutputWithMoreAssetsThanBatchSize_ThenLoad_Same()
        {
            var numberOfAssets = 25 + SimulationOutputRepository.AssetLoadBatchSize;
            var assetNameIdPairs = AssetNameIdPairLists.Random(numberOfAssets);
            var numericAttributeName = RandomStrings.WithPrefix("NumericAttribute");
            var textAttributeName = RandomStrings.WithPrefix("TextAttrbute");
            var numericAttributeNames = new List<string> { numericAttributeName };
            var textAttributeNames = new List<string> { textAttributeName };
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(
                TestHelper.UnitOfWork,
                assetNameIdPairs,
                numericAttributeNames,
                textAttributeNames
                );
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            SimulationOutputAssertions.SameSimulationOutput(simulationOutput, loadedOutput);
        }

        [Fact]
        public async Task SaveSimulationOutput_ThenLoad_LastModifiedDate_Expected()
        {
            var numberOfYears = 1;
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork, numberOfYears);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            await Task.Delay(100);
            var startDate = DateTime.Now;
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var endDate = DateTime.Now;
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            var lastModifiedDate = loadedOutput.LastModifiedDate;
            DateTimeAssertions.Between(startDate, endDate, lastModifiedDate, TimeSpan.FromMilliseconds(1));
        }
    }
}
