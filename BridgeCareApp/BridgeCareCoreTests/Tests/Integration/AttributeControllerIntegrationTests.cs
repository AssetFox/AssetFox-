using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using Microsoft.Data.SqlClient;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class AttributeControllerIntegrationTests
    {
        private AttributeController CreateController()
        {
            var attributeService = new AttributeService(TestHelper.UnitOfWork);
            var attributeImportService = new AttributeImportService(TestHelper.UnitOfWork);
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var controller = new AttributeController(
                attributeService,
                attributeImportService,
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor);
            return controller;
        }

        [Fact]
        public async Task CreateAttributes_InsertPortionThrows_UpdatePortionDoesNotHappen()
        {
            var attributeId1 = Guid.NewGuid();
            var attributeId2 = Guid.NewGuid();
            var attributeName1 = RandomStrings.WithPrefix("attribute");
            var attributeDto1 = AttributeTestSetup.CreateSingleNumericAttribute(
                TestHelper.UnitOfWork, attributeId1, attributeName1);
            var attributeDto2 = AttributeDtos.Numeric("dummyName", attributeId2);
            attributeDto2.DataSource = attributeDto1.DataSource;
            var allAttribute1 = AllAttributeDtos.ForAttribute(attributeDto1);
            var allAttribute2 = AllAttributeDtos.ForAttribute(attributeDto2);
            allAttribute1.Minimum = 2;
            allAttribute2.Maximum = double.NaN;
            allAttribute2.Minimum = double.Epsilon;
            var allAttributes = new List<AllAttributeDTO> { allAttribute1, allAttribute2 };
            var controller = CreateController();

            var exception = await Assert.ThrowsAsync<SqlException>(async () => await controller.CreateAttributes(allAttributes));

            var attributeNames = new List<string> { attributeName1 };
            var attributeAfter = TestHelper.UnitOfWork.AttributeRepo.GetSingleById(attributeId1);
            Assert.Equal(0, attributeAfter.Minimum);
        }
    }
}
