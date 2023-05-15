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
    /// <summary>
    /// The GraphQL query.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Get all simulations.
        /// </summary>
        /// <param name="_unitOfWork">An instance of a transaction with the database.</param>
        /// <returns>List of all simulations.</returns>
        [UseFiltering]
        [UseSorting]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork) => _unitOfWork.SimulationRepo.GetAllScenario();

        /// <summary>
        /// Get a simulation object and its repositories.
        /// </summary>
        /// <param name="_unitOfWork">An instance of a transaction with the database.</param>
        /// <param name="simulationId">The simulation ID passed in.</param>
        /// <returns>Simulation object with access to all its repositories, based on simulation ID.</returns>
        [UseFiltering]
        [UseSorting]
        public CompleteSimulationDTO GetSimulation([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId)
        {
            var simulationGuid = new Guid(simulationId);
            var coreSimulation = _unitOfWork.SimulationRepo.GetSimulation(simulationGuid);
            var fullSimulation = new CompleteSimulationDTO()
            {
                Name = coreSimulation.Name,
                NetworkId = coreSimulation.NetworkId,
                ReportStatus = coreSimulation.ReportStatus
            };

            fullSimulation.AnalysisMethod = _unitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationGuid);
            fullSimulation.BudgetPriorities = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationGuid);
            fullSimulation.InvestmentPlan = _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationGuid);
            fullSimulation.ReportIndexes = _unitOfWork.ReportIndexRepository.GetAllForScenario(simulationGuid);
            fullSimulation.CommittedProjects = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationGuid);
            fullSimulation.CalculatedAttributes = _unitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(simulationGuid).ToList();            
            fullSimulation.Treatments = _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(simulationGuid);
            fullSimulation.TargetConditionGoals = _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationGuid);
            fullSimulation.Budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationGuid);
            fullSimulation.DeficientConditionGoals = _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationGuid);
            fullSimulation.BudgetPriorities = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationGuid);
            fullSimulation.RemainingLifeLimits = _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationGuid);
            fullSimulation.CashFlowRules = _unitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationGuid);

            return fullSimulation;
        }

        /// <summary>
        /// Get the simulation results of an analysis.
        /// </summary>
        /// <param name="_unitOfWork">An instance of a transaction with the database.</param>
        /// <param name="simulationId">The simulation ID passed in.</param>
        /// <returns>Simulation output object, based on simulation ID.</returns>
        [UseFiltering]
        [UseSorting]
        public SimulationOutput GetSimulationOutput([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId) 
        {
            var simulationGuid = new Guid(simulationId);
            _unitOfWork.SimulationRepo.GetSimulation(simulationGuid);
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationGuid);
            return simulationOutput;
        }
    }  
}
