using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class ExpressionValidationTests
    {
        private ExpressionValidationService CreateValidationService(Mock<IUnitOfWork> unitOfWork)
        {
            var log = new LogNLog();
            return new ExpressionValidationService(unitOfWork.Object, log);
        }

        private ExpressionValidationController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var service = CreateValidationService(unitOfWork);
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new ExpressionValidationController(
                service,
                EsecSecurityMocks.Admin,
                unitOfWork.Object,
                hubService,
                accessor);
            return controller;
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
        public async Task ValidateEquation_OkObjectResult()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var controller = CreateController(unitOfWork);
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
            ActionResultAssertions.OkObject(result);
        }

        [Fact]
        public async Task ValidateValidEquation_Success()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var controller = CreateController(unitOfWork);
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "(10,1)",
                IsPiecewise = true
            };

            // Act
            var result = await controller.GetEquationValidationResult(model);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var validationResult = value as ValidationResult;
            Assert.True(validationResult.IsValid);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public async Task ValidateValidNonPiecewiseEquation_Valid()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var attributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.CulvDurationN,
            };
            attributeRepo.Setup(r => r.GetAllAttributesAbbreviated()).Returns(attributes);
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
            Assert.True(validationResult.IsValid);
            Assert.Equal("Success", validationResult.ValidationMessage);
        }

        [Fact]
        public async Task ValidateValidCriterion_Valid()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var aggregatedResultRepo = AggregatedResultRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var attributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.ActionType,
                AbbreviatedAttributeDtos.CulvDurationN,
            };
            var assetId = Guid.NewGuid();
            var aggregatedResult1 = AggregatedResultDtos.Text(AbbreviatedAttributeDtos.ActionType, assetId, "test");
            var aggregatedResult2 = AggregatedResultDtos.Number(AbbreviatedAttributeDtos.CulvDurationN, assetId, 1);
            var aggregatedResults = new List<AggregatedResultDTO> { aggregatedResult1, aggregatedResult2 };
            var networkId = Guid.NewGuid();
            aggregatedResultRepo.Setup(a => a.GetAggregatedResultsForAttributeNames(networkId, It.Is<List<string>>(list => list.Count() == 2 && list.Contains(TestAttributeNames.CulvDurationN) && list.Contains(TestAttributeNames.ActionType)))).Returns(aggregatedResults);
            attributeRepo.Setup(r => r.GetAllAttributesAbbreviated()).Returns(attributes);
            attributeRepo.Setup(r => r.GetAttributesWithNames(It.Is<List<string>>(list => list.Count() == 2
            && list.Contains(TestAttributeNames.CulvDurationN) && list.Contains(TestAttributeNames.ActionType)))).Returns(attributes);
            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{AttributeDtos.CulvDurationN.Name}]='1' AND [{AttributeDtos.ActionType.Name}]='test'",
                NetworkId = networkId,
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
        public void ValidateInvalidPiecewiseEquation_NotValid()
        {
            // Act + Assert
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var controller = CreateController(unitOfWork);
            var invalidPiecewiseEquationValidationData = GetInvalidPiecewiseEquationValidationData().ToList();

            foreach (var testDataSet in invalidPiecewiseEquationValidationData)
            {
                var invalidEquation = testDataSet[0] as EquationValidationParameters;
                var result = controller.GetEquationValidationResult(invalidEquation);
                var objectResult = (OkObjectResult)result.Result;
                var actualValidationResult = (ValidationResult)objectResult.Value;
                var expectedValidationResult = testDataSet[1] as ValidationResult;
                ObjectAssertions.Equivalent(expectedValidationResult, actualValidationResult);  
                Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
                Assert.Equal(expectedValidationResult.ValidationMessage, actualValidationResult.ValidationMessage);
            };
        }

        [Fact]
        public async Task ValidateInvalidNonPiecewiseEquation_NotValid ()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var controller = CreateController(unitOfWork);
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "[FALSE_ATTRIBUTE]",
                IsPiecewise = false
            };
            var attributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.ActionType,
                AbbreviatedAttributeDtos.CulvDurationN,
            };
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            attributeRepo.Setup(a => a.GetAllAttributesAbbreviated()).Returns(attributes);
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
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var attributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.ActionType,
                AbbreviatedAttributeDtos.CulvDurationN,
            };
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            attributeRepo.Setup(a => a.GetAllAttributesAbbreviated()).Returns(attributes);
            var service = CreateValidationService(unitOfWork);
            var model = new EquationValidationParameters
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = "",
                IsPiecewise = false
            };
            // Act
            var result = service.ValidateEquation(model);
            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateInvalidCriterion_NotValid()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var attributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.ActionType,
                AbbreviatedAttributeDtos.CulvDurationN,
            };
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            attributeRepo.Setup(a => a.GetAllAttributesAbbreviated()).Returns(attributes);
            var controller = CreateController(unitOfWork);
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

                ObjectAssertions.Equivalent(expectedValidationResult, actualValidationResult);
            }
        }

        [Fact]
        public async Task ValidateValidCriterionViaController_Valid()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var aggregatedResultRepo = AggregatedResultRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var attributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.ActionType,
                AbbreviatedAttributeDtos.CulvDurationN,
            };
            var culvDurationAttributes = new List<AbbreviatedAttributeDTO>
            {
                AbbreviatedAttributeDtos.CulvDurationN
            };
            var assetId = Guid.NewGuid();
            var aggregatedResult2 = AggregatedResultDtos.Number(AbbreviatedAttributeDtos.CulvDurationN, assetId, 1);
            var aggregatedResults = new List<AggregatedResultDTO> { aggregatedResult2 };
            var networkId = Guid.NewGuid();
            aggregatedResultRepo.Setup(a => a.GetAggregatedResultsForAttributeNames(networkId, It.Is<List<string>>(list => list.Count() == 1
            && list.Contains(TestAttributeNames.CulvDurationN)))).Returns(aggregatedResults);
            attributeRepo.Setup(r => r.GetAllAttributesAbbreviated()).Returns(attributes);
            attributeRepo.Setup(r => r.GetAttributesWithNames(It.Is<List<string>>(list => list.Count() == 1
            && list.Contains(TestAttributeNames.CulvDurationN)))).Returns(culvDurationAttributes);

            var model = new ValidationParameter
            {
                CurrentUserCriteriaFilter = new UserCriteriaDTO(),
                Expression = $"[{AttributeDtos.CulvDurationN.Name}]='1'",
                NetworkId = networkId,
            };

            // Act
            var result = await controller.GetCriterionValidationResult(model);
            var expected = new CriterionValidationResult
            {
                IsValid = true,
                ValidationMessage = "Success",
                ResultsCount = 1,
            };
            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.Equivalent(expected, value);
        }
    }
}
