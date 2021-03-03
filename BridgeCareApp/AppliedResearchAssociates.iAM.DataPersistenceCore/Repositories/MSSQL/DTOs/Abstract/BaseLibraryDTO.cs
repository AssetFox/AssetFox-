using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract
{
    public abstract class BaseLibraryDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Guid> AppliedScenarioIds { get; set; } = new List<Guid>();
    }
}
