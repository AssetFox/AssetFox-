using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CrudDTO
    {
        public CrudDTO()
        {
            matched = false;
        }

        [IgnoreDataMember]
        public bool matched { get; set; }
    }
}
