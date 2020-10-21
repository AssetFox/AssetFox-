using BridgeCare.Models;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IRunRollupRepository
    {
        void SetLastRunDate(int networkId, BridgeCareContext db);
        Task<string> RunRollup(SimulationModel model);
    }
}
