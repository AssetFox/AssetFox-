using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IRemainingLifeLimitRepository
    {
        RemainingLifeLimitLibraryModel GetSimulationRemainingLifeLimitLibrary(int id, BridgeCareContext db);
        RemainingLifeLimitLibraryModel SaveSimulationRemainingLifeLimitLibrary(RemainingLifeLimitLibraryModel model, BridgeCareContext db);
    }
}
