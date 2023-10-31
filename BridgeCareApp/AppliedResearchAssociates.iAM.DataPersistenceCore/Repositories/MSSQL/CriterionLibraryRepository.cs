using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CriterionLibraryRepository : ICriterionLibraryRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CriterionLibraryRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void JoinEntitiesWithCriteria(Dictionary<string, List<Guid>> entityIdsPerExpression, string joinEntity, string prependName) =>
            entityIdsPerExpression.Keys.ForEach(expression =>
            {
                var criterionLibraryEntity = _unitOfWork.Context.CriterionLibrary
                    .SingleOrDefault(_ => _.MergedCriteriaExpression == expression &&
                                          _.Name.Contains(DataPersistenceConstants.CriterionLibraryJoinEntities.NameConventionPerEntityType[joinEntity]));

                if (criterionLibraryEntity == null)
                {
                    var criterionLibraryNames = _unitOfWork.Context.CriterionLibrary
                        .Where(_ => _.Name.Contains(DataPersistenceConstants.CriterionLibraryJoinEntities.NameConventionPerEntityType[joinEntity]))
                        .Select(_ => _.Name).ToList();

                    var newCriterionLibraryName = $"{prependName} {DataPersistenceConstants.CriterionLibraryJoinEntities.NameConventionPerEntityType[joinEntity]} Criterion Library";
                    if (criterionLibraryNames.Contains(newCriterionLibraryName))
                    {
                        var version = 2;
                        while (criterionLibraryNames.Contains($"{newCriterionLibraryName} v{version}"))
                        {
                            version++;
                        }
                        newCriterionLibraryName = $"{newCriterionLibraryName} v{version}";
                    }

                    criterionLibraryEntity = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = newCriterionLibraryName,
                        MergedCriteriaExpression = expression
                    };

                    _unitOfWork.Context.AddEntity(criterionLibraryEntity, _unitOfWork.UserEntity?.Id);
                }

                switch (joinEntity)
                {
                case DataPersistenceConstants.CriterionLibraryJoinEntities.AnalysisMethod:
                    CreateCriterionLibraryAnalysisMethodJoin(criterionLibraryEntity.Id, entityIdsPerExpression[expression].First());
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.Budget:
                    CreateCriterionLibraryBudgetJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.BudgetPriority:
                    CreateCriterionLibraryBudgetPriorityJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.CashFlowRule:
                    CreateCriterionLibraryCashFlowRuleJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.DeficientConditionGoal:
                    CreateCriterionLibraryDeficientConditionGoalJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.PerformanceCurve:
                    CreateCriterionLibraryPerformanceCurveJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.RemainingLifeLimit:
                    CreateCriterionLibraryRemainingLifeLimitJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.TargetConditionGoal:
                    CreateCriterionLibraryTargetConditionGoalJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.ConditionalTreatmentConsequence:
                    CreateCriterionLibraryTreatmentConsequenceJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.TreatmentCost:
                    CreateCriterionLibraryTreatmentCostJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.TreatmentSupersedeRule:
                    CreateCriterionLibraryTreatmentSupersedeRuleJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                case DataPersistenceConstants.CriterionLibraryJoinEntities.SelectableTreatment:
                    CreateCriterionLibrarySelectableTreatmentJoins(criterionLibraryEntity.Id, entityIdsPerExpression[expression]);
                    break;

                default:
                    throw new InvalidOperationException("Unable to determine criterion library join entity type.");
                }
            });

        // [Obselete] - Not getting used anywhere
        public void UpsertCriterionLibraries(List<CriterionLibraryEntity> criterionLibraryEntities)
        {
            var existingEntityIds = _unitOfWork.Context.CriterionLibrary.Select(_ => _.Id).ToList();

            _unitOfWork.Context.UpdateAll(
                criterionLibraryEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                criterionLibraryEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryAnalysisMethodJoin(Guid criterionLibraryId, Guid analysisMethodId)
        {
            var joinEntity = new CriterionLibraryAnalysisMethodEntity
            {
                CriterionLibraryId = criterionLibraryId,
                AnalysisMethodId = analysisMethodId
            };

            _unitOfWork.Context.AddEntity(joinEntity, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryBudgetJoins(Guid criterionLibraryId, List<Guid> budgetIds)
        {
            var joinEntities = budgetIds.Select(budgetId =>
                    new CriterionLibraryBudgetEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        BudgetId = budgetId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryBudgetPriorityJoins(Guid criterionLibraryId, List<Guid> budgetPriorityIds)
        {
            var joinEntities = budgetPriorityIds.Select(budgetPriorityId =>
                    new CriterionLibraryBudgetPriorityEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        BudgetPriorityId = budgetPriorityId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryCashFlowRuleJoins(Guid criterionLibraryId, List<Guid> cashFlowRuleIds)
        {
            var joinEntities = cashFlowRuleIds.Select(cashFlowRuleId =>
                    new CriterionLibraryCashFlowRuleEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        CashFlowRuleId = cashFlowRuleId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryDeficientConditionGoalJoins(Guid criterionLibraryId, List<Guid> deficientConditionGoalIds)
        {
            var joinEntities = deficientConditionGoalIds.Select(deficientConditionGoalId =>
                    new CriterionLibraryDeficientConditionGoalEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        DeficientConditionGoalId = deficientConditionGoalId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryPerformanceCurveJoins(Guid criterionLibraryId, List<Guid> performanceCurveIds)
        {
            var joinEntities = performanceCurveIds.Select(performanceCurveId =>
                new CriterionLibraryPerformanceCurveEntity
                {
                    CriterionLibraryId = criterionLibraryId,
                    PerformanceCurveId = performanceCurveId
                })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryRemainingLifeLimitJoins(Guid criterionLibraryId, List<Guid> remainingLifeLimitIds)
        {
            var joinEntities = remainingLifeLimitIds.Select(remainingLifeLimitId =>
                    new CriterionLibraryRemainingLifeLimitEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        RemainingLifeLimitId = remainingLifeLimitId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryTargetConditionGoalJoins(Guid criterionLibraryId, List<Guid> targetConditionGoalIds)
        {
            var joinEntities = targetConditionGoalIds.Select(targetConditionGoalId =>
                    new CriterionLibraryTargetConditionGoalEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TargetConditionGoalId = targetConditionGoalId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryTreatmentConsequenceJoins(Guid criterionLibraryId, List<Guid> consequenceIds)
        {
            var joinEntities = consequenceIds.Select(consequenceId =>
                    new CriterionLibraryConditionalTreatmentConsequenceEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        ConditionalTreatmentConsequenceId = consequenceId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryTreatmentCostJoins(Guid criterionLibraryId, List<Guid> costIds)
        {
            var joinEntities = costIds.Select(costId =>
                    new CriterionLibraryTreatmentCostEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TreatmentCostId = costId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibraryTreatmentSupersedeRuleJoins(Guid criterionLibraryId, List<Guid> supersedeRuleIds)
        {
            var joinEntities = supersedeRuleIds.Select(supersedeRuleId =>
                    new CriterionLibraryTreatmentSupersedeRuleEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        TreatmentSupersedeRuleId = supersedeRuleId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void CreateCriterionLibrarySelectableTreatmentJoins(Guid criterionLibraryId, List<Guid> treatmentIds)
        {
            var joinEntities = treatmentIds.Select(treatmentId =>
                    new CriterionLibrarySelectableTreatmentEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        SelectableTreatmentId = treatmentId
                    })
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        // [Obselete] - Not getting used anywhere
        public void JoinSelectableTreatmentEntitiesWithCriteria(
            Dictionary<Guid, List<string>> expressionsPerSelectableTreatmentEntityId, string simulationName)
        {
            var criterionLibraryEntities = new List<CriterionLibraryEntity>();
            var criterionLibraryIdsPerSelectableTreatmentId = new Dictionary<Guid, List<Guid>>();

            expressionsPerSelectableTreatmentEntityId.Keys.ForEach(entityId =>
            {
                var criterionLibraryEntityIds = new List<Guid>();

                expressionsPerSelectableTreatmentEntityId[entityId].ForEach(expression =>
                {
                    var criterionLibraryEntity = _unitOfWork.Context.CriterionLibrary
                        .SingleOrDefault(_ => _.MergedCriteriaExpression == expression &&
                                              _.Name.Contains(
                                                  DataPersistenceConstants.CriterionLibraryJoinEntities
                                                      .NameConventionPerEntityType[
                                                          DataPersistenceConstants.CriterionLibraryJoinEntities
                                                              .SelectableTreatment]));

                    if (criterionLibraryEntity == null)
                    {
                        var criterionLibraryNames = _unitOfWork.Context.CriterionLibrary
                            .Where(_ => _.Name.Contains(
                                DataPersistenceConstants.CriterionLibraryJoinEntities.NameConventionPerEntityType[
                                    DataPersistenceConstants.CriterionLibraryJoinEntities.SelectableTreatment]))
                            .Select(_ => _.Name).ToList();

                        var newCriterionLibraryName =
                            $"{simulationName} Simulation {DataPersistenceConstants.CriterionLibraryJoinEntities.NameConventionPerEntityType[DataPersistenceConstants.CriterionLibraryJoinEntities.SelectableTreatment]} Criterion Library";
                        if (criterionLibraryNames.Contains(newCriterionLibraryName))
                        {
                            var version = 2;
                            while (criterionLibraryNames.Contains($"{newCriterionLibraryName} v{version}"))
                            {
                                version++;
                            }
                            newCriterionLibraryName = $"{newCriterionLibraryName} v{version}";
                        }

                        criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            Name = newCriterionLibraryName,
                            MergedCriteriaExpression = expression
                        };

                        criterionLibraryEntities.Add(criterionLibraryEntity);
                    }

                    criterionLibraryEntityIds.Add(criterionLibraryEntity.Id);
                });

                criterionLibraryIdsPerSelectableTreatmentId.Add(entityId, criterionLibraryEntityIds);
            });

            if (criterionLibraryEntities.Any())
            {
                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
            }

            CreateCriterionLibrarySelectableTreatmentJoins(criterionLibraryIdsPerSelectableTreatmentId);
        }

        private void CreateCriterionLibrarySelectableTreatmentJoins(Dictionary<Guid, List<Guid>> criterionLibraryIdsPerSelectableTreatmentIds)
        {
            var joinEntities = criterionLibraryIdsPerSelectableTreatmentIds
                .SelectMany(_ => _.Value
                    .Select(criterionLibraryId => new CriterionLibrarySelectableTreatmentEntity
                    {
                        CriterionLibraryId = criterionLibraryId,
                        SelectableTreatmentId = _.Key
                    })
                )
                .ToList();

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        public Task<List<CriterionLibraryDTO>> CriterionLibraries()
        {
            if (!_unitOfWork.Context.CriterionLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<CriterionLibraryDTO>());
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.CriterionLibrary.Where(_ => _.IsSingleUse == false).Select(_ => _.ToDto()).ToList());
        }

        public Task<CriterionLibraryDTO> CriteriaLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.CriterionLibrary.Any(_ => _.Id == libraryId))
            {
                return Task.Factory.StartNew(() => new CriterionLibraryDTO());
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.CriterionLibrary.First(_ => _.Id == libraryId).ToDto());
        }

        public Guid UpsertCriterionLibrary(CriterionLibraryDTO dto)
        {
            var entity = _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);

            return entity.Id;
        }

        public void DeleteCriterionLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.CriterionLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<CriterionLibraryEntity>(_ => _.Id == libraryId);
        }

        public void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation(Guid simulationId, List<string> budgetNames)
        {
            _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                _.IsSingleUse && _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                    join.ScenarioBudget.SimulationId == simulationId &&
                    budgetNames.Contains(join.ScenarioBudget.Name)));
        }

        public void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary(Guid budgetLibraryId, List<string> budgetNames)
        {
            _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                _.IsSingleUse && _.CriterionLibraryBudgetJoins.Any(join =>
                    join.Budget.BudgetLibraryId == budgetLibraryId &&
                    budgetNames.Contains(join.Budget.Name)));
        }

        public void AddLibraries(List<CriterionLibraryDTO> criteria)
        {
            var entities = criteria.Select(c => c.ToEntity()).ToList();
            _unitOfWork.Context.AddAll(entities, _unitOfWork.CurrentUser?.Id);
        }

        public void AddLibraryScenarioBudgetJoins(List<CriterionLibraryScenarioBudgetDTO> criteriaJoins)
        {
            var entities = criteriaJoins.Select(c => c.ToEntity()).ToList();
            _unitOfWork.Context.AddAll(entities, _unitOfWork.CurrentUser?.Id);
        }
        public void AddLibraryBudgetJoins(List<CriterionLibraryBudgetDTO> criteriaJoins)
        {
            var entities = criteriaJoins.Select(c => c.ToEntity()).ToList();
            _unitOfWork.Context.AddAll(entities, _unitOfWork.CurrentUser?.Id);
        }
    }
}
