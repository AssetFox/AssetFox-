﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class SimulationTests
    {
        private readonly TestHelper _testHelper;
        private readonly SimulationController _controller;

        private static readonly Guid UserId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");

        public SimulationTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            var simulationAnalysis =
                new SimulationAnalysisService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object);
            _controller = new SimulationController(_testHelper.MockEsecSecurity, _testHelper.UnitOfWork,
                simulationAnalysis, _testHelper.MockHubService.Object);
        }

        private void SetupForClone()
        {
            _testHelper.UnitOfWork.Context.AddEntity(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.AddEntity(_testHelper.TestUser);
            _testHelper.UnitOfWork.Context.AddEntity(new SimulationUserEntity
            {
                SimulationId = _testHelper.TestSimulation.Id,
                UserId = UserId,
                CanModify = true,
                IsOwner = true,
                CreatedBy = UserId,
                LastModifiedBy = UserId
            });

            var analysisMethodId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(new AnalysisMethodEntity
            {
                Id = analysisMethodId,
                SimulationId = _testHelper.TestSimulation.Id,
            });
            _testHelper.UnitOfWork.Context.AddEntity(new BenefitEntity
            {
                Id = Guid.NewGuid(),
                AnalysisMethodId = analysisMethodId,
                AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id
            });
            _testHelper.UnitOfWork.Context.AddEntity(new CriterionLibraryAnalysisMethodEntity
            {
                CriterionLibraryId = _testHelper.TestCriterionLibrary.Id,
                AnalysisMethodId = analysisMethodId
            });

            _testHelper.UnitOfWork.Context.AddEntity(new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = _testHelper.TestSimulation.Id
            });

            var budgetLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new BudgetLibraryEntity {Id = budgetLibraryId, Name = "Test Name"});
            _testHelper.UnitOfWork.Context.AddEntity(new BudgetLibrarySimulationEntity
            {
                BudgetLibraryId = budgetLibraryId, SimulationId = _testHelper.TestSimulation.Id
            });

            var budgetId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new BudgetEntity {Id = budgetId, BudgetLibraryId = budgetLibraryId, Name = "Test Budget Name"});

            var budgetPriorityLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new BudgetPriorityLibraryEntity { Id = budgetPriorityLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var cashFlowRuleLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new CashFlowRuleLibraryEntity { Id = cashFlowRuleLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new CashFlowRuleLibrarySimulationEntity
            {
                CashFlowRuleLibraryId = cashFlowRuleLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var deficientConditionGoalLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new DeficientConditionGoalLibraryEntity { Id = deficientConditionGoalLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new DeficientConditionGoalLibrarySimulationEntity
            {
                DeficientConditionGoalLibraryId = deficientConditionGoalLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var performanceCurveLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new PerformanceCurveLibraryEntity { Id = performanceCurveLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new PerformanceCurveLibrarySimulationEntity
            {
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var remainingLifeLimitLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new RemainingLifeLimitLibraryEntity { Id = remainingLifeLimitLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new RemainingLifeLimitLibrarySimulationEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var targetConditionGoalLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new TargetConditionGoalLibraryEntity { Id = targetConditionGoalLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new TargetConditionGoalLibrarySimulationEntity
            {
                TargetConditionGoalLibraryId = targetConditionGoalLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var treatmentLibraryId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(
                new TreatmentLibraryEntity { Id = treatmentLibraryId, Name = "Test Name" });
            _testHelper.UnitOfWork.Context.AddEntity(new TreatmentLibrarySimulationEntity
            {
                TreatmentLibraryId = treatmentLibraryId,
                SimulationId = _testHelper.TestSimulation.Id
            });

            var maintainableAssetId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(new MaintainableAssetEntity
            {
                Id = maintainableAssetId,
                NetworkId = _testHelper.TestNetwork.Id,
                FacilityName = "1",
                SectionName = "0123456789",
                Area = 4422,
                AreaUnit = "ft^2"
            });

            _testHelper.UnitOfWork.Context.AddEntity(
                new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceConstants.SectionLocation, "10123456789")
                {
                    MaintainableAssetId = maintainableAssetId
                });

            var committedProjectId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.AddEntity(new CommittedProjectEntity
            {
                Id = committedProjectId,
                SimulationId = _testHelper.TestSimulation.Id,
                BudgetId = budgetId,
                MaintainableAssetId = maintainableAssetId,
                Name = "Test Name"
            });

            _testHelper.UnitOfWork.Context.AddEntity(new CommittedProjectConsequenceEntity
            {
                Id = Guid.NewGuid(),
                AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id,
                CommittedProjectId = committedProjectId,
                ChangeValue = "+1"
            });
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.GetSimulations(_testHelper.TestNetwork.Id);

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
                // Act
                var dto = _testHelper.TestSimulation.ToDto(null);
                dto.Id = Guid.NewGuid();
                var result = await _controller.CreateSimulation(_testHelper.TestNetwork.Id, dto);

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
        public async void ShouldReturnOkResultOnPut()
        {
            try
            {
                // Act
                var result = await _controller.UpdateSimulation(_testHelper.TestSimulation.ToDto(null));

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
        public async void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Act
                var result = await _controller.DeleteSimulation(Guid.Empty);

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
        public async void ShouldGetAllSimulations()
        {
            try
            {
                // Act
                var result = await _controller.GetSimulations(_testHelper.TestNetwork.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<SimulationDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<SimulationDTO>));
                Assert.Single(dtos);

                Assert.Equal(_testHelper.TestSimulation.Id, dtos[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldCreateSimulation()
        {
            try
            {
                // Arrange
                _testHelper.UnitOfWork.Context.AddEntity(_testHelper.TestUser);

                var newSimulationDTO = _testHelper.TestSimulation.ToDto(null);
                newSimulationDTO.Id = Guid.NewGuid();
                newSimulationDTO.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = _testHelper.TestUser.Id, Username = _testHelper.TestUser.Username, CanModify = true, IsOwner = true
                    }
                };

                // Act
                var result = await _controller.CreateSimulation(_testHelper.TestNetwork.Id, newSimulationDTO) as OkObjectResult;
                var dto = (SimulationDTO)Convert.ChangeType(result!.Value, typeof(SimulationDTO));

                var simulationEntity = _testHelper.UnitOfWork.Context.Simulation
                    .Include(_ => _.SimulationUserJoins)
                    .ThenInclude(_ => _.User)
                    .SingleOrDefault(_ => _.Id == dto.Id);

                // Assert
                Assert.NotNull(simulationEntity);
                Assert.Equal(dto.Users[0].UserId, simulationEntity.CreatedBy);

                var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
                Assert.Single(simulationUsers);
                Assert.Equal(dto.Users[0].UserId, simulationUsers[0].UserId);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldUpdateSimulation()
        {
            try
            {
                // Arrange
                _testHelper.UnitOfWork.Context.AddEntity(_testHelper.TestUser);

                var getResult = await _controller.GetSimulations(_testHelper.TestNetwork.Id);
                var dtos = (List<SimulationDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<SimulationDTO>));

                var simulationDTO = dtos[0];
                simulationDTO.Name = "Updated Name";
                simulationDTO.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = _testHelper.TestUser.Id, Username = _testHelper.TestUser.Username, CanModify = true, IsOwner = true
                    }
                };

                // Act
                var result = await _controller.UpdateSimulation(simulationDTO);
                var dto = (SimulationDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(SimulationDTO));

                // Assert
                var simulationEntity = _testHelper.UnitOfWork.Context.Simulation
                    .Include(_ => _.SimulationUserJoins)
                    .ThenInclude(_ => _.User)
                    .Single(_ => _.Id == dto.Id);

                Assert.Equal(dto.Name, simulationEntity.Name);

                var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
                Assert.Single(simulationUsers);
                Assert.Equal(dto.Users[0].UserId, simulationUsers[0].UserId);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteSimulation()
        {
            try
            {
                // Arrange
                _testHelper.UnitOfWork.Context.AddEntity(_testHelper.TestUser);

                var getResult = await _controller.GetSimulations(_testHelper.TestNetwork.Id);
                var dtos = (List<SimulationDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<SimulationDTO>));

                var dto = dtos[0];
                dto.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = _testHelper.TestUser.Id, Username = _testHelper.TestUser.Username, CanModify = true, IsOwner = true
                    }
                };

                await _controller.UpdateSimulation(dto);

                // Act
                await _controller.DeleteSimulation(dto.Id);

                // Assert

                Assert.True(!_testHelper.UnitOfWork.Context.Simulation.Any(_ => _.Id == dto.Id));
                Assert.True(!_testHelper.UnitOfWork.Context.SimulationUser.Any(_ => _.UserId == dto.Users[0].UserId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldCloneSimulation()
        {
            try
            {
                // Arrange
                SetupForClone();

                // Act
                var result = await _controller.CloneSimulation(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);
                var dto = (SimulationDTO)Convert.ChangeType(okObjResult.Value, typeof(SimulationDTO));

                var originalSimulation = _testHelper.UnitOfWork.Context.Simulation
                    .Include(_ => _.AnalysisMethod)
                    .ThenInclude(_ => _.Benefit)
                    .Include(_ => _.AnalysisMethod)
                    .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                    .Include(_ => _.InvestmentPlan)
                    .Include(_ => _.BudgetLibrarySimulationJoin)
                    .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                    .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                    .Include(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                    .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                    .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                    .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)
                    .Include(_ => _.TreatmentLibrarySimulationJoin)
                    .Include(_ => _.CommittedProjects)
                    .ThenInclude(_ => _.CommittedProjectConsequences)
                    .Include(_ => _.SimulationUserJoins)
                    .Single(_ => _.Id == _testHelper.TestSimulation.Id);

                var clonedSimulation = _testHelper.UnitOfWork.Context.Simulation
                    .Include(_ => _.AnalysisMethod)
                    .ThenInclude(_ => _.Benefit)
                    .Include(_ => _.AnalysisMethod)
                    .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                    .Include(_ => _.InvestmentPlan)
                    .Include(_ => _.BudgetLibrarySimulationJoin)
                    .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                    .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                    .Include(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                    .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                    .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                    .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)
                    .Include(_ => _.TreatmentLibrarySimulationJoin)
                    .Include(_ => _.CommittedProjects)
                    .ThenInclude(_ => _.CommittedProjectConsequences)
                    .Include(_ => _.SimulationUserJoins)
                    .Single(_ => _.Id == dto.Id);

                Assert.NotEqual(clonedSimulation.Id, originalSimulation.Id);
                Assert.Equal(clonedSimulation.Name, originalSimulation.Name);
                Assert.NotEqual(clonedSimulation.AnalysisMethod.Id, originalSimulation.AnalysisMethod.Id);
                Assert.Equal(clonedSimulation.AnalysisMethod.Benefit.AttributeId,
                    originalSimulation.AnalysisMethod.Benefit.AttributeId);
                Assert.Equal(clonedSimulation.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibraryId,
                    originalSimulation.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibraryId);
                Assert.NotEqual(clonedSimulation.InvestmentPlan.Id, originalSimulation.InvestmentPlan.Id);
                Assert.Equal(clonedSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod,
                    originalSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod);
                Assert.Equal(clonedSimulation.BudgetLibrarySimulationJoin.BudgetLibraryId,
                    originalSimulation.BudgetLibrarySimulationJoin.BudgetLibraryId);
                Assert.Equal(clonedSimulation.BudgetPriorityLibrarySimulationJoin.BudgetPriorityLibraryId,
                    originalSimulation.BudgetPriorityLibrarySimulationJoin.BudgetPriorityLibraryId);
                Assert.Equal(clonedSimulation.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId,
                    originalSimulation.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId);
                Assert.Equal(
                    clonedSimulation.DeficientConditionGoalLibrarySimulationJoin.DeficientConditionGoalLibraryId,
                    originalSimulation.DeficientConditionGoalLibrarySimulationJoin.DeficientConditionGoalLibraryId);
                Assert.Equal(clonedSimulation.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId,
                    originalSimulation.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId);
                Assert.Equal(clonedSimulation.RemainingLifeLimitLibrarySimulationJoin.RemainingLifeLimitLibraryId,
                    originalSimulation.RemainingLifeLimitLibrarySimulationJoin.RemainingLifeLimitLibraryId);
                Assert.Equal(clonedSimulation.TargetConditionGoalLibrarySimulationJoin.TargetConditionGoalLibraryId,
                    originalSimulation.TargetConditionGoalLibrarySimulationJoin.TargetConditionGoalLibraryId);
                Assert.Equal(clonedSimulation.TreatmentLibrarySimulationJoin.TreatmentLibraryId,
                    originalSimulation.TreatmentLibrarySimulationJoin.TreatmentLibraryId);
                var clonedCommittedProjects = clonedSimulation.CommittedProjects.ToList();
                var originalCommittedProjects = originalSimulation.CommittedProjects.ToList();
                Assert.Equal(clonedCommittedProjects.Count, originalCommittedProjects.Count);
                Assert.NotEqual(clonedCommittedProjects[0].Id, originalCommittedProjects[0].Id);
                Assert.Equal(clonedCommittedProjects[0].Name, originalCommittedProjects[0].Name);
                var clonedCommittedProjectConsequences =
                    clonedCommittedProjects[0].CommittedProjectConsequences.ToList();
                var originalCommittedProjectConsequences =
                    originalCommittedProjects[0].CommittedProjectConsequences.ToList();
                Assert.Equal(clonedCommittedProjectConsequences.Count, originalCommittedProjectConsequences.Count);
                Assert.NotEqual(clonedCommittedProjectConsequences[0].Id, originalCommittedProjectConsequences[0].Id);
                Assert.Equal(clonedCommittedProjectConsequences[0].AttributeId, originalCommittedProjectConsequences[0].AttributeId);
                var clonedSimulationUsers = clonedSimulation.SimulationUserJoins.ToList();
                var originalSimulationUsers = originalSimulation.SimulationUserJoins.ToList();
                Assert.Equal(clonedSimulationUsers.Count, originalSimulationUsers.Count);
                Assert.NotEqual(clonedSimulationUsers[0].SimulationId, originalSimulationUsers[0].SimulationId);
                Assert.Equal(clonedSimulationUsers[0].IsOwner, originalSimulationUsers[0].IsOwner);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
