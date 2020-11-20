using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AnalysisMethodRepository : MSSQLRepository, IAnalysisMethodRepository
    {
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IBudgetPriorityRepository _budgetPriorityRepo;
        private readonly IInvestmentPlanRepository _investmentPlanRepo;
        private readonly ITargetConditionGoalRepository _targetConditionGoalRepo;
        private readonly IDeficientConditionGoalRepository _deficientConditionGoalRepo;

        public AnalysisMethodRepository(ICriterionLibraryRepository criterionLibraryRepo,
            IBudgetPriorityRepository budgetPriorityRepo,
            IInvestmentPlanRepository investmentPlanRepo,
            ITargetConditionGoalRepository targetConditionGoalRepo,
            IDeficientConditionGoalRepository deficientConditionGoalRepo,
            IAMContext context) : base(context)
        {
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _budgetPriorityRepo = budgetPriorityRepo ?? throw new ArgumentNullException(nameof(budgetPriorityRepo));
            _investmentPlanRepo = investmentPlanRepo ?? throw new ArgumentNullException(nameof(investmentPlanRepo));
            _targetConditionGoalRepo = targetConditionGoalRepo ?? throw new ArgumentNullException(nameof(targetConditionGoalRepo));
            _deficientConditionGoalRepo = deficientConditionGoalRepo ?? throw new ArgumentNullException(nameof(deficientConditionGoalRepo));
        }

        public void CreateAnalysisMethod(AnalysisMethod analysisMethod, string simulationName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}");
                    }

                    if (!Context.Attribute.Any(_ => _.Name == analysisMethod.Weighting.Name))
                    {
                        throw new RowNotInTableException($"No attribute found having name {analysisMethod.Weighting.Name}");
                    }

                    var simulation = Context.Simulation.Single(_ => _.Name == simulationName);
                    var attribute = Context.Attribute.Single(_ => _.Name == analysisMethod.Weighting.Name);

                    var analysisMethodEntity = analysisMethod.ToEntity(simulation.Id, attribute.Id);
                    Context.AnalysisMethod.Add(analysisMethodEntity);
                    Context.SaveChanges();

                    if (!analysisMethod.Filter.ExpressionIsBlank)
                    {
                        var analysisMethodEntityIdPerExpression = new Dictionary<string, List<Guid>>
                        {
                            {analysisMethod.Filter.Expression, new List<Guid> {analysisMethodEntity.Id}}
                        };
                        _criterionLibraryRepo.JoinEntitiesWithCriteria(analysisMethodEntityIdPerExpression,
                            "AnalysisMethodEntity", simulationName);
                    }

                    if (analysisMethod.BudgetPriorities.Any())
                    {
                        CreateAnalysisMethodBudgetPriorities(simulationName, analysisMethod.BudgetPriorities.ToList());
                    }

                    if (analysisMethod.TargetConditionGoals.Any())
                    {
                        CreateAnalysisMethodTargetConditionGoals(simulationName, analysisMethod.TargetConditionGoals.ToList());
                    }

                    if (analysisMethod.DeficientConditionGoals.Any())
                    {
                        CreateAnalysisMethodDeficientConditionGoals(simulationName, analysisMethod.DeficientConditionGoals.ToList());
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

        private void CreateAnalysisMethodBudgetPriorities(string simulationName, List<BudgetPriority> budgetPriorities)
        {
            _budgetPriorityRepo.CreateBudgetPriorityLibrary($"{simulationName} Simulation Budget Priority Library", simulationName);

            _budgetPriorityRepo.CreateBudgetPriorities(budgetPriorities, simulationName);
        }

        private void CreateAnalysisMethodTargetConditionGoals(string simulationName, List<TargetConditionGoal> targetConditionGoals)
        {
            _targetConditionGoalRepo.CreateTargetConditionGoalLibrary($"{simulationName} Simulation Target Condition Goal Library", simulationName);

            _targetConditionGoalRepo.CreateTargetConditionGoals(targetConditionGoals, simulationName);
        }

        private void CreateAnalysisMethodDeficientConditionGoals(string simulationName, List<DeficientConditionGoal> deficientConditionGoals)
        {
            _deficientConditionGoalRepo.CreateDeficientConditionGoalLibrary($"{simulationName} Simulation Deficient Condition Goal Library", simulationName);

            _deficientConditionGoalRepo.CreateDeficientConditionGoals(deficientConditionGoals, simulationName);
        }

        public AnalysisMethod GetSimulationAnalysisMethod(string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var investmentPlan = _investmentPlanRepo.GetSimulationInvestmentPlan(simulationName);

            return Context.AnalysisMethod.Include(_ => _.Simulation)
                .ThenInclude(_ => _.Network)
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetPriorityLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetPriorityLibrary)
                .ThenInclude(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.TargetConditionGoalLibrary)
                .ThenInclude(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.DeficientConditionGoalLibrary)
                .ThenInclude(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Single(_ => _.Simulation.Name == simulationName)
                .ToDomain(investmentPlan);
        }
    }
}
