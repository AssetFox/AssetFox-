﻿using BridgeCareCore.Services.SummaryReport.Visitors;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelSingleBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public RectangleEdge Edge { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
