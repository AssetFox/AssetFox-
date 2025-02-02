﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AnalysisMethodRepository : IAnalysisMethodRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AnalysisMethodRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));
        public void GetSimulationAnalysisMethod(Simulation simulation, string userCriteria)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if(!_unitOfWork.Context.AnalysisMethod.Any(_ => _.Simulation.Id == simulation.Id))
            {
                throw new RowNotInTableException("No analysis method was found for the given scenario.");
            }

           var analysisMethodEntity = _unitOfWork.Context.AnalysisMethod
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)

                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.ScenarioTargetConditionalGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.ScenarioTargetConditionalGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)

                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.ScenarioDeficientConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.ScenarioDeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)

                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)

                .AsNoTracking()
                .Single(_ => _.Simulation.Id == simulation.Id);

            // Atleast one budget priority should exist
            if(!analysisMethodEntity.Simulation.BudgetPriorities.Any())
            {
                throw new RowNotInTableException("No budget priority was found for the given scenario.");
            }

            analysisMethodEntity.FillSimulationAnalysisMethod(simulation, userCriteria);
            
        }

        public bool GetSimulationAnalysisMethodSetting(Guid simulationId)
        {
            return _unitOfWork.Context.AnalysisMethod.Any(_ => _.Simulation.Id == simulationId);
        }

        public AnalysisMethodDTO GetAnalysisMethod(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.AnalysisMethod.Any(_ => _.SimulationId == simulationId))
            {
                return new AnalysisMethodDTO
                {
                    Id = Guid.NewGuid(),
                    OptimizationStrategy = OptimizationStrategy.Benefit,
                    SpendingStrategy = SpendingStrategy.NoSpending,
                    ShouldApplyMultipleFeasibleCosts = false,
                    ShouldDeteriorateDuringCashFlow = false,
                    ShouldUseExtraFundsAcrossBudgets = false,
                    ShouldAllowMultipleTreatments = false,
                    Benefit = new BenefitDTO(),
                    CriterionLibrary = new CriterionLibraryDTO()
                };
            }

            return _unitOfWork.Context.AnalysisMethod
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.SimulationId == simulationId)
                .ToDto();
        }

        public void UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            AttributeEntity attributeEntity = null;

            if (!string.IsNullOrEmpty(dto.Attribute))
            {
                if (!_unitOfWork.Context.Attribute.Any(_ => _.Name == dto.Attribute))
                {
                    throw new RowNotInTableException($"No attribute found having name {dto.Attribute}.");
                }

                attributeEntity = _unitOfWork.Context.Attribute.Single(_ => _.Name == dto.Attribute);
            }

            var analysisMethodEntity = dto.ToEntity(simulationId, attributeEntity?.Id);

            _unitOfWork.Context.Upsert(analysisMethodEntity, _ => _.Id == dto.Id, _unitOfWork.UserEntity?.Id);

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);

            _unitOfWork.BenefitRepo.UpsertBenefit(dto.Benefit, dto.Id);          

            _unitOfWork.Context.DeleteEntity<CriterionLibraryAnalysisMethodEntity>(_ => _.AnalysisMethodId == dto.Id);

            var oldCriterionEntity = _unitOfWork.Context.CriterionLibrary.SingleOrDefault(_ => _.Id == dto.CriterionLibrary.Id);

            if (dto.CriterionLibrary?.Id != null && dto.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(dto.CriterionLibrary.MergedCriteriaExpression))
            {

                var criterionJoinEntity = new CriterionLibraryAnalysisMethodEntity
                {
                    CriterionLibraryId = dto.CriterionLibrary.Id,
                    AnalysisMethodId = dto.Id
                };

                if (oldCriterionEntity == null)
                {
                    var criterionEntity = dto.CriterionLibrary.ToEntity();
                    dto.CriterionLibrary.Name = criterionEntity.Name ?? "";
                    _unitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(dto.CriterionLibrary);
                }
                else if (oldCriterionEntity.MergedCriteriaExpression != dto.CriterionLibrary.MergedCriteriaExpression)
                    _unitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(dto.CriterionLibrary);


                _unitOfWork.Context.AddEntity(criterionJoinEntity, _unitOfWork.UserEntity?.Id);
            }
            else if (oldCriterionEntity != null)
                _unitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(oldCriterionEntity.Id);
        }
    }
}
