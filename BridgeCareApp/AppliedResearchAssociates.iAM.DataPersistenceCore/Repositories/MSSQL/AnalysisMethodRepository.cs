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
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        public AnalysisMethodRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (!_unitOfDataPersistenceWork.Context.Attribute.Any(_ => _.Name == analysisMethod.Weighting.Name))
            {
                throw new RowNotInTableException($"No attribute found having name {analysisMethod.Weighting.Name}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation.Single(_ => _.Id == simulationId);
            var attributeEntity = _unitOfDataPersistenceWork.Context.Attribute.Single(_ => _.Name == analysisMethod.Weighting.Name);

            var analysisMethodEntity = analysisMethod.ToEntity(simulationEntity.Id, attributeEntity.Id);
            _unitOfDataPersistenceWork.Context.AnalysisMethod.Add(analysisMethodEntity);
            _unitOfDataPersistenceWork.Context.SaveChanges();

            _unitOfDataPersistenceWork.BenefitRepo.CreateBenefit(analysisMethod.Benefit, analysisMethodEntity.Id);

            if (!analysisMethod.Filter.ExpressionIsBlank)
            {
                var analysisMethodEntityIdPerExpression = new Dictionary<string, List<Guid>>
                        {
                            {analysisMethod.Filter.Expression, new List<Guid> {analysisMethodEntity.Id}}
                        };
                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(analysisMethodEntityIdPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.AnalysisMethod, simulationEntity.Name);
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
            _unitOfDataPersistenceWork.BudgetPriorityRepo.CreateBudgetPriorityLibrary($"{simulationEntity.Name} Simulation Budget Priority Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.BudgetPriorityRepo.CreateBudgetPriorities(budgetPriorities, simulationEntity.Id);
        }

        private void CreateAnalysisMethodTargetConditionGoals(SimulationEntity simulationEntity, List<TargetConditionGoal> targetConditionGoals)
        {
            _unitOfDataPersistenceWork.TargetConditionGoalRepo.CreateTargetConditionGoalLibrary($"{simulationEntity.Name} Simulation Target Condition Goal Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.TargetConditionGoalRepo.CreateTargetConditionGoals(targetConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodDeficientConditionGoals(SimulationEntity simulationEntity, List<DeficientConditionGoal> deficientConditionGoals)
        {
            _unitOfDataPersistenceWork.DeficientConditionGoalRepo.CreateDeficientConditionGoalLibrary($"{simulationEntity.Name} Simulation Deficient Condition Goal Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.DeficientConditionGoalRepo.CreateDeficientConditionGoals(deficientConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodRemainingLifeLimits(SimulationEntity simulationEntity,
            List<RemainingLifeLimit> remainingLifeLimits)
        {
            _unitOfDataPersistenceWork.RemainingLifeLimitRepo.CreateRemainingLifeLimitLibrary($"{simulationEntity.Name} Simulation Remaining Life Limit Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.RemainingLifeLimitRepo.CreateRemainingLifeLimits(remainingLifeLimits, simulationEntity.Id);
        }

        public void GetSimulationAnalysisMethod(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}");
            }

            _unitOfDataPersistenceWork.Context.AnalysisMethod.Include(_ => _.Attribute)
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
