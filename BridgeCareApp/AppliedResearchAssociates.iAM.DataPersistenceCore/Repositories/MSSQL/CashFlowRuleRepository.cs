using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowRuleRepository : ICashFlowRuleRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CashFlowRuleRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateCashFlowRuleLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var cashFlowRuleLibraryEntity = new CashFlowRuleLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.CashFlowRuleLibrary.Add(cashFlowRuleLibraryEntity);

            _unitOfWork.Context.CashFlowRuleLibrarySimulation.Add(new CashFlowRuleLibrarySimulationEntity
            {
                CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id,
                SimulationId = simulationId
            });

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.CashFlowRuleLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException("The specified cash flow rule library was not found.");
            }

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(simulationEntity.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId))
                .ToList();

            _unitOfWork.Context.AddAll(cashFlowRuleEntities, _unitOfWork.UserEntity?.Id);

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);

            if (cashFlowRules.Any(_ => _.DistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules
                    .Where(_ => _.DistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.DistributionRules.ToList());

                _unitOfWork.CashFlowDistributionRuleRepo.CreateCashFlowDistributionRules(distributionRulesPerCashFlowRuleId);
            }

            if (cashFlowRules.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var cashFlowRuleEntityIdsPerExpression = cashFlowRules
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _.Id)
                    .ToDictionary(_ => _.Key, _ => _.ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(
                    cashFlowRuleEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.CashFlowRule, simulationEntity.Name);
            }
        }

        public List<CashFlowRuleLibraryDTO> CashFlowRuleLibrariesWithCashFlowRules()
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any())
            {
                return new List<CashFlowRuleLibraryDTO>();
            }

            return _unitOfWork.Context.CashFlowRuleLibrary
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CashFlowDistributionRules)
                .Include(_ => _.CashFlowRuleLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto, Guid simulationId)
        {
            var cashFlowRuleLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(cashFlowRuleLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException("No simulation was found for the given scenario.");
                }

                _unitOfWork.Context.DeleteEntity<CashFlowRuleLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(new CashFlowRuleLibrarySimulationEntity
                {
                    CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id,
                    SimulationId = simulationId
                }, _unitOfWork.UserEntity?.Id);

                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
                _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
            }
        }

        public void UpsertOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId)
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified cash flow rule library was not found.");
            }

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(libraryId))
                .ToList();

            var entityIds = cashFlowRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.CashFlowRule
                .Where(_ => _.CashFlowRuleLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<CashFlowRuleEntity>(_ =>
                _.CashFlowRuleLibrary.Id == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(cashFlowRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(cashFlowRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.DeleteAll<CriterionLibraryCashFlowRuleEntity>(_ =>
                _.CashFlowRule.CashFlowRuleLibraryId == libraryId);

            if (cashFlowRules.Any(_ => _.CashFlowDistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules.Where(_ => _.CashFlowDistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.CashFlowDistributionRules);
                _unitOfWork.CashFlowDistributionRuleRepo.UpsertOrDeleteCashFlowDistributionRules(
                    distributionRulesPerCashFlowRuleId, libraryId);
            }

            if (cashFlowRules.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var ruleCriterionJoinsToAdd = cashFlowRules
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(_ => new CriterionLibraryCashFlowRuleEntity
                    {
                        CriterionLibraryId = _.CriterionLibrary.Id,
                        CashFlowRuleId = _.Id
                    })
                    .ToList();

                _unitOfWork.Context.AddAll(ruleCriterionJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<CashFlowRuleLibraryEntity>(_ => _.Id == libraryId);
        }
    }
}
