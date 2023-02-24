using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class CommittedProjectServiceIntegrationTests
    {
        private CommittedProjectService CreateCommittedProjectService()
        {
            var service = new CommittedProjectService(TestHelper.UnitOfWork);
            return service;
        }

        [Fact]
        public void GetTreatmentCost_Behaves()
        {
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var service = CreateCommittedProjectService();
            var treatmentLibraryId = Guid.NewGuid();
            var assetKeyData = "";
            var treatmentName = "treatment";

            var cost = service.GetTreatmentCost(
                treatmentLibraryId,
                assetKeyData,
                treatmentName,
                NetworkTestSetup.NetworkId);
        }
    }
}
