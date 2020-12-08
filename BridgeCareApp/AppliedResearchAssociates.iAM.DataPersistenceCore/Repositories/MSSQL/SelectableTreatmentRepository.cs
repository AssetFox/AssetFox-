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
    public class SelectableTreatmentRepository : MSSQLRepository, ISelectableTreatmentRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ITreatmentConsequenceRepository _treatmentConsequenceRepo;
        private readonly ITreatmentCostRepository _treatmentCostRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly ITreatmentSchedulingRepository _treatmentSchedulingRepo;
        private readonly ITreatmentSupersessionRepository _treatmentSupersessionRepo;

        public SelectableTreatmentRepository(ITreatmentConsequenceRepository treatmentConsequenceRepo,
            ITreatmentCostRepository treatmentCostRepo,
            ICriterionLibraryRepository criterionLibraryRepo,
            ITreatmentSchedulingRepository treatmentSchedulingRepo,
            ITreatmentSupersessionRepository treatmentSupersessionRepo,
            IAMContext context) : base(context)
        {
            _treatmentConsequenceRepo = treatmentConsequenceRepo ??
                                        throw new ArgumentNullException(nameof(treatmentConsequenceRepo));
            _treatmentCostRepo = treatmentCostRepo ?? throw new ArgumentNullException(nameof(treatmentCostRepo));
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _treatmentSchedulingRepo = treatmentSchedulingRepo ??
                                       throw new ArgumentNullException(nameof(treatmentSchedulingRepo));
            _treatmentSupersessionRepo = treatmentSupersessionRepo ??
                                         throw new ArgumentNullException(nameof(treatmentSupersessionRepo));
        }

        public void CreateTreatmentLibrary(string name, string simulationName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}.");
                    }

                    var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

                    var treatmentLibraryEntity = new TreatmentLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = name
                    };

                    Context.TreatmentLibrary.Add(treatmentLibraryEntity);

                    Context.TreatmentLibrarySimulation.Add(new TreatmentLibrarySimulationEntity
                    {
                        TreatmentLibraryId = treatmentLibraryEntity.Id,
                        SimulationId = simulationEntity.Id
                    });

                    Context.SaveChanges();

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

        public void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, string simulationName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}.");
                    }

                    var simulationEntity = Context.Simulation
                        .Include(_ => _.TreatmentLibrarySimulationJoin)
                        .Single(_ => _.Name == simulationName);

                    var expressionsPerSelectableTreatmentEntityId = new Dictionary<Guid, List<string>>();

                    var treatmentEntities = selectableTreatments
                        .Select(_ =>
                        {
                            var selectableTreatmentEntity = _.ToEntity(simulationEntity.TreatmentLibrarySimulationJoin.TreatmentLibraryId);

                            if (_.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank))
                            {
                                var expressions = _.FeasibilityCriteria.Where(__ => !__.ExpressionIsBlank)
                                    .Select(__ => __.Expression).ToList();

                                expressionsPerSelectableTreatmentEntityId.Add(selectableTreatmentEntity.Id, expressions);
                            }

                            return selectableTreatmentEntity;
                        })
                        .ToList();

                    if (IsRunningFromXUnit)
                    {
                        Context.SelectableTreatment.AddRange(treatmentEntities);
                    }
                    else
                    {
                        Context.BulkInsert(treatmentEntities);
                    }

                    Context.SaveChanges();

                    if (selectableTreatments.Any(_ => _.Budgets.Any()))
                    {
                        var budgetNamesPerTreatmentId = selectableTreatments
                            .Where(_ => _.Budgets.Any())
                            .ToDictionary(_ => treatmentEntities.Single(__ => __.Name == _.Name).Id,
                                _ => _.Budgets.Select(__ => __.Name).ToList());
                        JoinTreatmentsWithBudgets(budgetNamesPerTreatmentId, simulationName);
                    }

                    if (selectableTreatments.Any(_ => _.Consequences.Any()))
                    {
                        var consequencesPerTreatmentId = selectableTreatments
                            .Where(_ => _.Consequences.Any())
                            .ToDictionary(_ => treatmentEntities.Single(__ => __.Name == _.Name).Id,
                                _ => _.Consequences.ToList());

                        _treatmentConsequenceRepo.CreateTreatmentConsequences(consequencesPerTreatmentId, simulationName);
                    }

                    if (selectableTreatments.Any(_ => _.Costs.Any()))
                    {
                        var costsPerTreatmentId = selectableTreatments
                            .Where(_ => _.Costs.Any())
                            .ToDictionary(_ => treatmentEntities.Single(__ => __.Name == _.Name).Id,
                                _ => _.Costs.ToList());

                        _treatmentCostRepo.CreateTreatmentCosts(costsPerTreatmentId, simulationName);
                    }

                    if (expressionsPerSelectableTreatmentEntityId.Values.Any())
                    {
                        _criterionLibraryRepo.JoinSelectableTreatmentEntitiesWithCriteria(expressionsPerSelectableTreatmentEntityId, simulationName);
                    }

                    if (selectableTreatments.Any(_ => _.Schedulings.Any()))
                    {
                        var schedulingsPerTreatmentId = selectableTreatments
                            .Where(_ => _.Schedulings.Any())
                            .ToDictionary(_ => treatmentEntities.Single(__ => __.Name == _.Name).Id,
                                _ => _.Schedulings.ToList());

                        _treatmentSchedulingRepo.CreateTreatmentSchedulings(schedulingsPerTreatmentId);
                    }

                    if (selectableTreatments.Any(_ => _.Supersessions.Any()))
                    {
                        var supersessionsPerTreatmentId = selectableTreatments
                            .Where(_ => _.Supersessions.Any())
                            .ToDictionary(_ => treatmentEntities.Single(__ => __.Name == _.Name).Id,
                                _ => _.Supersessions.ToList());

                        _treatmentSupersessionRepo.CreateTreatmentSupersessions(supersessionsPerTreatmentId, simulationName);
                    }

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

        private void JoinTreatmentsWithBudgets(Dictionary<Guid, List<string>> budgetNamesPerTreatmentId, string simulationName)
        {
            var treatmentBudgetJoins = new List<SelectableTreatmentBudgetEntity>();

            budgetNamesPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var budgetNames = budgetNamesPerTreatmentId[treatmentId];
                var budgetEntities = Context.Budget
                    .Include(_ => _.InvestmentPlan)
                    .ThenInclude(_ => _.InvestmentPlanSimulationJoins)
                    .ThenInclude(_ => _.Simulation)
                    .Where(_ => budgetNames.Contains(_.Name) &&
                                _.InvestmentPlan.InvestmentPlanSimulationJoins.SingleOrDefault(__ =>
                                    __.Simulation.Name == simulationName) != null);

                if (!budgetEntities.Any())
                {
                    throw new RowNotInTableException("No budgets for treatments were found.");
                }

                var budgetNamesFromDataSource = budgetEntities.Select(_ => _.Name).ToList();
                if (!budgetNames.All(budgetName => budgetNamesFromDataSource.Contains(budgetName)))
                {
                    var budgetNamesNotFound = budgetNames.Except(budgetNamesFromDataSource).ToList();
                    if (budgetNamesNotFound.Count() == 1)
                    {
                        throw new RowNotInTableException($"No budget found having name {budgetNamesNotFound[0]}.");
                    }

                    throw new RowNotInTableException($"No budgets found having names: {string.Join(", ", budgetNamesNotFound)}");
                }

                treatmentBudgetJoins.AddRange(budgetEntities.Select(_ => new SelectableTreatmentBudgetEntity
                {
                    SelectableTreatmentId = treatmentId,
                    BudgetId = _.Id
                }));
            });

            if (IsRunningFromXUnit)
            {
                Context.TreatmentBudget.AddRange(treatmentBudgetJoins);
            }
            else
            {
                Context.BulkInsert(treatmentBudgetJoins);
            }

            Context.SaveChanges();
        }

        public void GetSimulationTreatments(Simulation simulation)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}.");
            }

            Context.SelectableTreatment
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
