using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateTreatmentLibrary(string name, Guid simulationId);

        void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId);

        void GetSimulationTreatments(Simulation simulation);

        Task<List<TreatmentLibraryDTO>> TreatmentLibrariesWithTreatments();

        void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TreatmentLibraryDTO dto);

        void UpsertTreatmentLibrary(TreatmentLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo);

        void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId, UserInfoDTO userInfo);

        void DeleteTreatmentLibrary(Guid libraryId);
    }
}
