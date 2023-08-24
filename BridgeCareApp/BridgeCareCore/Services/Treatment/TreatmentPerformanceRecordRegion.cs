using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Collections.Generic;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentPerformanceRecordRegion
    {
        internal static RowBasedExcelRegionModel PerformanceFactorsRegion(TreatmentDTO dto)
        {
            var rows = new List<ExcelRowModel>
            {
                PFRow(),
                PFHeaderRow(),
            };
            if (dto.PerformanceFactors != null)
            {
                foreach (var pf in dto.PerformanceFactors)
                {
                    var budgetRow = PFRow(pf);
                    rows.Add(budgetRow);
                }
            }

            return RowBasedExcelRegionModels.WithRows(rows);
        }

        private static ExcelRowModel PFRow(TreatmentPerformanceFactorDTO pf)
        {
            var attribute = pf.Attribute;
            var attributeCell = ExcelValueModels.String(attribute);
            var performanceFactor = pf.PerformanceFactor;
            var PFCell = ExcelValueModels.String(performanceFactor.ToString());
            var returnValue = ExcelRowModels.WithEntries(attributeCell, PFCell);
            return returnValue;
        }

        private static ExcelRowModel PFRow()
        {
            var cell = ExcelValueModels.RichString(TreatmentExportStringConstants.PerformanceFactors, true, 14);
            var returnValue = ExcelRowModels.WithEntries(cell);
            return returnValue;
        }
        private static ExcelRowModel PFHeaderRow()
        {
            var attributeCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.Attribute);
            var PFCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.PerformanceFactor);
            var returnValue = ExcelRowModels.WithEntries(attributeCell, PFCell);
            returnValue.EveryCell = ExcelStyleModels.ThinBottomBorder();
            return returnValue;
        }
    }
}
