using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class SelectableTreatmentMapper
    {
        public static SelectableTreatmentEntity ToEntity(this SelectableTreatment domain, Guid treatmentLibraryId) =>
            new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = treatmentLibraryId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Description = domain.Description
            };

        public static void CreateSelectableTreatment(this SelectableTreatmentEntity entity, Simulation simulation)
        {
            var selectableTreatment = simulation.AddTreatment();
            selectableTreatment.Name = entity.Name;
            selectableTreatment.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            selectableTreatment.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            selectableTreatment.Description = entity.Description;

            if (entity.TreatmentBudgetJoins.Any())
            {
                entity.TreatmentBudgetJoins.ForEach(_ =>
                {
                    var budgetNames = entity.TreatmentBudgetJoins.Select(_ => _.Budget.Name).Distinct().ToList();
                    simulation.InvestmentPlan.Budgets.Where(_ => budgetNames.Contains(_.Name)).ToList()
                        .ForEach(budget => selectableTreatment.Budgets.Add(budget));
                });
            }

            if (entity.TreatmentConsequences.Any())
            {
                entity.TreatmentConsequences.ForEach(_ => _.ToSimulationAnalysisDomain(selectableTreatment));
            }

            if (entity.TreatmentCosts.Any())
            {
                entity.TreatmentCosts.ForEach(_ => _.ToSimulationAnalysisDomain(selectableTreatment));
            }

            if (entity.CriterionLibrarySelectableTreatmentJoins.Any())
            {
                entity.CriterionLibrarySelectableTreatmentJoins.ForEach(_ =>
                {
                    var feasibility = selectableTreatment.AddFeasibilityCriterion();
                    feasibility.Expression = _.CriterionLibrary.MergedCriteriaExpression;
                });
            }
            else
            {
                var feasibility = selectableTreatment.AddFeasibilityCriterion();
                feasibility.Expression = string.Empty;
            }

            if (entity.TreatmentSchedulings.Any())
            {
                entity.TreatmentSchedulings.ForEach(_ => _.ToSimulationAnalysisDomain(selectableTreatment));
            }

            if (entity.TreatmentSupersessions.Any())
            {
                entity.TreatmentSupersessions.ForEach(_ => _.ToSimulationAnalysisDomain(selectableTreatment));
            }

            if (selectableTreatment.Name == "No Treatment")
            {
                selectableTreatment.DesignateAsPassiveForSimulation();
            }
        }
    }
}
