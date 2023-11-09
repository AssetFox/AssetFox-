using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Benefit;

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


        [Fact]
        public void ShouldCreateAnalysisMethod()
        {
            // Arrange
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var repo = unitOfWork.AnalysisMethodRepo;
            var analysisMethodDto = repo.GetAnalysisMethod(simulation.Id);
            analysisMethodDto.Benefit = new BenefitDTO
            {
                Id = Guid.NewGuid(),
                Limit = 0.0,
                Attribute = TestHelper.UnitOfWork.Context.Attribute.First().Name
            };

            // Act
            repo.UpsertAnalysisMethod(simulation.Id, analysisMethodDto);
            // Assert
            var upsertedAnalysisMethodDto = repo.GetAnalysisMethod(simulation.Id);
            Assert.Equal(analysisMethodDto.Id, upsertedAnalysisMethodDto.Id);
            Assert.Equal(analysisMethodDto.Benefit.Id, upsertedAnalysisMethodDto.Benefit.Id);
        }

        private BenefitEntity TestBenefit(Guid analysisMethodId, Guid? benefitId = null)
        {
            var resolveId = benefitId ?? Guid.NewGuid();
            var returnValue = new BenefitEntity
            {
                Id = resolveId,
                AnalysisMethodId = analysisMethodId,
                Limit = 1
            };
            return returnValue;
        }

        [Fact]
        public void AnalysisMethodInDb_Update_Updates()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var repo = unitOfWork.AnalysisMethodRepo;

            var analysisMethodDto = repo.GetAnalysisMethod(simulation.Id);
            var attributeEntity = TestHelper.UnitOfWork.Context.Attribute.First();
            analysisMethodDto.Attribute = attributeEntity.Name;
            analysisMethodDto.CriterionLibrary = criterionLibrary;
            var analysisMethod = AnalysisMethodEntities.TestAnalysis(simulation.Id);
            var benefitDto = BenefitDtos.Dto(attributeEntity.Name);
            analysisMethodDto.Benefit = benefitDto;

            // Act
            repo.UpsertAnalysisMethod(simulation.Id, analysisMethodDto);

            // Assert
            var analysisMethodDtoAfter = repo.GetAnalysisMethod(simulation.Id);

            Assert.Equal(analysisMethodDtoAfter.Id, analysisMethodDto.Id);
            Assert.Equal(analysisMethodDtoAfter.Attribute, analysisMethodDto.Attribute);
            Assert.Equal(analysisMethodDtoAfter.CriterionLibrary.Id, analysisMethodDto.CriterionLibrary.Id);
            Assert.Equal(analysisMethodDtoAfter.Benefit.Id, analysisMethodDto.Benefit.Id);
            Assert.Equal(analysisMethodDtoAfter.Benefit.Attribute, analysisMethodDto.Benefit.Attribute);
        }
    }
}
