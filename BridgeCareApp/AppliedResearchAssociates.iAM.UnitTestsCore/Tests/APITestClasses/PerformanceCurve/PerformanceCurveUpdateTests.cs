using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class PerformanceCurveUpdateTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDb_UpdatesShiftAndDescription()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            performanceCurveLibraryDto.Description = "Updated Description";
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.Shift = true;

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Equal(performanceCurveLibraryDto.Description, performanceCurveLibraryDtoAfter.Description);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Shift, performanceCurveDtoAfter.Shift);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithEquation_EquationUnchanged()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var equation = EquationTestSetup.TwoWithJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.Equation = equation.ToDto();

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithRemovedEquation_EquationRemoved()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var equation = EquationTestSetup.TwoWithJoinInDb(_testHelper.UnitOfWork, null, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.Equation = null;

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Null(performanceCurveDtoAfter.Equation?.Expression);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithCriterionLibrary_CriterionLibraryUnchanged()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            var criterionCurveJoin = new CriterionLibraryPerformanceCurveEntity
            {
                PerformanceCurveId = performanceCurve.Id,
                CriterionLibraryId = criterionLibrary.Id
            };
            _testHelper.UnitOfWork.Context.Add(criterionCurveJoin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.CriterionLibrary = criterionLibrary.ToDto();

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(criterionLibrary.Id, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }


        [Fact]
        public async Task UpsertPerformanceCurveLibrary_CurveInDbWithRemovedCriterionLibrary_CriterionLibraryRemoved()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            var criterionCurveJoin = new CriterionLibraryPerformanceCurveEntity
            {
                PerformanceCurveId = performanceCurve.Id,
                CriterionLibraryId = criterionLibrary.Id
            };
            _testHelper.UnitOfWork.Context.Add(criterionCurveJoin);
            var performanceCurveLibraryDto = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            performanceCurveDto.CriterionLibrary = null;

            // Act
            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = _testHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(Guid.Empty, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }
    }
}
