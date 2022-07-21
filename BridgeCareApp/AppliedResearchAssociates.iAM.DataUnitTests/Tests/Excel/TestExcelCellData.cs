using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Excel
{
    public static class TestExcelCellData
    {
        public static StringExcelCellDatum HelloDatum()
            => ExcelCellData.String("hello");

        public static StringExcelCellDatum HelloCommaWorldDatum()
            => ExcelCellData.String("hello, world!");

        public static StringExcelCellDatum PunctuationDatum()
            => ExcelCellData.String(@"!@#$%^&*()""''[]{}.,;<>~`+_-=?/");

        public static StringExcelCellDatum DeltaDatum()
            => ExcelCellData.String("Delta");

        public static DoubleExcelCellDatum PiDatum()
            => ExcelCellData.Double(Math.PI);
    }
}
