using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;

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
                TreatmentIsUnselectableRow(dto)
            };
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        public static ExcelRowModel SizedTitleThenContent(string title, string content, int fontSize)
        {
            var firstCell = StackedExcelModels.Stacked(
                StackedExcelModels.BoldText(title),
                ExcelStyleModels.FontSize(fontSize));
            var nameCell = StackedExcelModels.Stacked(
                ExcelValueModels.String(content),
                ExcelStyleModels.FontSize(fontSize));
            return ExcelRowModels.WithEntries(firstCell, nameCell);
        }

        public static ExcelRowModel TitleThenContent(string title, string content)
        {
            var firstCell = ExcelValueModels.RichString(title, true);
            var nameCell = ExcelValueModels.String(content);
            return ExcelRowModels.WithEntries(firstCell, nameCell);
        }

        public static ExcelRowModel TreatmentNameRow(TreatmentDTO dto)
        {
            return SizedTitleThenContent(TreatmentExportStringConstants.TreatmentName, dto.Name, 18);
        }

        public static ExcelRowModel CriteriaRow(TreatmentDTO dto)
        {
            var criteria = dto.CriterionLibrary.MergedCriteriaExpression;
            return TitleThenContent(TreatmentExportStringConstants.Criterion, criteria);
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

        public static ExcelRowModel TreatmentIsUnselectableRow(TreatmentDTO dto)
        {
            var isUnselectable = dto.IsUnselectable;
            return TitleThenContent(TreatmentExportStringConstants.IsTreatmentUnselectable, isUnselectable.ToString());
        }
    }
}
