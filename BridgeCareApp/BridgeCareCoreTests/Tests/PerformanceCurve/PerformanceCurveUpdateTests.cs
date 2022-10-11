using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace BridgeCareCoreTests.Tests
{
    public class PerformanceCurveUpdateTests
    {

        // Wjwjwj -- change these to repo-level tests.
        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithEquation_EquationUnchanged()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>()
                }
            };

            // Act
            await controller.UpsertPerformanceCurveLibrary(request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }


        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithCriterionLibrary_UpdateRemovesCriterionLibraryFromCurve_CriterionLibraryRemoved()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var mergedExpression = RandomStrings.WithPrefix("MergedExpression");
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork, "Performance Curve", mergedExpression);
            PerformanceCurveCriterionLibraryJoinTestSetup.JoinPerformanceCurveToCriterionLibrary(
                TestHelper.UnitOfWork, curveId, "meow", mergedExpression);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.CriterionLibrary = null;

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>()
                }
            };

            // Act
            await controller.UpsertPerformanceCurveLibrary(request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(Guid.Empty, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == libraryId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithCriterionLibrary_CurveRemovedFromUpsertedLibrary_RemovesCurveAndCriterionJoin()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var mergedExpression = RandomStrings.WithPrefix("MergedExpression");
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork, "Performance Curve", mergedExpression);
            PerformanceCurveCriterionLibraryJoinTestSetup.JoinPerformanceCurveToCriterionLibrary(TestHelper.UnitOfWork, performanceCurve.Id, "meow", mergedExpression);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    RowsForDeletion = new List<Guid> { curveId },
                    AddedRows = new List<PerformanceCurveDTO>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };

            // Act
            await controller.UpsertPerformanceCurveLibrary(request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Empty(performanceCurveLibraryDtoAfter.PerformanceCurves);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == libraryId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithEquation_UpdateChangesExpression_ExpressionChangedInDb()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.Equation.Expression = "123";

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    UpdateRows = new List<PerformanceCurveDTO> { performanceCurveDto},
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>()
                }
            };

            // Act
            await controller.UpsertPerformanceCurveLibrary(request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves[0];
            Assert.Equal("123", performanceCurveAfter.Equation.Expression);
            Assert.Equal(performanceCurve.Equation.Id, performanceCurveAfter.Equation.Id);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithEquation_UpdateRemovesCurve_EquationDeleted()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            performanceCurveLibraryDto.PerformanceCurves.RemoveAt(0);

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    RowsForDeletion = new List<Guid> { curveId },
                    AddedRows = new List<PerformanceCurveDTO>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };

            // Act
            await controller.UpsertPerformanceCurveLibrary(request);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Empty(performanceCurveLibraryDtoAfter.PerformanceCurves);
        }

        [Fact]
        public async Task UpsertPerformanceCurve_EmptyLibraryInDb_Adds()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };


            await controller.UpsertPerformanceCurveLibrary(request);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Single(performanceCurveLibraryDtoAfter.PerformanceCurves);
        }

        [Fact]
        public async Task UpsertPerformanceCurveWithEquation_EmptyLibraryInDb_AddsCurveAndEquationToLibrary()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var equation = new EquationDTO
            {
                Expression = "3",
                Id = Guid.NewGuid(),
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                Equation = equation,
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
   
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };


            await controller.UpsertPerformanceCurveLibrary(request);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var equationAfter = performanceCurveAfter.Equation;
            Assert.Equal("3", equationAfter.Expression);
            var equationEntity = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.NotNull(equationEntity);
        }

        [Fact]
        public async Task UpsertPerformanceCurveWithEquationWithEmptyId_EmptyLibraryInDb_AddsCurveToLibraryWithoutEquation()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var equation = new EquationDTO
            {
                Expression = "3",
                Id = Guid.Empty,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                Equation = equation,
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
 
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };


            await controller.UpsertPerformanceCurveLibrary(request);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var equationAfter = performanceCurveAfter.Equation;
            Assert.Null (equationAfter.Expression);
        }


        [Fact]
        public async Task UpsertPerformanceCurveWithCriterionLibrary_EmptyLibraryInDb_AddsPerformanceCurveAndCriterionLibrary()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var criterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                MergedCriteriaExpression = "MergedCriteriaExpression",
                Description = "Description",
                IsSingleUse = true,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                CriterionLibrary = criterionLibrary,
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);

            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = libraryDto,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = libraryDto.Id,
                    AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };


            await controller.UpsertPerformanceCurveLibrary(request);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var criterionLibraryAfter = performanceCurveDtoAfter.CriterionLibrary;
            Assert.Equal("MergedCriteriaExpression", criterionLibraryAfter.MergedCriteriaExpression);
        }
    }
}
