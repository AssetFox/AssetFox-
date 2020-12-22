using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SelectableTreatmentRepository : ISelectableTreatmentRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public SelectableTreatmentRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateTreatmentLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationId}.");
            }

            var treatmentLibraryEntity = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.TreatmentLibrary.Add(treatmentLibraryEntity);

            _unitOfWork.Context.TreatmentLibrarySimulation.Add(new TreatmentLibrarySimulationEntity
            {
                TreatmentLibraryId = treatmentLibraryEntity.Id,
                SimulationId = simulationId
            });

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.TreatmentLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No treatment library found for simulation having id {simulationId}.");
            }

            var treatmentEntities = selectableTreatments
                .Select(_ => _.ToEntity(simulationEntity.TreatmentLibrarySimulationJoin.TreatmentLibraryId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.SelectableTreatment.AddRange(treatmentEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(treatmentEntities);
            }

            _unitOfWork.Context.SaveChanges();

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

                _unitOfWork.TreatmentConsequenceRepo.CreateTreatmentConsequences(consequencesPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Costs.Any())
                    .ToDictionary(_ => _.Id, _ => _.Costs.ToList());

                _unitOfWork.TreatmentCostRepo.CreateTreatmentCosts(costsPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank)))
            {
                var expressionsPerTreatmentId = selectableTreatments
                    .Where(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank))
                    .ToDictionary(_ => _.Id, _ => _.FeasibilityCriteria
                        .Where(__ => !__.ExpressionIsBlank).Select(__ => __.Expression).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinSelectableTreatmentEntitiesWithCriteria(expressionsPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Schedulings.Any()))
            {
                var schedulingsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Schedulings.Any())
                    .ToDictionary(_ => _.Id, _ => _.Schedulings.ToList());

                _unitOfWork.TreatmentSchedulingRepo.CreateTreatmentSchedulings(schedulingsPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Supersessions.Any()))
            {
                var supersessionsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Supersessions.Any())
                    .ToDictionary(_ => _.Id, _ => _.Supersessions.ToList());

                _unitOfWork.TreatmentSupersessionRepo.CreateTreatmentSupersessions(supersessionsPerTreatmentId, simulationEntity.Name);
            }
        }

        private void JoinTreatmentsWithBudgets(Dictionary<Guid, List<Guid>> budgetIdsPerTreatmentId)
        {
            var treatmentBudgetJoins = new List<SelectableTreatmentBudgetEntity>();

            budgetIdsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var budgetIds = budgetIdsPerTreatmentId[treatmentId];
                var budgetEntities = _unitOfWork.Context.Budget
                    .Where(_ => budgetIds.Contains(_.Id))
                    .ToList();

                if (!budgetEntities.Any())
                {
                    throw new RowNotInTableException("No budgets for treatments were found.");
                }

                treatmentBudgetJoins.AddRange(budgetIds.Select(budgetId => new SelectableTreatmentBudgetEntity
                {
                    SelectableTreatmentId = treatmentId, BudgetId = budgetId
                }));
            });

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.TreatmentBudget.AddRange(treatmentBudgetJoins);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(treatmentBudgetJoins);
            }

            _unitOfWork.Context.SaveChanges();
        }

        public void GetSimulationTreatments(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}.");
            }

            _unitOfWork.Context.SelectableTreatment
                .Include(_ => _.TreatmentBudgetJoins)
                .ThenInclude(_ => _.Budget)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CriterionLibrarySelectableTreatmentJoins)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentSchedulings)
                .Include(_ => _.TreatmentSupersessions)
                .Where(_ => _.TreatmentLibrary.TreatmentLibrarySimulationJoins.SingleOrDefault(__ =>
                    __.Simulation.Name == simulation.Name) != null)
                .ForEach(_ => _.CreateSelectableTreatment(simulation));
        }
    }
}
