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
            var facility = simulation.Network.Facilities.Single(_ =>
                _.Sections.Any(__ => $"{__.Name}{__.Area}" == $"{domain.Section.Name}{domain.Section.Area}"));
            var section =
                facility.Sections.Single(_ => $"{_.Name}{_.Area}" == $"{domain.Section.Name}{domain.Section.Area}");

            return new CommittedProjectEntity
            {
                Id = domain.Id,
                SimulationId = simulation.Id,
                BudgetId = budget.Id,
                SectionId = section.Id,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Cost = domain.Cost,
                Year = domain.Year
            };
        }

        public static void CreateCommittedProject(this CommittedProjectEntity entity, Simulation simulation)
        {
            var facility = simulation.Network.Facilities.Single(_ => _.Id == entity.Section.Facility.Id);
            var section = facility.Sections.Single(_ => _.Id == entity.Section.Id);

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
