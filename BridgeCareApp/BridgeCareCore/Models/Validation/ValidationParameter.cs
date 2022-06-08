using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models.Validation
{
    public class ValidationParameter
    {
        public string Expression { get; set; }

        public UserCriteriaDTO CurrentUserCriteriaFilter { get; set; }

        public Guid NetworkId { get; set; }
    }
}
