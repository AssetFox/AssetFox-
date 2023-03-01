using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Tests.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class ReportControllerIntegrationTests
    {
        private ReportController CreateController(
            Mock<IReportGenerator> reportGenerator,
            HttpContext httpContext)
        {
            var security = EsecSecurityMocks.Admin;
            var hubService = HubServiceMocks.Default();
            var accessor = HttpContextAccessorMocks.WithContext(httpContext);
            var logger = new CallbackLogger(str => { });
            var controller = new ReportController(
                reportGenerator.Object,
                security,
                TestHelper.UnitOfWork,
                hubService,
                accessor.Object,
                logger
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
            return controller;
        }

        [Fact]
        public async Task Blah()
        {
            var generator = ReportGeneratorMocks.New();
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Path = "/api/ReportController/GetFile";

            var controller = CreateController(generator, context);
            

            var response = await controller.GetFile("kitten");

            await controller.HandleGetFileCompletion("", "", "");
            var reportIndexes = TestHelper.UnitOfWork.Context.ReportIndex.ToList();
            var theIndex = reportIndexes.Single();
            Assert.Equal("Failure Report", theIndex.ReportTypeName);
            Assert.Equal(DateTime.Now.Date, theIndex.CreatedDate.Date);
            Assert.Equal(DateTime.Now.Date, theIndex.LastModifiedDate.Date);
        }
    }
}
