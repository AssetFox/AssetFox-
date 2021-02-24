using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class CashFlowTests
    {
        private readonly CashFlowTestHelper _testHelper;
        private readonly CashFlowController _controller;

        public CashFlowTests()
        {
            _testHelper = new CashFlowTestHelper();
            _controller = new CashFlowController(_testHelper.UnitOfDataPersistenceWork);
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {

        }
    }
}
