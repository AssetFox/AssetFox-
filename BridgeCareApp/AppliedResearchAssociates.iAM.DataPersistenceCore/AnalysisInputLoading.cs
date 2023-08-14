using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore;

public static class AnalysisInputLoading
{
    public static Simulation GetSimulationWithAssets(
        IUnitOfWork unitOfWork,
        Guid networkId,
        Guid simulationId,
        Func<bool> afterExplorer,
        Func<bool> afterNetwork,
        Func<bool> afterSimulation,
        Func<bool> afterInvestmentPlan,
        Func<bool> afterAnalysisMethod,
        Func<bool> afterPerformanceCurves,
        Func<bool> afterSelectableTreatments,
        Func<bool> afterCommittedProjects
        )
        => GetSimulation(
            true,
            unitOfWork,
            networkId,
            simulationId,
            afterExplorer,
            afterNetwork,
            afterSimulation,
            afterInvestmentPlan,
            afterAnalysisMethod,
            afterPerformanceCurves,
            afterSelectableTreatments,
            afterCommittedProjects);

    public static Simulation GetSimulationWithoutAssets(
        IUnitOfWork unitOfWork,
        Guid networkId,
        Guid simulationId
        )
        => GetSimulation(
            false,
            unitOfWork,
            networkId,
            simulationId,
            AlwaysTrue,
            AlwaysTrue,
            AlwaysTrue,
            AlwaysTrue,
            AlwaysTrue,
            AlwaysTrue,
            AlwaysTrue,
            AlwaysTrue);

    private static bool AlwaysTrue() => true;

    private static Simulation GetSimulation(
        bool allowLoadingOfAssets,
        IUnitOfWork unitOfWork,
        Guid networkId,
        Guid simulationId,
        Func<bool> afterExplorer,
        Func<bool> afterNetwork,
        Func<bool> afterSimulation,
        Func<bool> afterInvestmentPlan,
        Func<bool> afterAnalysisMethod,
        Func<bool> afterPerformanceCurves,
        Func<bool> afterSelectableTreatments,
        Func<bool> afterCommittedProjects)
    {
        // load
        var explorer = unitOfWork.AttributeRepo.GetExplorer();

        // intermediate update/check
        if (!afterExplorer())
        {
            return null;
        }

        // load
        var network = unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer, allowLoadingOfAssets);

        // intermediate update/check
        if (!afterNetwork())
        {
            return null;
        }

        // load
        unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

        // intermediate update/check
        if (!afterSimulation())
        {
            return null;
        }

        // load
        var simulation = network.Simulations.Single(_ => _.Id == simulationId);
        unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);

        // intermediate update/check
        if (!afterInvestmentPlan())
        {
            return null;
        }

        // load
        var userCriteria = unitOfWork.UserCriteriaRepo.GetUserCriteria(unitOfWork.CurrentUser.Id);
        unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, userCriteria);

        // intermediate update/check
        if (!afterAnalysisMethod())
        {
            return null;
        }

        // load
        var attributeNameLookup = unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
        unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation, attributeNameLookup);

        // intermediate update/check
        if (!afterPerformanceCurves())
        {
            return null;
        }

        // load
        unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);

        // intermediate update/check
        if (!afterSelectableTreatments())
        {
            return null;
        }

        // load
        unitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);

        // intermediate update/check
        if (!afterCommittedProjects())
        {
            return null;
        }

        // load
        unitOfWork.CalculatedAttributeRepo.PopulateScenarioCalculatedFields(simulation);

        return simulation;
    }
}
