using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Models.SummaryReport;
using static AppliedResearchAssociates.iAM.Domains.SelectableTreatment;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public static class WorkTypeTotalHelper
    {
        public static void FillWorkTypeTotals(YearsData item, WorkTypeTotal workTypeTotal)
        {
            var treatmentCategory = item.TreatmentCategory;

            switch (treatmentCategory)
            {
            case TreatmentCategory.Preservation:
                if (!workTypeTotal.PreservationCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.PreservationCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.PreservationCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case TreatmentCategory.CapacityAdding:
                if (!workTypeTotal.CapacityAddingCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.CapacityAddingCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.CapacityAddingCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case TreatmentCategory.Rehabilitation:
                if (!workTypeTotal.RehabilitationCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.RehabilitationCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.RehabilitationCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case TreatmentCategory.Replacement:
                if (!workTypeTotal.ReplacementCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.ReplacementCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.ReplacementCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case TreatmentCategory.Maintenance:
                if (!workTypeTotal.MaintenanceCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.MaintenanceCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.MaintenanceCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case TreatmentCategory.Other:
                if (!workTypeTotal.OtherCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.OtherCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.OtherCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            default:
                if (!workTypeTotal.OtherCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.OtherCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.OtherCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            }
        }
    }
}
