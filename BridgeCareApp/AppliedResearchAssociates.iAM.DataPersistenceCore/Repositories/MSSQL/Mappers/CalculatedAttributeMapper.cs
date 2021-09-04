using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CalculatedAttributeMapper
    {
        public static CalculatedAttributeLibraryDTO ToDto(this CalculatedAttributeLibraryEntity entity)
        {
            throw new NotImplementedException();
        }

        public static CalculatedAttributeDTO ToDto(this ScenarioCalculatedAttributeEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
