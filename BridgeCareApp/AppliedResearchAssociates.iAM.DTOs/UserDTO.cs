using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class UserDTO : BaseDTO
    {
        public string Username { get; set; }

        public bool HasInventoryAccess { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
