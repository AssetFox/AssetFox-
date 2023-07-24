using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SimulationCloning;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class CompleteSimulationCloner
    {
        internal static CompleteSimulationDTO Clone(CompleteSimulationDTO completeSimulation, CloneSimulationDTO cloneRequest)
        {
            var cloneAnalysisMethod = AnalysisMethodCloner.Clone(completeSimulation.AnalysisMethod);
            var cloneInvestmentPlan = InvestmentPlanCloner.Clone(completeSimulation.InvestmentPlan);
            {
                NoTreatmentBeforeCommittedProjects = completeSimulation.NoTreatmentBeforeCommittedProjects,                    
                Name = cloneRequest.scenarioName,
                NetworkId = cloneRequest.networkId,
                //figure out where the properties come from
                AnalysisMethod = cloneAnalysisMethod
                
            };
            return clone;

        }
        
    }
}
