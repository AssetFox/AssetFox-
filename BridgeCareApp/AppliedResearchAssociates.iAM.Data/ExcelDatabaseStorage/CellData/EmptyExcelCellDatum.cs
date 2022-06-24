using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData
{
    public class EmptyExcelCellDatum: IExcelCellDatum
    {
        public T Accept<T, THelper>(IExcelCellDatumVisitor<THelper, T> visitor, THelper helper)
        {
            var returnValue = visitor.Visit(this, helper);
            return returnValue;
        }
    }
}
