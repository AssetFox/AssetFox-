using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using HotChocolate;
using AppliedResearchAssociates.iAM.Data.SimulationCloning;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.Data.SimulationCloning;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    internal class CompleteSimulationRepository : ICompleteSimulationRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CompleteSimulationRepository(UnitOfDataPersistenceWork unitOfWork)
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
            var badCommittedProjects = sourceSimulation.CommittedProjects.Where(c => ! sourceSimulation.Budgets.Any(b => b.Id==c.ScenarioBudgetId)).ToList();
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
            var ownerId = _unitOfWork.CurrentUser?.Id??Guid.Empty;
            var baseEntityProperties = new BaseEntityProperties {CreatedBy = ownerId, LastModifiedBy = ownerId };
            var ownerName = _unitOfWork.CurrentUser?.Username;
            var cloneSimulation = CompleteSimulationCloner.Clone(sourceSimulation, dto, ownerId, ownerName);

            // save it
            var network = _unitOfWork.Context.Network.First(n => n.Id == dto.NetworkId);            
            var clone = CreateNewSimulation(cloneSimulation, network.KeyAttributeId, simulationCloningCommittedProjectErrors, baseEntityProperties);
            return clone;
        }

        private SimulationCloningResultDTO CreateNewSimulation(CompleteSimulationDTO completeSimulationDTO, Guid keyAttributeId, SimulationCloningCommittedProjectErrors simulationCloningCommittedProjectErrors, BaseEntityProperties baseEntityProperties)
        {
            var attributes = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
            var keyAttribute = _unitOfWork.AttributeRepo.GetAttributeName(keyAttributeId);
            var entity = CompleteSimulationMapper.ToNewEntity(completeSimulationDTO, attributes, keyAttribute, baseEntityProperties);

            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.Context.AddEntity(entity);               
            });
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(completeSimulationDTO.Id);
            var warningMessage = simulationCloningCommittedProjectErrors.BudgetsPreventingCloning.Any() && simulationCloningCommittedProjectErrors.NumberOfCommittedProjectsAffected > 0
                    ? $"The following committed project budgets were not found which has prevented {simulationCloningCommittedProjectErrors.NumberOfCommittedProjectsAffected} committed project(s) from being cloned: {string.Join(", ", simulationCloningCommittedProjectErrors.BudgetsPreventingCloning)}"
                    : null;
            var cloningResult = new SimulationCloningResultDTO
            {
                Simulation = simulation,
                WarningMessage = warningMessage,
            };
            return cloningResult;
        }
    }
}
