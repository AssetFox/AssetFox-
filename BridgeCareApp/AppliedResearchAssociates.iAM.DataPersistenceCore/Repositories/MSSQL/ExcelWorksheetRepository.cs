using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class ExcelWorksheetRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public ExcelWorksheetRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddExcelWorksheet(ExcelWorksheetDTO dto)
        {

        }
    }
}
