using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentLibraryUserRepository
    {
        void UpsertTreatmentLibraryUser(TreatmentLibraryDTO dto, Guid userId);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);
        List<TreatmentLibraryUserDTO> GetAllTreatmentLibraryUsers();
        void UpsertOrDeleteUsers(Guid treatmentLibraryId, IList<TreatmentLibraryUserDTO> libraryUsers);
        List<TreatmentLibraryUserDTO> GetLibararyUsers(Guid treatmentLibraryId);
    }
}
