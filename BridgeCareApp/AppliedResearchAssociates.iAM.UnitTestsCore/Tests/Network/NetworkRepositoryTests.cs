using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class NetworkRepositoryTests
    {
        [Fact]
        public void GetNetworkNameOrId_NetworkNotInDb_GetsId()
        {
            var networkId = Guid.NewGuid();
            var networkNameOrId = TestHelper.UnitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
            Assert.Contains(networkNameOrId.ToString(), networkNameOrId);
        }
    }
}
