using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class ExcelWorksheetRepository : IExcelWorksheetRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public ExcelWorksheetRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Guid AddExcelWorksheet(ExcelSpreadsheetDTO dto)
        {
            var entity = dto.ToEntity();
            var userId = _unitOfWork.UserEntity?.Id;
            var entities = new List<ExcelWorksheetEntity> { entity };
            _unitOfWork.Context.AddAll(entities, userId);
            return entity.Id;
        }
    }
}
