using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.AdminSettings
{
    public class AdminSettingsRepositoryTests
    {
        [Fact]
        public void CreateAgencyLogo_Does()
        {
            var logoString = "agenlogo"; // length has to be a multiple of 4
            var logoBytes = Convert.FromBase64String(logoString);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetAgencyLogo(logoBytes);

            var fetchedLogo = TestHelper.UnitOfWork.AdminSettingsRepo.GetAgencyLogo();
            Assert.EndsWith(logoString, fetchedLogo);
        }

        [Fact]
        public void CreateImplementationLogo_Does()
        {
            var logoString = "implementatilogo";
            var logoBytes = Convert.FromBase64String(logoString);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetImplementationLogo(logoBytes);

            var fetchedLogo = TestHelper.UnitOfWork.AdminSettingsRepo.GetImplementationLogo ();
            Assert.EndsWith(logoString, fetchedLogo);
        }

        [Fact]
        public void SetPrimaryNetwork_Does()
        {
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetPrimaryNetwork(networkEntity.Name);

            var primaryNetwork = TestHelper.UnitOfWork.AdminSettingsRepo.GetPrimaryNetwork();
            Assert.Equal(networkEntity.Name, primaryNetwork);
        }

        [Fact]
        public void SetRawDataNetwork_Does()
        {
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataNetwork(networkEntity.Name);

            var rawDataNetwork = TestHelper.UnitOfWork.AdminSettingsRepo.GetRawDataNetwork();
            Assert.Equal(networkEntity.Name, rawDataNetwork);
        }
    }
}
