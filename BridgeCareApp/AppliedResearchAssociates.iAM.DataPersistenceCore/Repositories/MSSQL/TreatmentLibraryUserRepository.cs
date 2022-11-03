using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentLibraryUserRepository : ITreatmentLibraryUserRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public TreatmentLibraryUserRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                          throw new ArgumentNullException(nameof(unitOfWork));
        public void UpsertTreatmentLibraryUser(TreatmentLibraryDTO dto, Guid userId) => _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, userId);

        public List<TreatmentLibraryUserDTO> GetAllTreatmentLibraryUsers()
        {
            if (!_unitOfWork.Context.TreatmentLibraryUser.Any())
            {
                return new List<TreatmentLibraryUserDTO>();
            }

            return _unitOfWork.Context.TreatmentLibraryUser.AsNoTracking()
                .Include(_ => _.TreatmentLibrary)
                .OrderBy(_ => _.UserId)
                .Select(_ => _.ToDto())
                .ToList();
        }
    }
}
