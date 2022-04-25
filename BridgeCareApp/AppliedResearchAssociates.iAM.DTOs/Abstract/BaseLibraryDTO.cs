using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public abstract class BaseLibraryDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Guid> AppliedScenarioIds { get; set; } = new List<Guid>();

        public Guid Owner { get; set; }

        public bool IsShared { get; set; }
    }
}
