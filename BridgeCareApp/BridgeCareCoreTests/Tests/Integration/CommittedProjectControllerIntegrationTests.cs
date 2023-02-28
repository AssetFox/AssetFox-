using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class CommittedProjectControllerIntegrationTests
    {
        private CommittedProjectController CreateController()
        {
            var service = new CommittedProjectService(TestHelper.UnitOfWork);
            var pagingService = new CommittedProjectPagingService(TestHelper.UnitOfWork);
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var contextAccessor = HttpContextAccessorMocks.Default();
            var claimHelper = ClaimHelperMocks.New();
            return new CommittedProjectController(

                service,
                pagingService,
                security,
                TestHelper.UnitOfWork,
                hubService,
                contextAccessor,
                claimHelper.Object
                );
        }

        [Fact]
        public void FillTreatmentValues_Does()
        {
            var controller = CreateController();
            var fillModel = new CommittedProjectFillTreatmentValuesModel
            {

            };

            var result = controller.FillTreatmentValues(fillModel);
        }
    }
}
