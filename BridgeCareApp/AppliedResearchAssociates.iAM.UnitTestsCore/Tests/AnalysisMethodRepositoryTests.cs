using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class AnalysisMethodRepositoryTests
    {
        [Fact]
        public void ShouldReturnOkResultOnGet()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var entity = AnalysisMethodEntities.TestAnalysis(simulation.Id);
            TestHelper.UnitOfWork.Context.AnalysisMethod.Add(entity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            // Act
            var result = unitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);
            // Assert
            Assert.NotNull(result);
        }
    }
}
