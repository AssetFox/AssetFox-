using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class ExpressionValidationTests
    {
        private static readonly Guid MaintainableAssetId = Guid.Parse("04580d3b-d99a-45f6-b854-adaa3f78910d");
        private static readonly Guid MaintainableAssetLocationId = Guid.Parse("14580d3b-d99a-45f6-b854-adaa3f78910d");

        private ExpressionValidationService CreateValidationService()
        {
            var service = new ExpressionValidationService(TestHelper.UnitOfWork, new LogNLog());
            return service;
        }

        private ExpressionValidationController SetupController()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            AddTestData();
            var service = CreateValidationService();
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new ExpressionValidationController(service, EsecSecurityMocks.Admin, unitOfWork,
                hubService,
                accessor);
            return controller;
        }

        private MaintainableAssetEntity TestMaintainableAsset { get; } =
            new MaintainableAssetEntity
            {
                Id = MaintainableAssetId,
                MaintainableAssetLocation = new MaintainableAssetLocationEntity
                {
                    Id = MaintainableAssetLocationId,
                    LocationIdentifier = "TestLocationIdentifier2",
                    Discriminator = DataPersistenceConstants.SectionLocation,
                    MaintainableAssetId = MaintainableAssetId,
                }
            };

        private AggregatedResultEntity TestNumericAggregatedResult { get; } = new AggregatedResultEntity
        {
            Id = Guid.NewGuid(),
            MaintainableAssetId = MaintainableAssetId,
            Discriminator = DataPersistenceConstants.AggregatedResultNumericDiscriminator,
            NumericValue = 1
        };

        private AggregatedResultEntity TestTextAggregatedResult { get; } = new AggregatedResultEntity
        {
            Id = Guid.NewGuid(),
            MaintainableAssetId = MaintainableAssetId,
            Discriminator = DataPersistenceConstants.AggregatedResultTextDiscriminator,
            TextValue = "test"
        };

        private static bool TestDataHaveBeenAdded = false;
        private static object TestDataLock = new object();

        private void AddTestData()
        {
            if (!TestDataHaveBeenAdded)
            {
                lock (TestDataLock)
                {
                    if (!TestDataHaveBeenAdded)
                    {
                        TestDataHaveBeenAdded = true;
                        var culvAttribute = AttributeDtos.CulvDurationN;
                        var actionTypeAttribute = AttributeDtos.ActionType;
                        TestMaintainableAsset.NetworkId = NetworkTestSetup.NetworkId;
                        TestHelper.UnitOfWork.Context.AddEntity(TestMaintainableAsset);
                        TestNumericAggregatedResult.AttributeId = AttributeDtos.CulvDurationN.Id;
                        TestTextAggregatedResult.AttributeId = AttributeDtos.ActionType.Id;
                        TestHelper.UnitOfWork.Context.AddAll(new List<AggregatedResultEntity>
                        {
                            TestNumericAggregatedResult, TestTextAggregatedResult
                        });
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetInvalidPiecewiseEquationValidationData()
        {
            yield return new object[]
            {
                new EquationValidationParameters
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "(x,y)", IsPiecewise = true
                },
                new ValidationResult
                {
                    IsValid = false,
                    ValidationMessage = "Failure to convert TIME,CONDITION pair to (int,double): x,y"
                }
            };

            yield return new object[]
            {
                new EquationValidationParameters
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "(-1,1)", IsPiecewise = true
                },
                new ValidationResult
                {
                    IsValid = false,
                    ValidationMessage = "Values for TIME must be 0 or greater"
                }
            };

            yield return new object[]
            {
                new EquationValidationParameters
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "(1,1)(1,2)", IsPiecewise = true
                },
                new ValidationResult
                {
                    IsValid = false,
                    ValidationMessage = "Only unique integer values for TIME are allowed"
                }
            };

            yield return new object[]
            {
                new EquationValidationParameters
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "", IsPiecewise = true
                },
                new ValidationResult
                {
                    IsValid = false,
                    ValidationMessage = "At least one TIME,CONDITION pair must be entered"
                }
            };
        }

        public IEnumerable<object[]> GetInvalidCriterionValidationData()
        {
            yield return new object[]
            {
                new ValidationParameter
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = ""
                },
                new CriterionValidationResult
                {
                    IsValid = false,
                    ResultsCount = 0,
                    ValidationMessage = "There is no criterion expression."
                }
            };

            yield return new object[]
            {
                new EquationValidationParameters
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "[FALSE_ATTRIBUTE]"
                },
                new CriterionValidationResult
                {
                    IsValid = false,
                    ResultsCount = 0,
                    ValidationMessage = "Unsupported Attribute FALSE_ATTRIBUTE"
                }
            };
        }

        [Fact]
        public async Task ShouldReturnOkResultOnEquationPost()
        {
            var controller = SetupController();
            // Arrange
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "(10,1)",
                IsPiecewise = true
            };

            // Act
            var result = await controller.GetEquationValidationResult(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldValidateEquation()
        {
            // Arrange
            var controller = SetupController();
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "(10,1)",
                IsPiecewise = true
            };

            // Act
            var result = await controller.GetEquationValidationResult(model);

            // Assert
            var validationResult = (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value,
                typeof(ValidationResult));
            Assert.True(validationResult.IsValid);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public async Task ShouldValidateNonPiecewiseEquation()
        {
            // Arrange
            var controller = SetupController();
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{AttributeDtos.CulvDurationN.Name}]*1",
                IsPiecewise = false
            };

            // Act
            var result = await controller.GetEquationValidationResult(model);

            // Assert
            var validationResult = (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value,
                typeof(ValidationResult));
            if (!validationResult.IsValid)
            {
                // Occasional test failure here. A breakpoint may hopefully catch it in the act someday.
                Assert.Equal("dummy assert to print the message", AttributeDtos.CulvDurationN.Name + " " + validationResult.ValidationMessage);
                Assert.True(validationResult.IsValid);
            }
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public async Task ShouldValidateCriterion()
        {
            // Arrange
            var controller = SetupController();
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{AttributeDtos.CulvDurationN.Name}]='1' AND [{AttributeDtos.ActionType.Name}]='test'",
                NetworkId = NetworkTestSetup.NetworkId,
            };

            // Act
            var result = await controller.GetCriterionValidationResult(model);

            // Assert
            var validationResult = (CriterionValidationResult)Convert.ChangeType((result as OkObjectResult).Value,
                typeof(CriterionValidationResult));

            Assert.True(validationResult.IsValid);
            Assert.Equal(1, validationResult.ResultsCount);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public void ShouldInvalidatePiecewiseEquations()
        {
            // Act + Assert
            var controller = SetupController();
            var invalidPiecewiseEquationValidationData = GetInvalidPiecewiseEquationValidationData().ToList();

            foreach (var testDataSet in invalidPiecewiseEquationValidationData)
            {
                var result = controller.GetEquationValidationResult(testDataSet[0] as EquationValidationParameters);
                var objectResult = (OkObjectResult)result.Result;
                var actualValidationResult = (ValidationResult)objectResult.Value;
                var expectedValidationResult = testDataSet[1] as ValidationResult;

                Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
                Assert.Equal(expectedValidationResult.ValidationMessage, actualValidationResult.ValidationMessage);
            };
        }

        [Fact]
        public async Task ShouldInvalidateNonPiecewiseEquation()
        {
            // Arrange
            var controller = SetupController();
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "[FALSE_ATTRIBUTE]",
                IsPiecewise = false
            };

            // Act
            var result = await controller.GetEquationValidationResult(model);

            // Assert
            var validationResult =
                (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value, typeof(ValidationResult));

            Assert.False(validationResult.IsValid);
            Assert.Equal("Unsupported Attribute FALSE_ATTRIBUTE", validationResult.ValidationMessage);
        }

        [Fact]
        public void ValidateEmptyEquation_Invalid()
        {
            // Arrange
            var service = CreateValidationService();
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "",
                IsPiecewise = false
            };

            var result = service.ValidateEquation(model);
            // Act + Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ShouldInvalidateCriteria()
        {
            var controller = SetupController();
            var invalid = GetInvalidCriterionValidationData().ToList();
            foreach (var testDataSet in invalid)
            {
                var validationParams = testDataSet[0] as ValidationParameter;
                validationParams.NetworkId = NetworkTestSetup.NetworkId;
                var result =
                    await controller.GetCriterionValidationResult(validationParams);

                var actualValidationResult =
                    (CriterionValidationResult)Convert.ChangeType((result as OkObjectResult).Value, typeof(CriterionValidationResult));

                var expectedValidationResult = testDataSet[1] as CriterionValidationResult;

                Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
                Assert.Equal(expectedValidationResult.ResultsCount, actualValidationResult.ResultsCount);
                Assert.Equal(expectedValidationResult.ValidationMessage, actualValidationResult.ValidationMessage);
            }
        }

        [Fact]
        public async Task ShouldReturnOkResultOnCriterionPost()
        {
            // Arrange   
            var controller = SetupController();
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{AttributeDtos.CulvDurationN.Name}]='1'",
                NetworkId = NetworkTestSetup.NetworkId,
            };

            // Act
            var result = await controller.GetCriterionValidationResult(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
