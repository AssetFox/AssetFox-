using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateTreatmentLibrary(string name, Guid simulationId);

        void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId);

        void GetSimulationTreatments(Simulation simulation);

        List<TreatmentLibraryDTO> TreatmentLibrariesWithTreatments();

        void UpsertPermitted(Guid simulationId, TreatmentLibraryDTO dto);

        void UpsertTreatmentLibrary(TreatmentLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId);

        void DeleteTreatmentLibrary(Guid libraryId);
    }
}
