using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public static class TestServices
    {
        public static ExcelRawDataImportService CreateExcelSpreadsheetImportService(UnitOfDataPersistenceWork unitOfWork)
        {
            var returnValue = new ExcelRawDataImportService(unitOfWork);
            return returnValue;
        }
    }
}
