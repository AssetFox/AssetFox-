using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting;
using HotChocolate;
using HotChocolate.Data;

namespace BridgeCareCore.GraphQL
{
    public class Query
    {
        [UseFiltering]
        [UseSorting]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork) =>
            _unitOfWork.SimulationRepo.GetAllScenario();


        [UseFiltering]
        [UseSorting]
        public CompleteSimulationDTO GetSimulation([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId)
        {
            var coreSimulation = _unitOfWork.SimulationRepo.GetSimulation(new System.Guid(simulationId));
            var simulationGuid = new Guid(simulationId);
            var fullSimulation = new CompleteSimulationDTO()
            {
                Name = coreSimulation.Name,
                NetworkId = coreSimulation.NetworkId,
                ReportStatus = coreSimulation.ReportStatus,

            };
            fullSimulation.AnalysisMethod = _unitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationGuid);
            fullSimulation.BudgetPriorities = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationGuid);
            fullSimulation.InvestmentPlan = _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationGuid);
            fullSimulation.ReportIndexes = _unitOfWork.ReportIndexRepository.GetAllForScenario(simulationGuid);
            fullSimulation.CommittedProjects = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationGuid);
            fullSimulation.CalculatedAttributes = _unitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(simulationGuid).ToList();            
           // fullSimulation.Treatments = (IList<ReducedTreatmentDTO>)_unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(simulationGuid);
            fullSimulation.TargetConditionGoals = _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationGuid);
            fullSimulation.Budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationGuid);
            fullSimulation.DeficientConditionGoals = _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationGuid);
            fullSimulation.BudgetPriorities = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationGuid);
            fullSimulation.RemainingLifeLimits = _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationGuid);
            fullSimulation.CashFlowRules = _unitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationGuid);
         

            return fullSimulation;
        }

        [UseFiltering]
        [UseSorting]
        /// <summary>
        /// Method that GetSimulationsOutput query uses in order to retrieve simulation output object via simulation id.
        /// </summary>
        /// <param name="simulationId">The simulation id passed in.</param>
        /// <returns>Simulation output object based on simulation id.</returns>
        public SimulationOutput GetSimulationOutputs([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId) 
        {
            _unitOfWork.SimulationRepo.GetSimulation(new Guid(simulationId));
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(new Guid(simulationId));
            return simulationOutput;
        }
 
    }
    
}
