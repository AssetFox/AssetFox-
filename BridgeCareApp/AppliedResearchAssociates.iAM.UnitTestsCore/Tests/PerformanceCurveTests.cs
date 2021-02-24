using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class PerformanceCurveTests
    {
        private readonly PerformanceCurveTestHelper _testHelper;
        private readonly PerformanceCurveController _controller;

        public PerformanceCurveTests()
        {
            _testHelper = new PerformanceCurveTestHelper();
            _controller = new PerformanceCurveController(_testHelper.UnitOfDataPersistenceWork);
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            // Act
            var result = _controller.PerformanceCurveLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void ShouldReturnOkResultOnPost()
        {
            // Act
            var result = _controller
                .AddOrUpdatePerformanceCurveLibrary(Guid.Empty, _testHelper.LibraryToAdd);

            // Assert
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public void ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = _controller.DeletePerformanceCurveLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public void ShouldReturnAllPerformanceCurveLibrariesOnGetAll()
        {
            // Arrange
            _testHelper.SetupForGet();

            // Act
            var result = _controller.PerformanceCurveLibraries();

            // Assert
            var okObjResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<PerformanceCurveLibraryDTO>) Convert.ChangeType(okObjResult.Value, typeof(List<PerformanceCurveLibraryDTO>));
            Assert.NotNull(dtos);
            Assert.Single(dtos);

            var library = dtos[0];
            Assert.Equal(library.Id, _testHelper.LibraryToAdd.Id);
            Assert.Single(library.PerformanceCurves);

            var curve = library.PerformanceCurves[0];
            Assert.Equal(curve.Id, _testHelper.CurveToAdd.Id);
        }

        [Fact]
        public void ShouldAddOrUpdatePerformanceCurveLibrariesWithPerformanceCurves()
        {
            // Arrange
            _testHelper.SetupForPost();

            // Act
            _testHelper.LibraryToUpdate.Name = "Updated";
            var equation = new EquationDTO {Id = Guid.NewGuid(), Expression = "(8,5)"};
            _testHelper.CurveToUpdate.Equation = equation;
            _testHelper.LibraryToUpdate.PerformanceCurves.Add(_testHelper.CurveToUpdate);


            // Assert
        }
    }
}
