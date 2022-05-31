using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
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
