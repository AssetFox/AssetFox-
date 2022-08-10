using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Networking;
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
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(_testHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
        }

        [Fact]
        public void SaveSimulationOutput_ThenLoad_Same()
        {
            var context = SimulationOutputCreationContextTestSetup.ContextWithObjectsInDatabase(_testHelper.UnitOfWork);
            var simulationOutput = SimulationOutputModels.SimulationOutput(context);
            _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(context.SimulationId, simulationOutput);
            var loadedOutput = _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(context.SimulationId);
            ObjectAssertions.Equivalent(simulationOutput, loadedOutput);
        }
    }
}
