using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CalculatedAttributePagingServiceTests
    {
        private CalculatedAttributePagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CalculatedAttributePagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO>());
            var request = new CalculatedAttributePagingSyncModel
            {

            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            Assert.Empty(result);
        }


        [Fact]
        public void GetSyncedScenarioDataset_RepoReturnsDto_ReturnsDto()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var attributeId = Guid.NewGuid();
            var equationCriterionPairId = Guid.NewGuid();
            var equationId = Guid.NewGuid();
            var dto = CalculatedAttributeDtos.Age(attributeId, equationCriterionPairId, equationId);
            var cloneDto = CalculatedAttributeDtos.Age(attributeId, equationCriterionPairId, equationId);
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO> { dto });
            var request = new CalculatedAttributePagingSyncModel
            {

            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedDto = result.Single();
            ObjectAssertions.Equivalent(cloneDto, returnedDto);
        }

        [Fact]
        public void GetSyncedScenarioDataset_AddedDtoInRequest_ReturnsDto()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var attributeId = Guid.NewGuid();
            var equationCriterionPairId = Guid.NewGuid();
            var equationId = Guid.NewGuid();
            var dto = CalculatedAttributeDtos.Age(attributeId, equationCriterionPairId, equationId);
            var cloneDto = CalculatedAttributeDtos.Age(attributeId, equationCriterionPairId, equationId);
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO>());
            var request = new CalculatedAttributePagingSyncModel
            {
                AddedCalculatedAttributes = new List<CalculatedAttributeDTO> { dto },
            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedDto = result.Single();
            ObjectAssertions.Equivalent(cloneDto, returnedDto);
        }

        [Fact]
        public void GetSyncedScenarioDataset_NewLibrary_InitializesIds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var attributeId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var equationCriterionPairId = Guid.NewGuid();
            var equationId = Guid.NewGuid();
            var dto = CalculatedAttributeDtos.Age(attributeId, equationCriterionPairId, equationId);
            var theEquation = dto.Equations.Single();
            Assert.Equal(equationId, theEquation.Equation.Id);
            theEquation.CriteriaLibrary = new CriterionLibraryDTO();
            var cloneDto = CalculatedAttributeDtos.Age(attributeId, equationCriterionPairId, equationId);
            var returnLibrary = CalculatedAttributeLibraryDtos.Empty();
            returnLibrary.CalculatedAttributes.Add(dto);
            repo.Setup(r => r.GetCalculatedAttributeLibraryByID(libraryId)).Returns(returnLibrary);
            var request = new CalculatedAttributePagingSyncModel
            {
                LibraryId = libraryId,
            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedDto = result.Single();
            ObjectAssertions.EquivalentExcluding(cloneDto, returnedDto, x => x.Equations, x => x.Id);
            var returnedEquation = returnedDto.Equations.Single();
            Assert.NotEqual(attributeId, returnedDto.Id);
            Assert.NotEqual(Guid.Empty, returnedEquation.CriteriaLibrary.Id);
            Assert.NotEqual(equationId, returnedEquation.Equation.Id);
            Assert.NotEqual(equationCriterionPairId, returnedEquation.Id);
        }

        [Fact]
        public void GetScenarioPage_SortByEquation_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var attributeId1 = Guid.NewGuid();
            var attributeId2 = Guid.NewGuid();
            var equationCriterionPairId1 = Guid.NewGuid();
            var equationCriterionPairId2 = Guid.NewGuid();
            var equationId1 = Guid.NewGuid();
            var equationId2 = Guid.NewGuid();
            var attribute = AttributeDtos.Age;
            attributeRepo.Setup(a => a.GetSingleById(attribute.Id)).Returns(attribute);
            var dto = CalculatedAttributeDtos.EmptyForAttribute(attribute);
            var pair1 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair2 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair3 = CalculatedAttributeEquationCriteriaPairDtos.New();
            pair1.Equation.Expression = "a";
            pair2.Equation.Expression = "b";
            pair3.Equation.Expression = "c";
            dto.Equations = new List<CalculatedAttributeEquationCriteriaPairDTO> { pair1, pair3, pair2 };
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO> { dto });
            var request = new CalculatedAttributePagingSyncModel
            {
                AddedCalculatedAttributes = new List<CalculatedAttributeDTO> { dto },
            };
            var pagingRequest = new CalculatedAttributePagingRequestModel
            {
                AttributeId = attribute.Id,
                SyncModel = request,
                sortColumn = "equation",
            };

            var result = pagingService.GetScenarioPage(libraryId, pagingRequest);
            var items = result.Items;
            var expressions = items.Select(i => i.Equation.Expression).ToList();
            var expectedExpressions = new List<string> { "a", "b", "c" };
            ObjectAssertions.Equivalent(expectedExpressions, expressions);
        }

        [Fact]
        public void GetScenarioPage_SortDescendingByCriteriaExpression_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var attributeId1 = Guid.NewGuid();
            var attributeId2 = Guid.NewGuid();
            var equationCriterionPairId1 = Guid.NewGuid();
            var equationCriterionPairId2 = Guid.NewGuid();
            var equationId1 = Guid.NewGuid();
            var equationId2 = Guid.NewGuid();
            var attribute = AttributeDtos.Age;
            attributeRepo.Setup(a => a.GetSingleById(attribute.Id)).Returns(attribute);
            var dto = CalculatedAttributeDtos.EmptyForAttribute(attribute);
            var pair1 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair2 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair3 = CalculatedAttributeEquationCriteriaPairDtos.New();
            pair1.CriteriaLibrary.MergedCriteriaExpression = "a";
            pair2.CriteriaLibrary.MergedCriteriaExpression = "b";
            pair3.CriteriaLibrary.MergedCriteriaExpression = "c";
            dto.Equations = new List<CalculatedAttributeEquationCriteriaPairDTO> { pair1, pair3, pair2 };
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO> { dto });
            var request = new CalculatedAttributePagingSyncModel
            {
                AddedCalculatedAttributes = new List<CalculatedAttributeDTO> { dto },
            };
            var pagingRequest = new CalculatedAttributePagingRequestModel
            {
                AttributeId = attribute.Id,
                SyncModel = request,
                sortColumn = "criteriaexpression",
                isDescending = true,
            };

            var result = pagingService.GetScenarioPage(libraryId, pagingRequest);
            var items = result.Items;
            var criterionExpressions = items.Select(i => i.CriteriaLibrary.MergedCriteriaExpression).ToList();
            var expectedExpressions = new List<string> { "c", "b", "a" };
            ObjectAssertions.Equivalent(expectedExpressions, criterionExpressions);
        }


        [Fact]
        public void GetScenarioPage_Search_FindsInEquation()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var attributeId1 = Guid.NewGuid();
            var attributeId2 = Guid.NewGuid();
            var equationCriterionPairId1 = Guid.NewGuid();
            var equationCriterionPairId2 = Guid.NewGuid();
            var equationId1 = Guid.NewGuid();
            var equationId2 = Guid.NewGuid();
            var attribute = AttributeDtos.Age;
            attributeRepo.Setup(a => a.GetSingleById(attribute.Id)).Returns(attribute);
            var dto = CalculatedAttributeDtos.EmptyForAttribute(attribute);
            var pair1 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair2 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair3 = CalculatedAttributeEquationCriteriaPairDtos.New();
            pair1.Equation.Expression = "aAa";
            pair2.Equation.Expression = "bBb";
            pair3.Equation.Expression = "cCc";
            dto.Equations = new List<CalculatedAttributeEquationCriteriaPairDTO> { pair1, pair3, pair2 };
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO> { dto });
            var request = new CalculatedAttributePagingSyncModel
            {
                AddedCalculatedAttributes = new List<CalculatedAttributeDTO> { dto },
            };
            var pagingRequest = new CalculatedAttributePagingRequestModel
            {
                AttributeId = attribute.Id,
                SyncModel = request,
                search = "bbB",
            };

            var result = pagingService.GetScenarioPage(libraryId, pagingRequest);
            var item = result.Items.Single();
            Assert.Equal("bBb", item.Equation.Expression);
        }
        [Fact]
        public void GetScenarioPage_Search_FindsInMergedCriteriaExpression()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var attributeRepo = AttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var attributeId1 = Guid.NewGuid();
            var attributeId2 = Guid.NewGuid();
            var equationCriterionPairId1 = Guid.NewGuid();
            var equationCriterionPairId2 = Guid.NewGuid();
            var equationId1 = Guid.NewGuid();
            var equationId2 = Guid.NewGuid();
            var attribute = AttributeDtos.Age;
            attributeRepo.Setup(a => a.GetSingleById(attribute.Id)).Returns(attribute);
            var dto = CalculatedAttributeDtos.EmptyForAttribute(attribute);
            var pair1 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair2 = CalculatedAttributeEquationCriteriaPairDtos.New();
            var pair3 = CalculatedAttributeEquationCriteriaPairDtos.New();
            pair1.CriteriaLibrary.MergedCriteriaExpression = "aAa";
            pair2.CriteriaLibrary.MergedCriteriaExpression = "bBb";
            pair3.CriteriaLibrary.MergedCriteriaExpression = "cCc";
            dto.Equations = new List<CalculatedAttributeEquationCriteriaPairDTO> { pair1, pair3, pair2 };
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO> { dto });
            var request = new CalculatedAttributePagingSyncModel
            {
                AddedCalculatedAttributes = new List<CalculatedAttributeDTO> { dto },
            };
            var pagingRequest = new CalculatedAttributePagingRequestModel
            {
                AttributeId = attribute.Id,
                SyncModel = request,
                search = "bbB",
            };

            var result = pagingService.GetScenarioPage(libraryId, pagingRequest);

            var item = result.Items.Single();
            Assert.Equal("bBb", item.CriteriaLibrary.MergedCriteriaExpression);
        }

    }
}
