using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentDetailsRegion
    {
        internal static RowBasedExcelRegionModel DetailsRegion(TreatmentDTO dto)
        {
            var rows = new List<ExcelRowModel>
            {
                TreatmentNameRow(dto),
                CriteriaRow(dto),
                CategoryRow(dto),
                AssetTypeRow(dto),
                YearsBeforeAnyRow(dto),
                YearsBeforeSameRow(dto),
                TreatmentDescriptionRow(dto),
            };
            return RowBasedExcelRegionModels.WithRows(rows);
        }


        public static ExcelRowModel TitleThenContent(string title, string content)
        {
            var firstCell = ExcelValueModels.RichString(title, true);
            var nameCell = ExcelValueModels.String(content);
            return ExcelRowModels.WithEntries(firstCell, nameCell);
        }
        public static ExcelRowModel TreatmentNameRow(TreatmentDTO dto)
        {
            return TitleThenContent(TreatmentExportStringConstants.TreatmentName, dto.Name);
        }

        public static ExcelRowModel CriteriaRow(TreatmentDTO dto)
        {
            var criteria = dto.CriterionLibrary.MergedCriteriaExpression;
            return TitleThenContent(TreatmentExportStringConstants.Criteria, criteria);
        }

        public static ExcelRowModel CategoryRow(TreatmentDTO dto)
        {
            var categoryName = dto.Category.ToString();
            return TitleThenContent(TreatmentExportStringConstants.Category, categoryName);
        }

        public static ExcelRowModel AssetTypeRow(TreatmentDTO dto)
        {
            var assetTypeName = dto.AssetType.ToString();
            return TitleThenContent(TreatmentExportStringConstants.AssetType, assetTypeName);
        }

        public static ExcelRowModel YearsBeforeAnyRow(TreatmentDTO dto)
        {
            var yearsBeforeAny = dto.ShadowForAnyTreatment.ToString();
            return TitleThenContent(TreatmentExportStringConstants.YearsBeforeAny, yearsBeforeAny);
        }
        public static ExcelRowModel YearsBeforeSameRow(TreatmentDTO dto)
        {
            var yearsBeforeSame = dto.ShadowForSameTreatment.ToString();
            return TitleThenContent(TreatmentExportStringConstants.YearsBeforeSame, yearsBeforeSame);
        }

        public static ExcelRowModel TreatmentDescriptionRow(TreatmentDTO dto)
        {
            var description = dto.Description;
            return TitleThenContent(TreatmentExportStringConstants.TreatmentDescription, description);
        }
    }
}
