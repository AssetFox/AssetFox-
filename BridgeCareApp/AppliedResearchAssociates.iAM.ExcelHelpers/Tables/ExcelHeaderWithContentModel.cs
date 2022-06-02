using System;

namespace AppliedResearchAssociates.iAM.ExcelHelpers.Tables
{
    public class ExcelHeaderWithContentModel<TData>
    {
        public IExcelModel Header { get; set; }
        public Func<TData, IExcelModel> Content { get; set; }   
    }
}
