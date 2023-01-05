using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services.Aggregation;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class AggregationTests
    {
        private Mock<ILog> _mockLog;
        private Mock<IAggregationService> _mockService;

        public AggregationTests()
        {
            _mockLog = new Mock<ILog>();
            _mockService = new Mock<IAggregationService>();
        }

        public AggregationController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new AggregationController(_mockLog.Object, _mockService.Object, EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork, hubService, accessor);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }
        [Fact]
        public async Task UserIsAggregateNetworkDataAuthorized()
        {
            // Admin test authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("TestAggregatePolicy",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkAggregateAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "TestAggregatePolicy");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsAggregateNetworkDataAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("TestAggregatePolicy",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.NetworkAggregateAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "TestAggregatePolicy");
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
// Shaun: Tests (below) still need to be used per Jake, but were commented out prior to the above test code implementation.

/*private static List<(int, double)> ExpectedResult = new List<(int, double)> { (2020, 101.5), (2019, 115) };
[Test]
public void AverageAggregationLinearRuleTest()
{
    var averageRule = new AverageAggregationRule();
    var data = new List<AttributeDatum<double>>();
    // Arrange
    data.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
    data.Add(TestDataForAttribute.NumericAttributeDataLinearLocation_2);
    data.Add(TestDataForAttribute.NumericAttributeDataLinearLocation_a);
    // Act
    var distinctYears = data.Select(_ => _.TimeStamp.Year).Distinct();
    //var resultSet = averageRule.Apply(data);
    // Assert
    //foreach (var item in resultSet)
    //{
        // Item1 is year and Item2 is average value
        //var expectedAverage = ExpectedResult.Where(_ => _.Item2 == item.Item2).Select(s => s.Item2).FirstOrDefault();
        //Assert.That(item.Item2, Is.EqualTo(expectedAverage));
        // make sure the actual average is expected attribute
    //}
    //Assert.That(resultSet.Count(), Is.EqualTo(distinctYears.Count()));
}

[Test]
public void AverageAggregationSectionRuleTest()
{
    var averageRule = new AverageAggregationRule();
    var data = new List<AttributeDatum<double>>();
    // Arrange
    data.Add(TestDataForAttribute.NumericAttributeSectionLocOutput);
    data.Add(TestDataForAttribute.NumericAttributeSectionLocOutput_2);
    data.Add(TestDataForAttribute.NumericAttributeSectionLocOutput_a);
    // Act
    var distinctYears = data.Select(_ => _.TimeStamp.Year).Distinct();
    //var resultSet = averageRule.Apply(data);
    // Assert
    //foreach (var item in resultSet)
    //{
        // Item1 is year and Item2 is average value
        //var expectedAverage = ExpectedResult.Where(_ => _.Item1 == item.Item1).Select(s => s.Item2).FirstOrDefault();
        //Assert.That(item.Item2, Is.EqualTo(expectedAverage));
    //}
    //Assert.That(resultSet.Count(), Is.EqualTo(distinctYears.Count()));
}

[Test]
public void PredominantAggregationLinearRuleTest()
{
    var predominantRule = new PredominantAggregationRule();
    // Arrange
    var data = new List<AttributeDatum<string>>
    {
        TestDataForAttribute.TextAttributeDataLinearLocOutput,
        TestDataForAttribute.TextAttributeDataLinearLocOutput_2
    };
    // Act
    //var resultSet = predominantRule.Apply(data);

    // Assert
    //Assert.That(resultSet.Count(), Is.EqualTo(data.Count));
}

[Test]
public void PredominantAggregationSectionRuleTest()
{
    var predominantRule = new PredominantAggregationRule();
    // Arrange
    var data = new List<IAttributeDatum>
    {
        TestDataForAttribute.TextAttributeDataSectionLocOutput,
        TestDataForAttribute.TextAttributeDataSectionLocOutput_2
    };
    // Act
    //var resultSet = predominantRule.Apply(data);

    // Assert
    //Assert.That(resultSet.Count(), Is.EqualTo(data.Count));
}

[Test]
public void AggregateTest()
{
    var data = new List<IAttributeDatum>()
    {
        TestDataForAttribute.TextAttributeDataSectionLocOutput,
        TestDataForAttribute.TextAttributeDataSectionLocOutput_2
    };
    var networkSegments = new List<Segment>()
    {
        new Segment(new SectionLocation("B-0-1"))
    };
    Aggregator.AssignAttributeDataToSegments(data, networkSegments);
}*/
