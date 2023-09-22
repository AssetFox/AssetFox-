﻿using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.Common;

namespace BridgeCareCore.Services.SimulationCloning
{
    public class CompleteSimulationCloningService : ICompleteSimulationCloningService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompleteSimulationCloningService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public CompleteSimulationDTO GetSimulation(string simulationId)
        {
            var simulationGuid = new Guid(simulationId);
            var coreSimulation = _unitOfWork.SimulationRepo.GetSimulation(simulationGuid);
            var fullSimulation = new CompleteSimulationDTO()
            {
                Name = coreSimulation.Name,
                NetworkId = coreSimulation.NetworkId,
                ReportStatus = coreSimulation.ReportStatus,
                Id = simulationGuid,


            };

            fullSimulation.AnalysisMethod = _unitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationGuid);
            fullSimulation.BudgetPriorities = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationGuid);
            fullSimulation.InvestmentPlan = _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationGuid);
            fullSimulation.ReportIndexes = _unitOfWork.ReportIndexRepository.GetAllForScenario(simulationGuid);
            fullSimulation.CommittedProjects = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationGuid);
            fullSimulation.CalculatedAttributes = _unitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(simulationGuid).ToList();
            fullSimulation.Treatments = _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationGuid);
            fullSimulation.TargetConditionGoals = _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationGuid);
            fullSimulation.Budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationGuid);
            fullSimulation.DeficientConditionGoals = _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationGuid);
            fullSimulation.BudgetPriorities = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationGuid);
            fullSimulation.RemainingLifeLimits = _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationGuid);
            fullSimulation.CashFlowRules = _unitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationGuid);
            fullSimulation.PerformanceCurves = _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationGuid);


            return fullSimulation;
        }
        public SimulationCloningResultDTO Clone(CloneSimulationDTO dto)
        {
            // load it
            var sourceSimulationId = dto.SourceScenarioId.ToString();
            var sourceSimulation = this.GetSimulation(sourceSimulationId);
            var simulationCloningCommittedProjectErrors = new SimulationCloningCommittedProjectErrors();
            var badCommittedProjects = sourceSimulation.CommittedProjects.Where(c => !sourceSimulation.Budgets.Any(b => b.Id == c.ScenarioBudgetId)).ToList();
            if (badCommittedProjects.Any())
            {
                var badCommittedProjectIds = badCommittedProjects.Select(b => b.ScenarioBudgetId.Value).ToList();
                var dictionary = _unitOfWork.BudgetRepo.GetScenarioBudgetDictionary(badCommittedProjectIds);
                simulationCloningCommittedProjectErrors.NumberOfCommittedProjectsAffected = badCommittedProjects.Count();
                foreach (var badBudgetName in dictionary.Values)
                {
                    if (!simulationCloningCommittedProjectErrors.BudgetsPreventingCloning.Contains(badBudgetName))
                    {
                        simulationCloningCommittedProjectErrors.BudgetsPreventingCloning.Add(badBudgetName);
                    }
                }
            }

            // do the clone
            var ownerId = _unitOfWork.CurrentUser?.Id ?? Guid.Empty;
            var baseEntityProperties = new BaseEntityProperties { CreatedBy = ownerId, LastModifiedBy = ownerId };//Unclear if it should be at the BridgeCareCore level
            var ownerName = _unitOfWork.CurrentUser?.Username;
            var cloneSimulation = CompleteSimulationCloner.Clone(sourceSimulation, dto, ownerId, ownerName);//This can be at the BridgeCareCore level

            // save it
            var keyAttribute = _unitOfWork.NetworkRepo.GetNetworkKeyAttribute(dto.NetworkId);           
            var clone = _unitOfWork.SimulationRepo.CreateSimulation(cloneSimulation, keyAttribute, simulationCloningCommittedProjectErrors, baseEntityProperties);
            return clone;
        }
    }
}
