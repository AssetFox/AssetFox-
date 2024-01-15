using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.Validation;
using HotChocolate.Language.Visitors;

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
        Guid simulationId,
        ValidationResultBag validationResultBag) // TODO fill up validationResultBag if any errors // add try catch to each call within GetSimulation and build results if allowLoadingOfAssets is false?
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
            AlwaysTrue,
            validationResultBag);

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
        Func<bool> afterCommittedProjects,
        ValidationResultBag validationResultBag = null)
    {
        Explorer explorer = null;
        Network network = null;
        Simulation simulation = null;

        // load
        try
        {
            explorer = unitOfWork.AttributeRepo.GetExplorer();
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterExplorer())
        {
            return null;
        }

        // load
        try
        {
            network = unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer, allowLoadingOfAssets, simulationId);
            if (!allowLoadingOfAssets && !network.Assets.Any())
            {
                var fakeAsset = network.AddAsset();
                fakeAsset.AssetName = "Fake asset";
                fakeAsset.SpatialWeighting.Expression = "0";
            }
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterNetwork())
        {
            return null;
        }

        // load
        try
        {
            unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterSimulation())
        {
            return null;
        }

        // load
        try
        {
            simulation = network.Simulations.Single(_ => _.Id == simulationId);
            unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterInvestmentPlan())
        {
            return null;
        }

        // load
        try
        {
            var userCriteria = unitOfWork.UserCriteriaRepo.GetUserCriteria(unitOfWork.CurrentUser.Id);
            unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, userCriteria);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterAnalysisMethod())
        {
            return null;
        }

        // load
        try
        {
            var attributeNameLookup = unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
            unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation, attributeNameLookup);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterPerformanceCurves())
        {
            return null;
        }

        // load
        try
        {
            unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterSelectableTreatments())
        {
            return null;
        }

        // load
        try
        {
            unitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }
        // intermediate update/check
        if (!afterCommittedProjects())
        {
            return null;
        }

        // load
        try
        {
            unitOfWork.CalculatedAttributeRepo.PopulateScenarioCalculatedFields(simulation);
        }
        catch (Exception ex)
        {
            if (allowLoadingOfAssets)
            {
                throw;
            }
            AddError(validationResultBag, ex.Message);
        }

        return simulation;
    }

    private static void AddError(ValidationResultBag validationResultBag, string errorMessage) => validationResultBag.Add(ValidationStatus.Error, errorMessage, typeof(AnalysisInputLoading));
}
