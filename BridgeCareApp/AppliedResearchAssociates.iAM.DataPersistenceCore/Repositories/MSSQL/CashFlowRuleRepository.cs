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
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowRuleRepository : ICashFlowRuleRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public CashFlowRuleRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateCashFlowRuleLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
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
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.CashFlowRuleLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No cash flow rule library found for simulation having id {simulationId}.");
            }

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(simulationEntity.CashFlowRuleLibrarySimulationJoin.CashFlowRuleLibraryId))
                .ToList();

            _unitOfWork.Context.AddAll(cashFlowRuleEntities, _unitOfWork.UserEntity?.Id);

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

        public Task<List<CashFlowRuleLibraryDTO>> CashFlowRuleLibrariesWithCashFlowRules()
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<CashFlowRuleLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.CashFlowRuleLibrary
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CashFlowDistributionRules)
                .Include(_ => _.CashFlowRuleLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, CashFlowRuleLibraryDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }
            }

            UpsertCashFlowRuleLibrary(dto, simulationId, userInfo);
            UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id, userInfo);
        }

        public void UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var cashFlowRuleLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(cashFlowRuleLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<CashFlowRuleLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(new CashFlowRuleLibrarySimulationEntity
                {
                    CashFlowRuleLibraryId = cashFlowRuleLibraryEntity.Id,
                    SimulationId = simulationId
                }, userEntity?.Id);
            }

            _unitOfWork.Context.SaveChanges();
        }

        public void UpsertOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No cash flow rule library found having id {libraryId}.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToEntity(libraryId))
                .ToList();

            var entityIds = cashFlowRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.CashFlowRule
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
                _unitOfWork.Context.UpsertOrDelete(cashFlowRuleEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(cashFlowRuleEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

            _unitOfWork.Context.DeleteAll<CriterionLibraryCashFlowRuleEntity>(_ =>
                _.CashFlowRule.CashFlowRuleLibraryId == libraryId);

            if (cashFlowRules.Any(_ => _.CashFlowDistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules.Where(_ => _.CashFlowDistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.CashFlowDistributionRules);
                _unitOfWork.CashFlowDistributionRuleRepo.UpsertOrDeleteCashFlowDistributionRules(distributionRulesPerCashFlowRuleId, libraryId);
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

                _unitOfWork.Context.AddAll(ruleCriterionJoinsToAdd, userEntity?.Id);
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
