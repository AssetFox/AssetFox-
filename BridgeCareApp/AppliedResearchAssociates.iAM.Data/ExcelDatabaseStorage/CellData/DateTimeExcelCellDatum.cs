﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public class DateTimeExcelCellDatum : IExcelCellDatum
    {
        public DateTime Value { get; set; }

        public T Accept<T, THelper>(IExcelCellDatumVisitor<THelper, T> visitor, THelper helper)
        {
            var returnValue = visitor.Visit(this, helper);
            return returnValue;
        }
    }
}
