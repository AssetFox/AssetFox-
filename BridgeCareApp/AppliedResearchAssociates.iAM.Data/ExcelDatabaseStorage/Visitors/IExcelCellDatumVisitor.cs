﻿using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    /// <summary>THelper is the type of any helper object one may want to use.
    /// T is whatever is output by the visitor.</summary>
    public interface IExcelCellDatumVisitor<THelper, TOutput>
    {
        TOutput Visit(StringExcelCellDatum datum, THelper helper);
        TOutput Visit(DoubleExcelCellDatum datum, THelper helper);
        TOutput Visit(DateTimeExcelCellDatum datum, THelper helper);
        TOutput Visit(EmptyExcelCellDatum datum, THelper helper);
    }
}
