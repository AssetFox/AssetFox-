using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CommittedProjectMapper
    {
        public static CommittedProjectEntity ToEntity(this CommittedProject domain, SimulationEntity simulation)
        {
            var budget =
                simulation.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets.Single(_ => _.Name == domain.Budget.Name);

            var maintainableAsset = simulation.Network.MaintainableAssets.Single(_ =>
                _.Id == domain.Section.Id);

            return new CommittedProjectEntity
            {
                Id = domain.Id,
                SimulationId = simulation.Id,
                BudgetId = budget.Id,
                MaintainableAssetId = maintainableAsset.Id,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Cost = domain.Cost,
                Year = domain.Year
            };
        }

        public static void CreateCommittedProject(this CommittedProjectEntity entity, Simulation simulation)
        {
            var facilitySectionNameSplit =
                entity.MaintainableAsset.MaintainableAssetLocation.LocationIdentifier.Split("-");

            var facility = simulation.Network.Facilities
                .Single(_ => _.Name == facilitySectionNameSplit[0]);
            var section = facility.Sections.Single(_ =>
                _.Id == entity.MaintainableAsset.Id);

            var committedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(section, entity.Year));
            committedProject.Id = entity.Id;
            committedProject.Name = entity.Name;
            committedProject.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            committedProject.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            committedProject.Cost = entity.Cost;
            committedProject.Budget = simulation.InvestmentPlan.Budgets.Single(_ => _.Name == entity.Budget.Name);

            if (entity.CommittedProjectConsequences.Any())
            {
                entity.CommittedProjectConsequences.ForEach(_ => _.CreateCommittedProjectConsequence(committedProject));
            }
        }
    }
}
