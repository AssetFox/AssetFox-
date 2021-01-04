﻿using System;
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
                Id = domain.Id,
                TreatmentLibraryId = treatmentLibraryId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Description = domain.Description
            };

        public static void CreateSelectableTreatment(this SelectableTreatmentEntity entity, Simulation simulation)
        {
            var selectableTreatment = simulation.AddTreatment();
            selectableTreatment.Id = entity.Id;
            selectableTreatment.Name = entity.Name;
            selectableTreatment.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            selectableTreatment.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            selectableTreatment.Description = entity.Description;

            if (entity.TreatmentBudgetJoins.Any())
            {
                var budgetIds = entity.TreatmentBudgetJoins.Select(_ => _.Budget.Id).ToList();
                simulation.InvestmentPlan.Budgets.Where(_ => budgetIds.Contains(_.Id)).ToList()
                    .ForEach(budget => selectableTreatment.Budgets.Add(budget));
            }

            if (entity.TreatmentConsequences.Any())
            {
                entity.TreatmentConsequences.ForEach(_ => _.CreateConditionalTreatmentConsequence(selectableTreatment));
            }

            if (entity.TreatmentCosts.Any())
            {
                entity.TreatmentCosts.ForEach(_ => _.CreateTreatmentCost(selectableTreatment));
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
                entity.TreatmentSchedulings.ForEach(_ => _.CreateTreatmentScheduling(selectableTreatment));
            }

            if (entity.TreatmentSupersessions.Any())
            {
                entity.TreatmentSupersessions.ForEach(_ => _.CreateTreatmentSupersession(selectableTreatment));
            }

            if (selectableTreatment.Name == "No Treatment")
            {
                selectableTreatment.DesignateAsPassiveForSimulation();
            }
        }
    }
}