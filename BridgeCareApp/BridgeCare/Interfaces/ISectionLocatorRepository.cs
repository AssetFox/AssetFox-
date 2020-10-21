using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISectionLocatorRepository
    {
        SectionLocationModel Locate(SectionModel section, BridgeCareContext db);
    }
}
