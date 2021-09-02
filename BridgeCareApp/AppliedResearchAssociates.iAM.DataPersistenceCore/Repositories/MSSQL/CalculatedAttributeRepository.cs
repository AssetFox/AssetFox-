using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CalculatedAttributeRepository : ICalculatedAttributesRepository
    {
        public void DeleteCalculatedAttributeLibrary(Guid libraryId) => throw new NotImplementedException();
        public ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries() => throw new NotImplementedException();
        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) => throw new NotImplementedException();
        public void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library) => throw new NotImplementedException();
        public void UpsertOrDeleteCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId) => throw new NotImplementedException();
        public void UpsertOrDeleteScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId) => throw new NotImplementedException();
    }
}
