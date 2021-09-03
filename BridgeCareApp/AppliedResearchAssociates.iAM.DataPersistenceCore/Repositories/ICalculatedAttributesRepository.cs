using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICalculatedAttributesRepository
    {
        // Libraries
        ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries();

        void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library);

        void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId);

        void DeleteCalculatedAttributeFromLibrary(Guid libraryId, Guid calculatedAttributeId);

        void DeleteCalculatedAttributeLibrary(Guid libraryId);

        // Scenarios
        ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId);      

        void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId);

        void DeleteCalculatedAttributeFromScenario(Guid scenarioId, Guid calculatedAttributeId);

        void ClearCalculatedAttributes(Guid scenarioId);
    }
}
