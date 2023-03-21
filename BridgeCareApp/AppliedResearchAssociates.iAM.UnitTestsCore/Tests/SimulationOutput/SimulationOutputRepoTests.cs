﻿using System;
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
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutputViaRelational(context.SimulationId, simulationOutput);
        }

        [Fact]
        public void SaveSimulationOutput_ThenLoad_Same()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutputViaRelational(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutputViaRelation(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            ObjectAssertions.EquivalentExcluding(simulationOutput, loadedOutput, so => so.LastModifiedDate);
        }

        [Theory]
        
        [InlineData(2)]
        [InlineData(12)]
        public void SaveMultiYearSimulationOutput_ThenLoad_Same(int numberOfYears)
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork, numberOfYears);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutputViaRelational(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutputViaRelation(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            ObjectAssertions.EquivalentExcluding(simulationOutput, loadedOutput, so => so.LastModifiedDate);
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
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutputViaRelational(context.SimulationId, simulationOutput);
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutputViaRelation(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            SimulationOutputAssertions.SameSimulationOutput(simulationOutput, loadedOutput);
        }

        [Fact]
        public void SaveSimulationOutput_ThenLoad_LastModifiedDate_Expected()
        {
            var numberOfYears = 1;
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork, numberOfYears);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            var startDate = DateTime.Now;
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutputViaRelational(context.SimulationId, simulationOutput);
            var endDate = DateTime.Now;
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutputViaRelation(context.SimulationId);
            var lastModifiedDate = loadedOutput.LastModifiedDate;
            DateTimeAssertions.Between(startDate, endDate, lastModifiedDate, TimeSpan.FromMilliseconds(1));
        }


        [Fact]
        public void SaveSimulationOutput_UpdateSimulation_ThenLoad_LastModifiedDate_MatchesSimulationUpdateDate()
        {
            var numberOfYears = 1;
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(TestHelper.UnitOfWork, numberOfYears);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            var startDate = DateTime.Now;
            TestHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutputViaRelational(context.SimulationId, simulationOutput);
            var endDate = DateTime.Now;
            var oldSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(context.SimulationId);
            oldSimulation.Name = RandomStrings.WithPrefix("Modified simulation");
            var updateStartDate = DateTime.Now;
            TestHelper.UnitOfWork.SimulationRepo.UpdateSimulationAndPossiblyUsers(oldSimulation);
            var updateEndDate = DateTime.Now;
            var loadedOutput = TestHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutputViaRelation(context.SimulationId);
            var lastModifiedDate = loadedOutput.LastModifiedDate;
            DateTimeAssertions.Between(updateStartDate, updateEndDate, lastModifiedDate, TimeSpan.FromMilliseconds(1));
        }
    }
}
