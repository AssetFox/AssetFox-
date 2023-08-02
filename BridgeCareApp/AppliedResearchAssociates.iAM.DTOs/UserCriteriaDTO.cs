using System;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class UserCriteriaDTO
    {
        public Guid CriteriaId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Criteria { get; set; }
        public bool HasCriteria { get; set; }
        public bool HasAccess { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
