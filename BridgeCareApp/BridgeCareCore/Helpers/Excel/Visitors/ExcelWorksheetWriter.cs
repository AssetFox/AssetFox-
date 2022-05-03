using System.Collections.Generic;
using OfficeOpenXml;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Services/BAMSSummaryReport/Visitors/ExcelWorksheetWriter.cs
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Visitors;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Visitors
========
namespace BridgeCareCore.Helpers.Excel.Visitors
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/Visitors/ExcelWorksheetWriter.cs
{
    public class ExcelWorksheetWriter: IExcelWorksheetModelVisitor<ExcelWorksheet, ExcelWorksheet>
    {
        public ExcelWorksheet Visit(AnchoredExcelRegionModel model, ExcelWorksheet worksheet)
        {
            var writer = new ExcelWriter();
            writer.WriteRegion(worksheet, model.Region, model.StartRow, model.StartColumn);
            return worksheet;
        }

        public ExcelWorksheet Visit(AutoFitColumnsExcelWorksheetContentModel model, ExcelWorksheet worksheet)
        {
            worksheet.Cells.AutoFitColumns(model.MinWidth);
            return worksheet;
        }

        public ExcelWorksheet Visit(SpecificColumnWidthChangeExcelWorksheetModel model, ExcelWorksheet worksheet)
        {
            var column = worksheet.Column(model.ColumnNumber);
            var oldWidth = column.Width;
            var newWidth = model.WidthChange(oldWidth);
            column.Width = newWidth;
            return worksheet;
        }

        public static ExcelWorksheet VisitList(ExcelWorksheet worksheet, List<IExcelWorksheetContentModel> contents)
        {
            var writer = new ExcelWorksheetWriter();
            foreach (var content in contents)
            {
                content.Accept(writer, worksheet);
            }
            return worksheet;
        }
    }
}
