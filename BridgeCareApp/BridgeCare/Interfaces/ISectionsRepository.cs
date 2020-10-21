using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISectionsRepository
    {
        IQueryable<SectionModel> GetSections(int networkId, BridgeCareContext db);

        int GetBrKey(int networkID, int sectionID, BridgeCareContext db);

        int GetSectionId(int networkID, int brKey, BridgeCareContext db);
    }
}
