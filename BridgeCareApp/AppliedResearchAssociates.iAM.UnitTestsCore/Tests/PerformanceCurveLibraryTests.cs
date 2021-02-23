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
    public class PerformanceCurveLibraryTests
    {
        private readonly PerformanceCurveLibraryTestHelper _testHelper;
        private readonly PerformanceCurveLibraryController _libraryController;

        public PerformanceCurveLibraryTests()
        {
            _testHelper = new PerformanceCurveLibraryTestHelper();
            _libraryController = new PerformanceCurveLibraryController(_testHelper.UnitOfDataPersistenceWork);
        }

        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            // Act
            var result = _libraryController.PerformanceCurveLibraries();

            // Assert
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public void ShouldReturnOkResultOnPost()
        {
            // Act
            var result = _libraryController
                .AddOrUpdatePerformanceCurveLibrary(Guid.Empty, new PerformanceCurveLibraryDTO());

            // Assert
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public void ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = _libraryController.DeletePerformanceCurveLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result.Result);
        }



        /*[Fact]
        public void ShouldReturnAllPerformanceCurveLibrariesOnGetAll()
        {
            // Act
            var result = _libraryController.PerformanceCurveLibraries();

            // Assert
            var okObjResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (List<PerformanceCurveLibraryDTO>) Convert.ChangeType(okObjResult.Value, typeof(List<PerformanceCurveLibraryDTO>));
            Assert.NotNull(dto);
        }*/
    }
}
