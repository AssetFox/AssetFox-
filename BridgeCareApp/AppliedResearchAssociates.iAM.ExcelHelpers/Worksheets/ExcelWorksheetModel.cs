using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelWorksheetModel
    {
        public string TabName { get; set; }
        public List<IExcelWorksheetContentModel> Content { get; set; }
    }
}
