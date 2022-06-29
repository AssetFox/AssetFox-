using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.Data.Helpers;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class IExcelCellDatumExtensions
    {
        public static object ObjectValue(this IExcelCellDatum datum)
        {
            var visitor = ExcelCellDatumValueGetter.Instance;
            var returnValue = datum.Accept(visitor, Unit.Default);
            return returnValue;
        }
    }
}
