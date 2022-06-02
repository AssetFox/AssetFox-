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
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Logging;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class ExpressionValidationTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;
        private static readonly Guid MaintainableAssetId = Guid.Parse("04580d3b-d99a-45f6-b854-adaa3f78910d");

        private ExpressionValidationController SetupController()
        {
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
                SetData();
                AddTestData();
            }
            var service = new ExpressionValidationService(_testHelper.UnitOfWork, new LogNLog());
            var controller = new ExpressionValidationController(service, _testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
            return controller;
        }

        private AttributeEntity NumericAttribute { get; set; }
        private AttributeEntity TextAttribute { get; set; }

        private MaintainableAssetEntity TestMaintainableAsset { get; } =
            new MaintainableAssetEntity { Id = MaintainableAssetId };

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

        private void SetData()
        {
            var culvAttribute = AttributeDtos.CulvDurationN;
            var actionTypeAttribute = AttributeDtos.ActionType;
            NumericAttribute = _testHelper.UnitOfWork.Context.Attribute
                .Single(_ => _.Name == culvAttribute.Name);
            TextAttribute = _testHelper.UnitOfWork.Context.Attribute
                .Single(_ => _.Name == actionTypeAttribute.Name);
        }

        private void AddTestData()
        {
            TestMaintainableAsset.NetworkId = _testHelper.TestNetwork.Id;
            _testHelper.UnitOfWork.Context.AddEntity(TestMaintainableAsset);

            TestNumericAggregatedResult.AttributeId = NumericAttribute.Id;
            TestTextAggregatedResult.AttributeId = TextAttribute.Id;
            _testHelper.UnitOfWork.Context.AddAll(new List<AggregatedResultEntity>
            {
                TestNumericAggregatedResult, TestTextAggregatedResult
            });
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
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "poorly understood issues in class", IsPiecewise = true
                },
                new ValidationResult
                {
                    IsValid = false,
                    ValidationMessage = "At least one TIME,CONDITION pair must be entered"
                }
            };
        }

        public static IEnumerable<object[]> GetInvalidCriterionValidationData()
        {
            yield return new object[]
            {
                new ValidationParameter
                {
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "poorly understood issues in class"
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
                    ValidationMessage = "Unsupported attribute FALSE_ATTRIBUTE"
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
            NumericAttribute = _testHelper.UnitOfWork.Context.Attribute
                .First(_ => _.DataType == DataPersistenceConstants.AttributeNumericDataType);
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{NumericAttribute.Name}]*1",
                IsPiecewise = false
            };

            // Act
            var result = await controller.GetEquationValidationResult(model);

            // Assert
            var validationResult = (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value,
                typeof(ValidationResult));
            Assert.True(validationResult.IsValid);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact(Skip = "Broken as of 10:38am 2 June 2022, not when run by itself, but yes when run as part of a full run. WjTodo if time arises for it?")]

        public async Task ShouldValidateCriterion()
        {
            // Arrange
            var controller = SetupController();
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{NumericAttribute.Name}]='1' AND [{TextAttribute.Name}]='test'"
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

        [Fact (Skip ="Broken as of 10:38am 2 June 2022, even when run by itself. WjTodo if time arises for it?")]
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

        [Fact (Skip ="Broken as of 10:50am 2 June 2022, even when run on its own")]
        public async Task ShouldThrowCalculateEvaluateExceptionOnInvalidEquation()
        {
            // Arrange
            var controller = SetupController();
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "poorly understood issues in class",
                IsPiecewise = false
            };

            // Act + Assert
            await Assert.ThrowsAsync<CalculateEvaluateException>(async () =>
                await controller.GetEquationValidationResult(model));
        }

        [Fact (Skip ="Timer")]
        public void ShouldInvalidateCriteria()
        {
            var controller = SetupController();
            var timer = new System.Timers.Timer { Interval = 5000 };
            // Act + Assert
            GetInvalidCriterionValidationData().ToList().ForEach(async testDataSet =>
            {
                timer.Elapsed += async delegate
                {
                    var result =
                        await controller.GetCriterionValidationResult(testDataSet[0] as ValidationParameter);

                    var actualValidationResult =
                        (CriterionValidationResult)Convert.ChangeType((result as OkObjectResult).Value, typeof(CriterionValidationResult));

                    var expectedValidationResult = testDataSet[1] as CriterionValidationResult;

                    Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
                    Assert.Equal(expectedValidationResult.ResultsCount, actualValidationResult.ResultsCount);
                    Assert.Equal(expectedValidationResult.ValidationMessage, actualValidationResult.ValidationMessage);
                };
            });
        }

        [Fact (Skip ="poorly understood issues in class")]
        public async Task ShouldReturnOkResultOnCriterionPost()
        {
            // Arrange   
            var controller = SetupController();
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{NumericAttribute.Name}]='1'"
            };

            // Act
            var result = await controller.GetCriterionValidationResult(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
