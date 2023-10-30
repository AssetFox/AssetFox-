using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using static AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums.TreatmentEnum;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SelectableTreatmentRepository : ISelectableTreatmentRepository
    {
        public const string TreatmentLibraryNotFoundErrorMessage = "The provided treatment library was not found";
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SelectableTreatmentRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                          throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateScenarioSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            var treatmentEntities = selectableTreatments
                .Select(_ => _.ToScenarioEntity(simulationId))
                .ToList();

            _unitOfWork.Context.AddAll(treatmentEntities);

            if (selectableTreatments.Any(_ => _.Budgets.Any()))
            {
                var budgetIdsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Budgets.Any())
                    .ToDictionary(_ => _.Id, _ => _.Budgets.Select(__ => __.Id).ToList());

                JoinTreatmentsWithBudgets(budgetIdsPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = selectableTreatments
                    .Where(_ => _.Consequences.Any())
                    .ToDictionary(_ => _.Id, _ => _.Consequences.ToList());

                _unitOfWork.TreatmentConsequenceRepo.CreateScenarioConditionalTreatmentConsequences(consequencesPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Costs.Any())
                    .ToDictionary(_ => _.Id, _ => _.Costs.ToList());

                _unitOfWork.TreatmentCostRepo.CreateScenarioTreatmentCosts(costsPerTreatmentId,
                    simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank)))
            {
                var criterionJoins = new List<CriterionLibraryScenarioSelectableTreatmentEntity>();
                var criteria = selectableTreatments.Where(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank))
                    .SelectMany(_ => _.FeasibilityCriteria.Select(criterion =>
                    {
                        var entity = criterion.ToEntity($"{_.Name} Criterion");
                        entity.IsSingleUse = true;
                        criterionJoins.Add(new CriterionLibraryScenarioSelectableTreatmentEntity
                        {
                            CriterionLibraryId = entity.Id,
                            ScenarioSelectableTreatmentId = _.Id
                        });
                        return entity;
                    })).ToList();
                _unitOfWork.Context.AddAll(criteria);
                _unitOfWork.Context.AddAll(criterionJoins);
            }

            if (selectableTreatments.Any(_ => _.Schedulings.Any()))
            {
                _unitOfWork.Context.AddAll(selectableTreatments.Where(_ => _.Schedulings.Any())
                    .SelectMany(_ => _.Schedulings.Select(scheduling => scheduling.ToScenarioEntity(_.Id))).ToList());
            }

            if (selectableTreatments.Any(_ => _.SupersedeRules.Any()))
            {
                _unitOfWork.Context.AddAll(selectableTreatments.Where(_ => _.SupersedeRules.Any())
                    .SelectMany(_ => _.SupersedeRules.Select(supersedeRules => supersedeRules.ToScenarioTreatmentSupersedeRuleEntity(_.Id, simulationId))).ToList());
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        private void JoinTreatmentsWithBudgets(Dictionary<Guid, List<Guid>> budgetIdsPerTreatmentId)
        {
            var treatmentBudgetJoins = new List<ScenarioSelectableTreatmentScenarioBudgetEntity>();

            budgetIdsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var budgetIds = budgetIdsPerTreatmentId[treatmentId];
                if (!_unitOfWork.Context.ScenarioBudget.Any(_ => budgetIds.Contains(_.Id)))
                {
                    throw new RowNotInTableException("No budgets for the specified treatments were found.");
                }

                treatmentBudgetJoins.AddRange(budgetIds.Select(budgetId =>
                    new ScenarioSelectableTreatmentScenarioBudgetEntity
                    {
                        ScenarioSelectableTreatmentId = treatmentId,
                        ScenarioBudgetId = budgetId
                    }));
            });

            _unitOfWork.Context.AddAll(treatmentBudgetJoins);
        }

        public void GetScenarioSelectableTreatments(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.ScenarioSelectableTreatment.Any(_ => _.SimulationId == simulation.Id))
            {
                return;
            }

            var treatments = _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Where(_ => _.SimulationId == simulation.Id)
                .Include(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.ScenarioTreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentSchedulings)
                .Include(_ => _.ScenarioTreatmentSupersedeRules)
                .Include(_ => _.ScenarioTreatmentPerformanceFactors)
                .ToList();

            treatments.ForEach(_ => _.CreateSelectableTreatment(simulation));
        }

        public void GetScenarioSelectableTreatmentsNoChildren(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.ScenarioSelectableTreatment.Any(_ => _.SimulationId == simulation.Id))
            {
                return;
            }

            var treatments = _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Where(_ => _.SimulationId == simulation.Id)
                .ToList();

            treatments.ForEach(_ => _.CreateSelectableTreatment(simulation));
        }

        public TreatmentLibraryDTO GetSingleTreatmentLibary(Guid libraryId)
        {
            var entity = _unitOfWork.Context.TreatmentLibrary.AsNoTracking()
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .SingleOrDefault(_ => _.Id == libraryId);
            var returnValue = entity == null
                ? null : entity.ToDto();
            return returnValue;
        }

        public TreatmentLibraryDTO GetSingleTreatmentLibaryNoChildren(Guid libraryId)
        {
            var entity = _unitOfWork.Context.TreatmentLibrary.AsNoTracking()
                .SingleOrDefault(_ => _.Id == libraryId);
            var returnValue = entity == null
                ? null : entity.ToDto();
            return returnValue;
        }

        public List<TreatmentLibraryDTO> GetAllTreatmentLibraries()
        {
            if (!_unitOfWork.Context.SelectableTreatment.Any())
            {
                return new List<TreatmentLibraryDTO>();
            }

            return _unitOfWork.Context.TreatmentLibrary.AsNoTracking()
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .OrderBy(_ => _.Name)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public DateTime GetLibraryModifiedDate(Guid treatmentLibraryId)
        {
            var dtos = _unitOfWork.Context.TreatmentLibrary.Where(_ => _.Id == treatmentLibraryId).FirstOrDefault().LastModifiedDate;
            return dtos;
        }

        public List<TreatmentLibraryDTO> GetAllTreatmentLibrariesNoChildren()
        {
            if (!_unitOfWork.Context.SelectableTreatment.Any())
            {
                return new List<TreatmentLibraryDTO>();
            }

            return _unitOfWork.Context.TreatmentLibrary.AsNoTracking()
                .Include(_ => _.Treatments)
                .OrderBy(_ => _.Name)
                .Select(_ => _.ToDto())
                .ToList();
        }
        public List<TreatmentLibraryDTO> GetTreatmentLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.TreatmentLibraryUser
                .AsNoTracking()
                .Include(u => u.TreatmentLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.TreatmentLibrary.ToDto())
                .ToList();
        }

        public void UpsertTreatmentLibrary(TreatmentLibraryDTO dto) =>
        _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);


        public void AddLibraryTreatments(List<TreatmentDTO> treatments, Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified treatment library was not found.");
            }

            var selectableTreatmentEntities = treatments.Select(_ => _.ToLibraryEntity(libraryId)).ToList();

            var entityIds = selectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Where(_ => _.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();
    

            _unitOfWork.Context.AddAll(selectableTreatmentEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList());
           
            if (treatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId =
                    treatments.Where(_ => _.Costs.Any()).ToList().ToDictionary(_ => _.Id, _ => _.Costs);
                _unitOfWork.TreatmentCostRepo.UpsertOrDeleteTreatmentCosts(costsPerTreatmentId, libraryId);
            }

            if (treatments.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = treatments.Where(_ => _.Consequences.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.Consequences);
                _unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteTreatmentConsequences(consequencesPerTreatmentId,
                    libraryId);
            }

            if (treatments.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibrarySelectableTreatmentEntity>();

                var criteria = treatments
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(treatment =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibrarySelectableTreatmentEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            SelectableTreatmentId = treatment.Id
                        });
                        return criterionLibraryEntity;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }           
        }

        public void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified treatment library was not found.");
            }

            var selectableTreatmentEntities = treatments.Select(_ => _.ToLibraryEntity(libraryId)).ToList();

            var entityIds = selectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Where(_ => _.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<SelectableTreatmentEntity>(_ =>
                _.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(selectableTreatmentEntities.Where(_ => existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.AddAll(selectableTreatmentEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.TreatmentCostEquationJoin.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId ||
                _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment
                    .TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibrarySelectableTreatmentEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryTreatmentCostEntity>(_ =>
                _.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryConditionalTreatmentConsequenceEntity>(_ =>
                _.ConditionalTreatmentConsequence.SelectableTreatment.TreatmentLibraryId == libraryId);

            if (treatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId =
                    treatments.Where(_ => _.Costs.Any()).ToList().ToDictionary(_ => _.Id, _ => _.Costs);
                _unitOfWork.TreatmentCostRepo.UpsertOrDeleteTreatmentCosts(costsPerTreatmentId, libraryId);
            }

            if (treatments.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = treatments.Where(_ => _.Consequences.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.Consequences);
                _unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteTreatmentConsequences(consequencesPerTreatmentId,
                    libraryId);
            }

            if (treatments.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibrarySelectableTreatmentEntity>();

                var criteria = treatments
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(treatment =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibrarySelectableTreatmentEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            SelectableTreatmentId = treatment.Id
                        });
                        return criterionLibraryEntity;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public void ReplaceTreatmentLibrary(Guid libraryId, List<TreatmentDTO> treatments)
        {

            _unitOfWork.Context.DeleteAll<SelectableTreatmentEntity>(_ =>
                _.TreatmentLibraryId == libraryId);

            UpsertOrDeleteTreatments(treatments, libraryId);

        }

        public void DeleteTreatmentLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                    _.TreatmentCostEquationJoin.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId ||
                    _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment
                        .TreatmentLibraryId == libraryId);

                _unitOfWork.Context.DeleteEntity<TreatmentLibraryEntity>(_ => _.Id == libraryId);
            });
        }

        public List<TreatmentDTO> GetScenarioSelectableTreatments(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario");
            }

            return _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentPerformanceFactors)
                .Include(_=>_.ScenarioTreatmentSupersedeRules)
                .ThenInclude(_=>_.CriterionLibraryScenarioTreatmentSupersedeRuleJoin)
                .ThenInclude(_=>_.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<string> GetSelectableTreatmentNames(Guid libraryId)
        {

            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException(TreatmentLibraryNotFoundErrorMessage);
            }
            var names = _unitOfWork.Context.SelectableTreatment
                .Where(t => t.TreatmentLibraryId == libraryId)
                .Select(t => t.Name)
                .OrderBy(s => s)
                .ToList();
            return names;
        }

        public List<TreatmentDTO> GetSelectableTreatments(Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException(TreatmentLibraryNotFoundErrorMessage);
            }

            return _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_=>_.TreatmentSupersedeRules)
                .ThenInclude(_=>_.CriterionLibraryTreatmentSupersedeRuleJoin)
                .ThenInclude(_=>_.CriterionLibrary)
                .Where(_ => _.TreatmentLibraryId == libraryId)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void AddScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatments,
           Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var scenarioSelectableTreatmentEntities =
                scenarioSelectableTreatments.Select(_ => _.ToScenarioEntity(simulationId)).ToList();
            var entityIds = scenarioSelectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();
            _unitOfWork.AsTransaction(() =>
            {
                
                _unitOfWork.Context.AddAll(scenarioSelectableTreatmentEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                    .ToList());

             
                if (scenarioSelectableTreatments.Any(_ => _.Costs.Any()))
                {
                    var costsPerTreatmentId =
                        scenarioSelectableTreatments.Where(_ => _.Costs.Any()).ToList()
                            .ToDictionary(_ => _.Id, _ => _.Costs);
                    _unitOfWork.TreatmentCostRepo.UpsertOrDeleteScenarioTreatmentCosts(costsPerTreatmentId, simulationId);
                }

                if (scenarioSelectableTreatments.Any(_ => _.Consequences.Any()))
                {
                    var consequencesPerTreatmentId = scenarioSelectableTreatments.Where(_ => _.Consequences.Any()).ToList()
                        .ToDictionary(_ => _.Id, _ => _.Consequences);
                    _unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteScenarioTreatmentConsequences(
                        consequencesPerTreatmentId, simulationId);
                }
                if (scenarioSelectableTreatments.Any(_ => _.PerformanceFactors.Any()))
                {
                    var performancePerTreatmentId = scenarioSelectableTreatments.Where(_ => _.PerformanceFactors.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.PerformanceFactors);
                    _unitOfWork.TreatmentPerformanceFactorRepo.UpsertScenarioTreatmentPerformanceFactors(performancePerTreatmentId, simulationId);
                }
                if (scenarioSelectableTreatments.Any(_ => _.BudgetIds.Any()))
                {
                    var treatmentBudgetJoinsToAdd = scenarioSelectableTreatments.Where(_ => _.BudgetIds.Any()).SelectMany(
                        _ =>
                            _.BudgetIds.Select(budgetId =>
                                new ScenarioSelectableTreatmentScenarioBudgetEntity
                                {
                                    ScenarioSelectableTreatmentId = _.Id,
                                    ScenarioBudgetId = budgetId
                                })).ToList();

                    _unitOfWork.Context.AddAll(treatmentBudgetJoinsToAdd, _unitOfWork.UserEntity?.Id);
                }

                if (scenarioSelectableTreatments.Any(_ =>
                    _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
                {
                    var criterionJoins = new List<CriterionLibraryScenarioSelectableTreatmentEntity>();

                    var criteria = scenarioSelectableTreatments
                        .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                        .Select(treatment =>
                        {
                            var criterionLibraryEntity = new CriterionLibraryEntity
                            {
                                Id = Guid.NewGuid(),
                                MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                                Name = $"{treatment.Name} Criterion",
                                IsSingleUse = true
                            };
                            criterionJoins.Add(new CriterionLibraryScenarioSelectableTreatmentEntity
                            {
                                CriterionLibraryId = criterionLibraryEntity.Id,
                                ScenarioSelectableTreatmentId = treatment.Id
                            });
                            return criterionLibraryEntity;
                        }).ToList();

                    _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                    _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
                }

                // TODO should SupersedeRules be taken care here? Guessing it can be done at Import API work item.

                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
                _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
            });
        }

        public void UpsertOrDeleteScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatments,
            Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var scenarioSelectableTreatmentEntities =
                scenarioSelectableTreatments.Select(_ => _.ToScenarioEntity(simulationId)).ToList();
            var entityIds = scenarioSelectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();
            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentScenarioBudgetEntity>(_ =>
                    _.ScenarioSelectableTreatment.SimulationId == simulationId);

                _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentEntity>(_ =>
                    _.SimulationId == simulationId && !entityIds.Contains(_.Id));

                _unitOfWork.Context.UpdateAll(scenarioSelectableTreatmentEntities
                    .Where(_ => existingEntityIds.Contains(_.Id))
                    .ToList());
                _unitOfWork.Context.AddAll(scenarioSelectableTreatmentEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                    .ToList());

                _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                    _.ScenarioTreatmentCostEquationJoin.ScenarioTreatmentCost.ScenarioSelectableTreatment.SimulationId ==
                    simulationId ||
                    _.ScenarioConditionalTreatmentConsequenceEquationJoin.ScenarioConditionalTreatmentConsequence
                        .ScenarioSelectableTreatment
                        .SimulationId == simulationId);

                _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentScenarioBudgetEntity>(_ =>
                    _.ScenarioSelectableTreatment.SimulationId == simulationId);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioSelectableTreatmentEntity>(_ =>
                    _.ScenarioSelectableTreatment.SimulationId == simulationId);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioTreatmentCostEntity>(_ =>
                    _.ScenarioTreatmentCost.ScenarioSelectableTreatment.SimulationId == simulationId);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>(_ =>
                    _.ScenarioConditionalTreatmentConsequence.ScenarioSelectableTreatment.SimulationId == simulationId);

                if (scenarioSelectableTreatments.Any(_ => _.Costs.Any()))
                {
                    var costsPerTreatmentId =
                        scenarioSelectableTreatments.Where(_ => _.Costs.Any()).ToList()
                            .ToDictionary(_ => _.Id, _ => _.Costs);
                    _unitOfWork.TreatmentCostRepo.UpsertOrDeleteScenarioTreatmentCosts(costsPerTreatmentId, simulationId);
                }

                if (scenarioSelectableTreatments.Any(_ => _.Consequences.Any()))
                {
                    var consequencesPerTreatmentId = scenarioSelectableTreatments.Where(_ => _.Consequences.Any()).ToList()
                        .ToDictionary(_ => _.Id, _ => _.Consequences);
                    _unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteScenarioTreatmentConsequences(
                        consequencesPerTreatmentId, simulationId);
                }
                if (scenarioSelectableTreatments.Any(_ => _.PerformanceFactors.Any()))
                {
                    var performancePerTreatmentId = scenarioSelectableTreatments.Where(_ => _.PerformanceFactors.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.PerformanceFactors);
                    _unitOfWork.TreatmentPerformanceFactorRepo.UpsertScenarioTreatmentPerformanceFactors(performancePerTreatmentId, simulationId);
                }
                if (scenarioSelectableTreatments.Any(_ => _.BudgetIds.Any()))
                {
                    var treatmentBudgetJoinsToAdd = scenarioSelectableTreatments.Where(_ => _.BudgetIds.Any()).SelectMany(
                        _ =>
                            _.BudgetIds.Select(budgetId =>
                                new ScenarioSelectableTreatmentScenarioBudgetEntity
                                {
                                    ScenarioSelectableTreatmentId = _.Id,
                                    ScenarioBudgetId = budgetId
                                })).ToList();

                    _unitOfWork.Context.AddAll(treatmentBudgetJoinsToAdd, _unitOfWork.UserEntity?.Id);
                }

                if (scenarioSelectableTreatments.Any(_ =>
                    _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
                {
                    var criterionJoins = new List<CriterionLibraryScenarioSelectableTreatmentEntity>();

                    var criteria = scenarioSelectableTreatments
                        .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                        .Select(treatment =>
                        {
                            var criterionLibraryEntity = new CriterionLibraryEntity
                            {
                                Id = Guid.NewGuid(),
                                MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                                Name = $"{treatment.Name} Criterion",
                                IsSingleUse = true
                            };
                            criterionJoins.Add(new CriterionLibraryScenarioSelectableTreatmentEntity
                            {
                                CriterionLibraryId = criterionLibraryEntity.Id,
                                ScenarioSelectableTreatmentId = treatment.Id
                            });
                            return criterionLibraryEntity;
                        }).ToList();

                    _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                    _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
                }

                if (scenarioSelectableTreatments.Any(_ => _.SupersedeRules != null && _.SupersedeRules.Any()))
                {
                    var supersedeRulesPerTreatmentId = scenarioSelectableTreatments.Where(_ => _.SupersedeRules.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.SupersedeRules);
                    _unitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(supersedeRulesPerTreatmentId, simulationId);
                }

                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
                _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
            });
        }

        public void DeleteTreatment(TreatmentDTO treatment, Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified treatment library was not found.");
            }

            var selectableTreatmentEntity = treatment.ToLibraryEntity(libraryId);
            var entityId = selectableTreatmentEntity.Id;
            _unitOfWork.AsTransaction(() =>
            {

                _unitOfWork.Context.DeleteAll<SelectableTreatmentEntity>(_ =>
                    _.TreatmentLibraryId == libraryId && entityId == _.Id);

                _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                    (_.TreatmentCostEquationJoin.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId ||
                        _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment
                        .TreatmentLibraryId == libraryId) && _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment.Id == treatment.Id);

                _unitOfWork.Context.DeleteAll<CriterionLibrarySelectableTreatmentEntity>(_ =>
                    _.SelectableTreatment.TreatmentLibraryId == libraryId && _.SelectableTreatment.Id == treatment.Id);

                _unitOfWork.Context.DeleteAll<CriterionLibraryTreatmentCostEntity>(_ =>
                    _.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId && _.TreatmentCost.SelectableTreatment.Id == treatment.Id);

                _unitOfWork.Context.DeleteAll<CriterionLibraryConditionalTreatmentConsequenceEntity>(_ =>
                    _.ConditionalTreatmentConsequence.SelectableTreatment.TreatmentLibraryId == libraryId && _.ConditionalTreatmentConsequence.SelectableTreatment.Id == treatment.Id);
            });
        }

        public void DeleteScenarioSelectableTreatment(TreatmentDTO scenarioSelectableTreatment,
            Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var entityId = scenarioSelectableTreatment.Id;

            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentScenarioBudgetEntity>(_ =>
                    _.ScenarioSelectableTreatment.SimulationId == simulationId && _.ScenarioSelectableTreatment.Id == entityId);

                _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentEntity>(_ =>
                    _.SimulationId == simulationId && _.Id == entityId);

                _unitOfWork.Context.DeleteAll<EquationEntity>(_ => (_.ScenarioTreatmentCostEquationJoin.ScenarioTreatmentCost.ScenarioSelectableTreatment.SimulationId == simulationId
                        && _.ScenarioTreatmentCostEquationJoin.ScenarioTreatmentCost.ScenarioSelectableTreatment.Id == entityId)
                    ||
                    (_.ScenarioConditionalTreatmentConsequenceEquationJoin.ScenarioConditionalTreatmentConsequence
                        .ScenarioSelectableTreatment.SimulationId == simulationId
                        && _.ScenarioConditionalTreatmentConsequenceEquationJoin.ScenarioConditionalTreatmentConsequence
                        .ScenarioSelectableTreatment.Id == entityId));

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioSelectableTreatmentEntity>(_ =>
                    _.ScenarioSelectableTreatment.SimulationId == simulationId && _.ScenarioSelectableTreatment.Id == entityId);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioTreatmentCostEntity>(_ =>
                    _.ScenarioTreatmentCost.ScenarioSelectableTreatment.SimulationId == simulationId
                    && _.ScenarioTreatmentCost.ScenarioSelectableTreatment.Id == entityId);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>(_ =>
                    _.ScenarioConditionalTreatmentConsequence.ScenarioSelectableTreatment.SimulationId == simulationId
                    && _.ScenarioConditionalTreatmentConsequence.ScenarioSelectableTreatment.Id == entityId);

                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
                _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
            });
        }

        public List<SimpleTreatmentDTO> GetSimpleTreatmentsBySimulationId(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario");
            }

            return _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId)
                .Select(_ => new SimpleTreatmentDTO()
                {
                    Id = _.Id,
                    Name = _.Name
                })
                .OrderBy(_ => _.Name)
                .ToList();
        }

        public List<SimpleTreatmentDTO> GetSimpleTreatmentsByLibraryId(Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException(TreatmentLibraryNotFoundErrorMessage);
            }

            return _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Where(_ => _.TreatmentLibraryId == libraryId)
                .Select(_ => new SimpleTreatmentDTO()
                {
                    Id = _.Id,
                    Name = _.Name
                })
                .OrderBy(_ => _.Name)
                .ToList();
        }

        public TreatmentDTOWithSimulationId GetScenarioSelectableTreatmentById(Guid id)
        {
            return _unitOfWork.Context.ScenarioSelectableTreatment.AsNoTracking()
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentPerformanceFactors)
                .Single(_ => _.Id == id)
                .ToDtoWithSimulationId();
        }

        public TreatmentDTO GetSelectableTreatmentById(Guid id)
        {
            return _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.Id == id)
                .ToDto();
        }

        public TreatmentLibraryDTO GetTreatmentLibraryWithSingleTreatmentByTreatmentId(Guid treatmentId)
        {
            var entity = _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentConsequences.OrderBy(__ => __.Attribute.Name))
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentLibrary)
                .Single(_ => _.Id == treatmentId);
            return entity.TreatmentLibrary.ToDto();
        }
        public TreatmentDTO GetDefaultNoTreatment(Guid simulationId)
        {
            try
            {
                var scenarioSelectableTreatments = GetScenarioSelectableTreatments(simulationId);
                var defaultTreatment = scenarioSelectableTreatments.SingleOrDefault(_ => _.CriterionLibrary == null || string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression));
                if (defaultTreatment == null)
                    return null;
                return defaultTreatment;
            }
            catch (InvalidOperationException)
            {
                var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(simulationId);
                throw new InvalidOperationException("More than one default treatments found in scenario" + simulationName);
            }
        }
        public ScenarioSelectableTreatmentEntity GetDefaultTreatment(Guid simulationId)
        {
            try
            {
                var scenarioSelectableTreatments = GetScenarioSelectableTreatments(simulationId);
                var defaultTreatment = scenarioSelectableTreatments.SingleOrDefault(_ => _.CriterionLibrary == null || string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression));
                if (defaultTreatment == null)
                    return null;
                return _unitOfWork.Context.ScenarioSelectableTreatment.FirstOrDefault(_ => _.Id == defaultTreatment.Id);
            }
            catch (InvalidOperationException)
            {
                var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(simulationId);
                throw new InvalidOperationException("More than one default treatments found in scenario" + simulationName);
            }
        }

        public TreatmentDTO GetSelectableTreatmentByLibraryIdAndName(Guid treatmentLibraryId, string treatmentName)
        {
            var dbSet = _unitOfWork.Context.SelectableTreatment;
            var entity = dbSet
                        .FirstOrDefault(_ => _.Name == treatmentName && _.TreatmentLibraryId == treatmentLibraryId);
            var dto = entity.ToDtoNullSafe();
            return dto;
        }

        public void AddLibraryIdToScenarioSelectableTreatments(List<TreatmentDTO> treatmentDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in treatmentDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }

        public void AddModifiedToScenarioSelectableTreatments(List<TreatmentDTO> treatmentDTOs, bool IsModified)
        {
            foreach (var dto in treatmentDTOs)
            {
                dto.IsModified = IsModified;
            }
        }

        public void AddDefaultPerformanceFactors(Guid scenarioId, List<TreatmentDTO> treatments)
        {
            var distinctPerformanceCurves = _unitOfWork.Context.ScenarioPerformanceCurve.Include(_ => _.Attribute).Where(_ => _.SimulationId == scenarioId).ToList().GroupBy(_ => _.Attribute.Name).Select(_ => _.First().Attribute.Name).ToList();
            if (distinctPerformanceCurves.Count > 0)
            {
                treatments.ForEach(_ =>
                {
                    var factorsToBeRemoved = _.PerformanceFactors.Where(p => !distinctPerformanceCurves.Contains(p.Attribute)).Select(__ => __.Attribute).ToList();
                    var factorsToBeAdded = distinctPerformanceCurves.Where(dpc => _.PerformanceFactors.FirstOrDefault(__ => __.Attribute == dpc) == null).ToList();
                    if(factorsToBeAdded.Count > 0)
                        _.PerformanceFactors =  _.PerformanceFactors.Concat(factorsToBeAdded.Select(__ => new TreatmentPerformanceFactorDTO() { Attribute = __, Id = Guid.NewGuid(), PerformanceFactor = 1 })).ToList();
                    if(factorsToBeRemoved.Count > 0)
                        _.PerformanceFactors.RemoveAll(__ => factorsToBeRemoved.Contains(__.Attribute));                  
                });
            }
        }

        public void UpsertOrDeleteTreatmentLibraryTreatmentsAndPossiblyUsers(TreatmentLibraryDTO dto, bool isNewLibrary, Guid userId)
        {
            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
                _unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
                if (isNewLibrary)
                {
                    var users = LibraryUserDtolists.OwnerAccess(userId);
                    _unitOfWork.TreatmentLibraryUserRepo.UpsertOrDeleteUsers(dto.Id, users);
                }
            });
        }
    }
}
