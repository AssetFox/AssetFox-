using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICalculatedAttributesRepository
    {
        void CreateScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid simulationId);

        void GetScenarioCalculatedAttributes(Simulation simulation);

        ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId);

        ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries();

        void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library);

        void UpsertOrDeleteCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId);

        void DeleteCalculatedAttributeLibrary(Guid libraryId);

        void UpsertOrDeleteScenarioCalculatdAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId);
    }
}
