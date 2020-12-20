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
    public class AnalysisMethodRepository : IAnalysisMethodRepository
    {
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IBudgetPriorityRepository _budgetPriorityRepo;
        private readonly ITargetConditionGoalRepository _targetConditionGoalRepo;
        private readonly IDeficientConditionGoalRepository _deficientConditionGoalRepo;
        private readonly IBenefitRepository _benefitRepo;
        private readonly IRemainingLifeLimitRepository _remainingLifeLimitRepo;
        private readonly IAMContext _context;

        public AnalysisMethodRepository(ICriterionLibraryRepository criterionLibraryRepo,
            IBudgetPriorityRepository budgetPriorityRepo,
            ITargetConditionGoalRepository targetConditionGoalRepo,
            IDeficientConditionGoalRepository deficientConditionGoalRepo,
            IBenefitRepository benefitRepo,
            IRemainingLifeLimitRepository remainingLifeLimitRepo,
            IAMContext context)
        {
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _budgetPriorityRepo = budgetPriorityRepo ?? throw new ArgumentNullException(nameof(budgetPriorityRepo));
            _targetConditionGoalRepo = targetConditionGoalRepo ?? throw new ArgumentNullException(nameof(targetConditionGoalRepo));
            _deficientConditionGoalRepo = deficientConditionGoalRepo ?? throw new ArgumentNullException(nameof(deficientConditionGoalRepo));
            _benefitRepo = benefitRepo ?? throw new ArgumentNullException(nameof(benefitRepo));
            _remainingLifeLimitRepo = remainingLifeLimitRepo ?? throw new ArgumentNullException(nameof(remainingLifeLimitRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (!_context.Attribute.Any(_ => _.Name == analysisMethod.Weighting.Name))
            {
                throw new RowNotInTableException($"No attribute found having name {analysisMethod.Weighting.Name}");
            }

            var simulationEntity = _context.Simulation.Single(_ => _.Id == simulationId);
            var attributeEntity = _context.Attribute.Single(_ => _.Name == analysisMethod.Weighting.Name);

            var analysisMethodEntity = analysisMethod.ToEntity(simulationEntity.Id, attributeEntity.Id);
            _context.AnalysisMethod.Add(analysisMethodEntity);

            _benefitRepo.CreateBenefit(analysisMethod.Benefit, analysisMethodEntity.Id);

            if (!analysisMethod.Filter.ExpressionIsBlank)
            {
                var analysisMethodEntityIdPerExpression = new Dictionary<string, List<Guid>>
                        {
                            {analysisMethod.Filter.Expression, new List<Guid> {analysisMethodEntity.Id}}
                        };
                _criterionLibraryRepo.JoinEntitiesWithCriteria(analysisMethodEntityIdPerExpression,
                    "AnalysisMethodEntity", simulationEntity.Name);
            }

            if (analysisMethod.BudgetPriorities.Any())
            {
                CreateAnalysisMethodBudgetPriorities(simulationEntity, analysisMethod.BudgetPriorities.ToList());
            }

            if (analysisMethod.TargetConditionGoals.Any())
            {
                CreateAnalysisMethodTargetConditionGoals(simulationEntity, analysisMethod.TargetConditionGoals.ToList());
            }

            if (analysisMethod.DeficientConditionGoals.Any())
            {
                CreateAnalysisMethodDeficientConditionGoals(simulationEntity, analysisMethod.DeficientConditionGoals.ToList());
            }

            if (analysisMethod.RemainingLifeLimits.Any())
            {
                CreateAnalysisMethodRemainingLifeLimits(simulationEntity, analysisMethod.RemainingLifeLimits.ToList());
            }
        }

        private void CreateAnalysisMethodBudgetPriorities(SimulationEntity simulationEntity, List<BudgetPriority> budgetPriorities)
        {
            _budgetPriorityRepo.CreateBudgetPriorityLibrary($"{simulationEntity.Name} Simulation Budget Priority Library", simulationEntity.Id);

            _budgetPriorityRepo.CreateBudgetPriorities(budgetPriorities, simulationEntity.Id);
        }

        private void CreateAnalysisMethodTargetConditionGoals(SimulationEntity simulationEntity, List<TargetConditionGoal> targetConditionGoals)
        {
            _targetConditionGoalRepo.CreateTargetConditionGoalLibrary($"{simulationEntity.Name} Simulation Target Condition Goal Library", simulationEntity.Id);

            _targetConditionGoalRepo.CreateTargetConditionGoals(targetConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodDeficientConditionGoals(SimulationEntity simulationEntity, List<DeficientConditionGoal> deficientConditionGoals)
        {
            _deficientConditionGoalRepo.CreateDeficientConditionGoalLibrary($"{simulationEntity.Name} Simulation Deficient Condition Goal Library", simulationEntity.Id);

            _deficientConditionGoalRepo.CreateDeficientConditionGoals(deficientConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodRemainingLifeLimits(SimulationEntity simulationEntity,
            List<RemainingLifeLimit> remainingLifeLimits)
        {
            _remainingLifeLimitRepo.CreateRemainingLifeLimitLibrary($"{simulationEntity.Name} Simulation Remaining Life Limit Library", simulationEntity.Id);

            _remainingLifeLimitRepo.CreateRemainingLifeLimits(remainingLifeLimits, simulationEntity.Id);
        }

        public void GetSimulationAnalysisMethod(Simulation simulation)
        {
            if (!_context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}");
            }

            _context.AnalysisMethod.Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetPriorityLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetPriorityLibrary)
                .ThenInclude(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryBudgetPriorityJoin)
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
                .ThenInclude(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.TargetConditionGoalLibrary)
                .ThenInclude(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.DeficientConditionGoalLibrary)
                .ThenInclude(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.DeficientConditionGoalLibrary)
                .ThenInclude(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .ThenInclude(_ => _.RemainingLifeLimitLibrary)
                .ThenInclude(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .ThenInclude(_ => _.RemainingLifeLimitLibrary)
                .ThenInclude(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.Simulation.Name == simulation.Name)
                .FillSimulationAnalysisMethod(simulation);
        }
    }
}
