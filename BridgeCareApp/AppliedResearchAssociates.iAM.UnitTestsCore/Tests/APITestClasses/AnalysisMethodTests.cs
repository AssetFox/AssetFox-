﻿using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class AnalysisMethodTests
    {
        private readonly TestHelper _testHelper;
        private readonly AnalysisMethodController _controller;

        private static readonly Guid AnalysisMethodId = Guid.Parse("e93670b5-af82-4b58-9487-6eceed99b91e");
        private static readonly Guid BenefitId = Guid.Parse("be2497dd-3acd-4cdd-88a8-adeb9893f1df");

        public AnalysisMethodTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new AnalysisMethodController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object);
        }

        public AnalysisMethodEntity TestAnalysis { get; } = new AnalysisMethodEntity
        {
            Id = AnalysisMethodId,
            OptimizationStrategy = OptimizationStrategy.Benefit,
            SpendingStrategy = SpendingStrategy.NoSpending,
            ShouldApplyMultipleFeasibleCosts = false,
            ShouldDeteriorateDuringCashFlow = false,
            ShouldUseExtraFundsAcrossBudgets = false
        };

        public BenefitEntity TestBenefit { get; } = new BenefitEntity
        {
            Id = BenefitId,
            AnalysisMethodId = AnalysisMethodId,
            Limit = 1
        };

        private void SetupForGet()
        {
            TestAnalysis.SimulationId = _testHelper.TestSimulation.Id;
            _testHelper.UnitOfWork.Context.AnalysisMethod.Add(TestAnalysis);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsert()
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.AnalysisMethod(_testHelper.TestSimulation.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            try
            {
                // Arrange
                var attributeEntity = _testHelper.UnitOfWork.Context.Attribute.First();
                var dto = TestAnalysis.ToDto();
                TestBenefit.Attribute = attributeEntity;
                dto.Benefit = TestBenefit.ToDto();
                dto.Benefit.Attribute = attributeEntity.Name;

                // Act
                var result =
                    await _controller.UpsertAnalysisMethod(_testHelper.TestSimulation.Id, dto);

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAnalysisMethod()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.AnalysisMethod(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dto = (AnalysisMethodDTO)Convert.ChangeType(okObjResult.Value, typeof(AnalysisMethodDTO));

                Assert.Equal(AnalysisMethodId, dto.Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldCreateAnalysisMethod()
        {
            try
            {
                // Arrange
                var getResult = await _controller.AnalysisMethod(_testHelper.TestSimulation.Id);
                var analysisMethodDto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(AnalysisMethodDTO));

                analysisMethodDto.Benefit = new BenefitDTO
                {
                    Id = Guid.NewGuid(),
                    Limit = 0.0,
                    Attribute = _testHelper.UnitOfWork.Context.Attribute.First().Name
                };

                // Act
                await _controller.UpsertAnalysisMethod(_testHelper.TestSimulation.Id, analysisMethodDto);

                // Assert
                getResult = await _controller.AnalysisMethod(_testHelper.TestSimulation.Id);
                var upsertedAnalysisMethodDto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(AnalysisMethodDTO));

                Assert.Equal(analysisMethodDto.Id, upsertedAnalysisMethodDto.Id);
                Assert.Equal(analysisMethodDto.Benefit.Id, upsertedAnalysisMethodDto.Benefit.Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldUpdateAnalysisMethod()
        {
            try
            {
                // Arrange
                SetupForUpsert();
                var getResult = await _controller.AnalysisMethod(_testHelper.TestSimulation.Id);
                var dto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(AnalysisMethodDTO));
                var attributeEntity = _testHelper.UnitOfWork.Context.Attribute.First();
                dto.Attribute = attributeEntity.Name;
                dto.CriterionLibrary = _testHelper.TestCriterionLibrary.ToDto();
                TestBenefit.Attribute = attributeEntity;
                dto.Benefit = TestBenefit.ToDto();
                dto.Benefit.Attribute = attributeEntity.Name;

                // Act
                await _controller.UpsertAnalysisMethod(_testHelper.TestSimulation.Id, dto);

                // Assert
                var analysisMethodDto =
                    _testHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(_testHelper
                        .TestSimulation.Id);

                Assert.Equal(dto.Id, analysisMethodDto.Id);
                Assert.Equal(dto.Attribute, analysisMethodDto.Attribute);
                Assert.Equal(dto.CriterionLibrary.Id, analysisMethodDto.CriterionLibrary.Id);
                Assert.Equal(dto.Benefit.Id, analysisMethodDto.Benefit.Id);
                Assert.Equal(dto.Benefit.Attribute, analysisMethodDto.Benefit.Attribute);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}