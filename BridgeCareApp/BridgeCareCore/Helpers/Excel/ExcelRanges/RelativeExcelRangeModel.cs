using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/ExcelRanges/RelativeExcelRangeModel.cs
namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/RelativeExcelRangeModel.cs
{
    /// <summary>For modelling situations where we know the content
    /// and size of our range but not its location.</summary>
    public class RelativeExcelRangeModel
    {
        public IExcelModel Content { get; set; }
        public ExcelRangeSize Size { get; set; } = new ExcelRangeSize();
    }
}
