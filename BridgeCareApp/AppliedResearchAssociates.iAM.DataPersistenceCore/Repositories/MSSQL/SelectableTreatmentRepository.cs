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

        private readonly ITreatmentConsequenceRepository _treatmentConsequenceRepo;
        private readonly ITreatmentCostRepository _treatmentCostRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly ITreatmentSchedulingRepository _treatmentSchedulingRepo;
        private readonly ITreatmentSupersessionRepository _treatmentSupersessionRepo;
        private readonly IAMContext _context;

        public SelectableTreatmentRepository(ITreatmentConsequenceRepository treatmentConsequenceRepo,
            ITreatmentCostRepository treatmentCostRepo,
            ICriterionLibraryRepository criterionLibraryRepo,
            ITreatmentSchedulingRepository treatmentSchedulingRepo,
            ITreatmentSupersessionRepository treatmentSupersessionRepo,
            IAMContext context)
        {
            _treatmentConsequenceRepo = treatmentConsequenceRepo ??
                                        throw new ArgumentNullException(nameof(treatmentConsequenceRepo));
            _treatmentCostRepo = treatmentCostRepo ?? throw new ArgumentNullException(nameof(treatmentCostRepo));
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _treatmentSchedulingRepo = treatmentSchedulingRepo ??
                                       throw new ArgumentNullException(nameof(treatmentSchedulingRepo));
            _treatmentSupersessionRepo = treatmentSupersessionRepo ??
                                         throw new ArgumentNullException(nameof(treatmentSupersessionRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateTreatmentLibrary(string name, Guid simulationId)
        {
            using (var contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!_context.Simulation.Any(_ => _.Id == simulationId))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationId}.");
                    }

                    var treatmentLibraryEntity = new TreatmentLibraryEntity {Id = Guid.NewGuid(), Name = name};

                    _context.TreatmentLibrary.Add(treatmentLibraryEntity);

                    _context.TreatmentLibrarySimulation.Add(new TreatmentLibrarySimulationEntity
                    {
                        TreatmentLibraryId = treatmentLibraryEntity.Id, SimulationId = simulationId
                    });

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _context.Simulation
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
                _context.SelectableTreatment.AddRange(treatmentEntities);
            }
            else
            {
                _context.BulkInsert(treatmentEntities);
            }

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

                _treatmentConsequenceRepo.CreateTreatmentConsequences(consequencesPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Costs.Any())
                    .ToDictionary(_ => _.Id, _ => _.Costs.ToList());

                _treatmentCostRepo.CreateTreatmentCosts(costsPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank)))
            {
                var expressionsPerTreatmentId = selectableTreatments
                    .Where(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank))
                    .ToDictionary(_ => _.Id, _ => _.FeasibilityCriteria
                        .Where(__ => !__.ExpressionIsBlank).Select(__ => __.Expression).ToList());

                _criterionLibraryRepo.JoinSelectableTreatmentEntitiesWithCriteria(expressionsPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Schedulings.Any()))
            {
                var schedulingsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Schedulings.Any())
                    .ToDictionary(_ => _.Id, _ => _.Schedulings.ToList());

                _treatmentSchedulingRepo.CreateTreatmentSchedulings(schedulingsPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Supersessions.Any()))
            {
                var supersessionsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Supersessions.Any())
                    .ToDictionary(_ => _.Id, _ => _.Supersessions.ToList());

                _treatmentSupersessionRepo.CreateTreatmentSupersessions(supersessionsPerTreatmentId, simulationEntity.Name);
            }
        }

        private void JoinTreatmentsWithBudgets(Dictionary<Guid, List<Guid>> budgetIdsPerTreatmentId)
        {
            var treatmentBudgetJoins = new List<SelectableTreatmentBudgetEntity>();

            budgetIdsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var budgetIds = budgetIdsPerTreatmentId[treatmentId];
                var budgetEntities = _context.Budget
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
                _context.TreatmentBudget.AddRange(treatmentBudgetJoins);
            }
            else
            {
                _context.BulkInsert(treatmentBudgetJoins);
            }
        }

        public void GetSimulationTreatments(Simulation simulation)
        {
            if (!_context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}.");
            }

            _context.SelectableTreatment
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
