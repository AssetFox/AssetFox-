using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
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
        public void ChangeAgencyLogo_Does()
        {
            var logoString1 = "agenlog1"; // length has to be a multiple of 4
            var logoString2 = "agenlog2"; // length has to be a multiple of 4
            var logoBytes1 = Convert.FromBase64String(logoString1);
            var logoBytes2 = Convert.FromBase64String(logoString2);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetAgencyLogo(logoBytes1);
            TestHelper.UnitOfWork.AdminSettingsRepo.SetAgencyLogo(logoBytes2);

            var fetchedLogo = TestHelper.UnitOfWork.AdminSettingsRepo.GetAgencyLogo();
            Assert.EndsWith(logoString2, fetchedLogo);
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
        public void ChangeImplementationLogo_Does()
        {
            var logoString1 = "implementatilog1"; // length has to be a multiple of 4
            var logoString2 = "implementatilog2"; // length has to be a multiple of 4
            var logoBytes1 = Convert.FromBase64String(logoString1);
            var logoBytes2 = Convert.FromBase64String(logoString2);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetImplementationLogo(logoBytes1);
            TestHelper.UnitOfWork.AdminSettingsRepo.SetImplementationLogo(logoBytes2);

            var fetchedLogo = TestHelper.UnitOfWork.AdminSettingsRepo.GetImplementationLogo();
            Assert.EndsWith(logoString2, fetchedLogo);
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
        public void ChangeNetworks_Does()
        {
            var networkEntity1 = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var keyAttributeName = RandomStrings.WithPrefix("keyAttribute");
            var keyAttributeId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var maintainableAssets = MaintainableAssetLists.SingleInNetwork(networkId, CommonTestParameterValues.DefaultEquation);
            var resultAttributeName = RandomStrings.WithPrefix("result");
            var resultAttributeId = Guid.NewGuid();
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork,
                resultAttributeId, resultAttributeName, ConnectionType.EXCEL, keyAttributeName);
            var networkEntity2 = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetPrimaryNetwork(networkEntity1.Name);
            var primaryNetworkBefore = TestHelper.UnitOfWork.NetworkRepo.GetMainNetwork();
            Assert.Equal(networkEntity1.Name, primaryNetworkBefore.Name);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetPrimaryNetwork(networkEntity2.Name);
            var primaryNetworkAfter = TestHelper.UnitOfWork.NetworkRepo.GetMainNetwork();
            Assert.Equal(networkEntity2.Name, primaryNetworkAfter.Name);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataNetwork(networkEntity1.Name);
            var rawDataNetworkBefore = TestHelper.UnitOfWork.NetworkRepo.GetRawNetwork();
            Assert.Equal(networkEntity1.Name, rawDataNetworkBefore.Name);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataNetwork(networkEntity2.Name);
            var rawDataNetworkAfter = TestHelper.UnitOfWork.NetworkRepo.GetRawNetwork();
            Assert.Equal(networkEntity2.Name, rawDataNetworkAfter.Name);
        }

        [Fact]
        public void SetRawDataNetwork_Does()
        {
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataNetwork(networkEntity.Name);

            var rawDataNetwork = TestHelper.UnitOfWork.AdminSettingsRepo.GetRawDataNetwork();
            Assert.Equal(networkEntity.Name, rawDataNetwork);
        }

        [Fact]
        public void DeleteAdminSetting_Does()
        {
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.AdminSettingsRepo.SetPrimaryNetwork(networkEntity.Name);
            var primaryNetwork = TestHelper.UnitOfWork.AdminSettingsRepo.GetPrimaryNetwork();
            Assert.Equal(networkEntity.Name, primaryNetwork);

            TestHelper.UnitOfWork.AdminSettingsRepo.DeleteAdminSetting(AdminSettingsRepository.primaryNetworkKey);
            var primaryNetworkAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetPrimaryNetwork();
            Assert.Null(primaryNetworkAfter);
        }

        [Fact]
        public void SetOneKeyField_ThenGet_Same()
        {
            var attributeName = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName);
            var keyFields = attributeName;
            TestHelper.UnitOfWork.AdminSettingsRepo.SetKeyFields(keyFields);

            var keyFieldsAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetKeyFields();
            var keyFieldAfter = keyFieldsAfter.Single();

            Assert.Equal(keyFields, keyFieldAfter);
        }

        [Fact]
        public void SetTwoKeyFields_ThenGet_Same()
        {
            var attributeName1 = RandomStrings.WithPrefix("attribute");
            var attributeName2 = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName1);
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName2);
            var keyFields = $"{attributeName1},{attributeName2}";
            TestHelper.UnitOfWork.AdminSettingsRepo.SetKeyFields(keyFields);

            var keyFieldsAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetKeyFields();
            var expectedKeyFieldsAfter = new List<string> { attributeName1, attributeName2 };
            Assert.Equal(expectedKeyFieldsAfter, keyFieldsAfter);
        }

        [Fact]
        public void ChangeKeyFields_Does()
        {
            var attributeName1 = RandomStrings.WithPrefix("attribute");
            var attributeName2 = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName1);
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName2);
            var keyFields1 = $"{attributeName1}";
            var keyFields2 = $"{attributeName2}";
            TestHelper.UnitOfWork.AdminSettingsRepo.SetKeyFields(keyFields1);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetKeyFields(keyFields2);

            var keyFieldsAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetKeyFields();
            var expectedKeyFieldsAfter = new List<string> { attributeName2 };
            Assert.Equal(expectedKeyFieldsAfter, keyFieldsAfter);
        }

        [Fact]
        public void SetOneRawKeyField_ThenGet_Same()
        {
            var attributeName = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName);
            var keyFields = attributeName;
            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataKeyFields(keyFields);

            var keyFieldsAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetRawKeyFields();
            var keyFieldAfter = keyFieldsAfter.Single();

            Assert.Equal(keyFields, keyFieldAfter);
        }

        [Fact]
        public void SetTwoRawKeyFields_ThenGet_Same()
        {
            var attributeName1 = RandomStrings.WithPrefix("attribute");
            var attributeName2 = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName1);
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName2);
            var keyFields = $"{attributeName1},{attributeName2}";
            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataKeyFields(keyFields);

            var keyFieldsAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetRawKeyFields();
            var expectedKeyFieldsAfter = new List<string> { attributeName1, attributeName2 };
            Assert.Equal(expectedKeyFieldsAfter,keyFieldsAfter);
        }

        [Fact]
        public void ChangeRawKeyFields_Does()
        {
            var attributeName1 = RandomStrings.WithPrefix("attribute");
            var attributeName2 = RandomStrings.WithPrefix("attribute");
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName1);
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, name: attributeName2);
            var keyFields1 = $"{attributeName1}";
            var keyFields2 = $"{attributeName2}";
            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataKeyFields(keyFields1);

            TestHelper.UnitOfWork.AdminSettingsRepo.SetRawDataKeyFields(keyFields2);

            var keyFieldsAfter = TestHelper.UnitOfWork.AdminSettingsRepo.GetRawKeyFields();
            var expectedKeyFieldsAfter = new List<string> { attributeName2 };
            Assert.Equal(expectedKeyFieldsAfter, keyFieldsAfter);
        }
    }
}
