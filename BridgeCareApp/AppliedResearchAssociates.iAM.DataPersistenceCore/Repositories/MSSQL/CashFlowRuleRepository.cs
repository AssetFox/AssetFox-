using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowRuleRepository : ICashFlowRuleRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public CashFlowRuleRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateCashFlowRuleLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var cashFlowRuleLibraryEntity = new CashFlowRuleLibraryEntity {Id = Guid.NewGuid(), Name = name};

            _unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Add(cashFlowRuleLibraryEntity);

            _unitOfDataPersistenceWork.Context.CashFlowRuleLibrarySimulation.Add(new CashFlowRuleLibrarySimulationEntity
            {
                CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.CashFlowRuleLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No cash flow rule library found for simulation having id {simulationId}.");
            }

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(simulationEntity.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.CashFlowRule.AddRange(cashFlowRuleEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(cashFlowRuleEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (cashFlowRules.Any(_ => _.DistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules
                    .Where(_ => _.DistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.DistributionRules.ToList());

                _unitOfDataPersistenceWork.CashFlowDistributionRuleRepo.CreateCashFlowDistributionRules(distributionRulesPerCashFlowRuleId);
            }

            if (cashFlowRules.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var cashFlowRuleEntityIdsPerExpression = cashFlowRules
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _.Id)
                    .ToDictionary(_ => _.Key, _ => _.ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(
                    cashFlowRuleEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.CashFlowRule, simulationEntity.Name);
            }
        }

        public Task<List<CashFlowRuleLibraryDTO>> CashFlowRuleLibrariesWithCashFlowRules()
        {
            if (!_unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<CashFlowRuleLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.CashFlowRuleLibrary
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CashFlowDistributionRules)
                .Include(_ => _.CashFlowRuleLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void AddOrUpdateCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto, Guid simulationId)
        {
            var cashFlowRuleLibraryEntity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.AddOrUpdate(cashFlowRuleLibraryEntity, dto.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<CashFlowRuleLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.CashFlowRuleLibrarySimulation.Add(new CashFlowRuleLibrarySimulationEntity
                {
                    CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id,
                    SimulationId = simulationId
                });
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No cash flow rule library found having id {libraryId}.");
            }

            var entities = cashFlowRules
                .Select(_ => _.ToEntity(libraryId))
                .ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.CashFlowRule
                .Where(_ => _.CashFlowRuleLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<CashFlowRuleEntity, bool>>>
            {
                {"delete", _ => _.CashFlowRuleLibrary.Id == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryCashFlowRuleEntity>(_ =>
                _.CashFlowRule.CashFlowRuleLibraryId == libraryId);

            if (cashFlowRules.Any(_ => _.CashFlowDistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules.Where(_ => _.CashFlowDistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.CashFlowDistributionRules);
                _unitOfDataPersistenceWork.CashFlowDistributionRuleRepo.AddOrUpdateOrDeleteCashFlowDistributionRules(distributionRulesPerCashFlowRuleId, libraryId);
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

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryCashFlowRule.AddRange(ruleCriterionJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(ruleCriterionJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete = _unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Single(_ => _.Id == libraryId);

            // deleting the library should start a cascade delete of the cash flow rules which will cascade into the criterion library joins
            // and the cash flow distribution rules
            _unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.Remove(libraryToDelete);

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
