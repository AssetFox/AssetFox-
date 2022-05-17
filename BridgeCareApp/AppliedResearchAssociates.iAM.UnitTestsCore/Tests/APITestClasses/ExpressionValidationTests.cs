using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Logging;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    // wjfix all tests in this class
    public class ExpressionValidationTests
    {
        private readonly TestHelper _testHelper;
        private readonly ExpressionValidationService _service;
        private ExpressionValidationController _controller;

        private static readonly Guid MaintainableAssetId = Guid.Parse("04580d3b-d99a-45f6-b854-adaa3f78910d");

        public ExpressionValidationTests()
        {
            Thread.Sleep(2000);
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
                SetData();
                AddTestData();
            }
            _service = new ExpressionValidationService(_testHelper.UnitOfWork, new LogNLog());
            _controller = new ExpressionValidationController(_service, _testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

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
            NumericAttribute = _testHelper.UnitOfWork.Context.Attribute
                .First(_ => _.DataType == DataPersistenceConstants.AttributeNumericDataType);
            TextAttribute = _testHelper.UnitOfWork.Context.Attribute
                .First(_ => _.DataType == DataPersistenceConstants.AttributeTextDataType);
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
                    CurrentUserCriteriaFilter = new UserCriteriaDTO(), Expression = "", IsPiecewise = true
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
                    ValidationMessage = "Unsupported attribute FALSE_ATTRIBUTE"
                }
            };
        }

        [Fact]
        public async void ShouldReturnOkResultOnEquationPost()
        {
            // Arrange
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "(10,1)",
                IsPiecewise = true
            };

            // Act
            var result = await _controller.GetEquationValidationResult(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void ShouldValidateEquation()
        {
            // Arrange
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "(10,1)",
                IsPiecewise = true
            };

            // Act
            var result = await _controller.GetEquationValidationResult(model);

            // Assert
            var validationResult = (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value,
                typeof(ValidationResult));
            Assert.True(validationResult.IsValid);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public async void ShouldValidateNonPiecewiseEquation()
        {
            // Arrange
            NumericAttribute = _testHelper.UnitOfWork.Context.Attribute
                .First(_ => _.DataType == DataPersistenceConstants.AttributeNumericDataType);
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{NumericAttribute.Name}]*1",
                IsPiecewise = false
            };

            // Act
            var result = await _controller.GetEquationValidationResult(model);

            // Assert
            var validationResult = (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value,
                typeof(ValidationResult));
            Assert.True(validationResult.IsValid);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public async void ShouldValidateCriterion()
        {
            // Arrange
            SetData();
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{NumericAttribute.Name}]='1' AND [{TextAttribute.Name}]='test'"
            };

            // Act
            var result = await _controller.GetCriterionValidationResult(model);

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
            var invalidPiecewiseEquationValidationData = GetInvalidPiecewiseEquationValidationData().ToList();

            foreach (var testDataSet in invalidPiecewiseEquationValidationData)
            {
                var result = _controller.GetEquationValidationResult(testDataSet[0] as EquationValidationParameters);
                var objectResult = (OkObjectResult)result.Result;
                var actualValidationResult = (ValidationResult)objectResult.Value;
                var expectedValidationResult = testDataSet[1] as ValidationResult;

                Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
                Assert.Equal(expectedValidationResult.ValidationMessage, actualValidationResult.ValidationMessage);
            };
        }

        [Fact]
        public async void ShouldInvalidateNonPiecewiseEquation()
        {
            // Arrange
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "[FALSE_ATTRIBUTE]",
                IsPiecewise = false
            };

            // Act
            var result = await _controller.GetEquationValidationResult(model);

            // Assert
            var validationResult =
                (ValidationResult)Convert.ChangeType((result as OkObjectResult).Value, typeof(ValidationResult));

            Assert.False(validationResult.IsValid);
            Assert.Equal("Unsupported Attribute FALSE_ATTRIBUTE", validationResult.ValidationMessage);
        }

        [Fact]
        public void ShouldThrowCalculateEvaluateExceptionOnInvalidEquation()
        {
            // Arrange
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "",
                IsPiecewise = false
            };

            // Act + Assert
            Assert.ThrowsAsync<CalculateEvaluateException>(async () =>
                await _controller.GetEquationValidationResult(model));
        }

        [Fact]
        public void ShouldInvalidateCriteria()
        {
            var timer = new System.Timers.Timer { Interval = 5000 };
            // Act + Assert
            GetInvalidCriterionValidationData().ToList().ForEach(async testDataSet =>
            {
                timer.Elapsed += async delegate
                {
                    var result =
                        await _controller.GetCriterionValidationResult(testDataSet[0] as ValidationParameter);

                    var actualValidationResult =
                        (CriterionValidationResult)Convert.ChangeType((result as OkObjectResult).Value, typeof(CriterionValidationResult));

                    var expectedValidationResult = testDataSet[1] as CriterionValidationResult;

                    Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
                    Assert.Equal(expectedValidationResult.ResultsCount, actualValidationResult.ResultsCount);
                    Assert.Equal(expectedValidationResult.ValidationMessage, actualValidationResult.ValidationMessage);
                };
            });
        }

        [Fact]
        public async void ShouldReturnOkResultOnCriterionPost()
        {
            // Arrange                
            SetData();
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{NumericAttribute.Name}]='1'"
            };

            // Act
            var result = await _controller.GetCriterionValidationResult(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
