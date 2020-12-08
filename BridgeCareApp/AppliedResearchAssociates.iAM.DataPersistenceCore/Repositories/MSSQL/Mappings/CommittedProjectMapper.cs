using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class CommittedProjectMapper
    {
        public static CommittedProjectEntity ToEntity(this CommittedProject domain, Guid simulationId, Guid budgetId, Guid sectionId)
        {
            return new CommittedProjectEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                BudgetId = budgetId,
                SectionId = sectionId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Cost = domain.Cost,
                Year = domain.Year
            };
        }

        public static void CreateCommittedProject(this CommittedProjectEntity entity, Simulation simulation)
        {
            var facility = simulation.Network.Facilities.Single(_ => _.Name == entity.Section.Facility.Name);
            var section = facility.Sections.Single(_ => _.Name == entity.Section.Name);

            var committedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(section, entity.Year));
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
