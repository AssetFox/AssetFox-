using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelNumberFormatModel: IExcelModel
    {
        public string Format { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
