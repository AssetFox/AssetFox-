using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationOutputRepoTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        [Fact]
        public void SaveSimulationOutput_Does()
        {
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(_testHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
        }

        [Fact]
        public void SaveSimulationOutput_ThenLoad_Same()
        {
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(_testHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
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
            var context = SimulationOutputCreationContextTestSetup.SimpleContextWithObjectsInDatabase(_testHelper.UnitOfWork, numberOfYears);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput.InitialAssetSummaries, loadedOutput.InitialAssetSummaries);
            ObjectAssertions.Equivalent(simulationOutput, loadedOutput);
        }
     }
}
