﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Helpers.Excel.Visitors;

namespace BridgeCareCore.Helpers.Excel
{
    public class ExcelFontSizeModel : IExcelModel
    {
        public float FontSize { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
