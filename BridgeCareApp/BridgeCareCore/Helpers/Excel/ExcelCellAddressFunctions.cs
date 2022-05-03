using OfficeOpenXml;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/ExcelCellAddressFunctions.cs
namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelCellAddressFunctions.cs
{
    public static class ExcelCellAddressFunctions
    {
        public static ExcelCellAddress Offset(ExcelCellAddress address, int columnDelta, int rowDelta)
            => new ExcelCellAddress(address.Row + rowDelta, address.Column + columnDelta);

        public static ExcelCellAddress Left(ExcelCellAddress address)
            => new ExcelCellAddress(address.Row, address.Column - 1);

        public static ExcelCellAddress Up(ExcelCellAddress address)
            => new ExcelCellAddress(address.Row - 1, address.Column);
    }
}
