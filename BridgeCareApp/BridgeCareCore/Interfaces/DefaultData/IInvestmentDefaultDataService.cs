using System.Threading.Tasks;
using BridgeCareCore.Models.DefaultData;

namespace BridgeCareCore.Interfaces.DefaultData
{
    public interface IInvestmentDefaultDataService
    {
        Task<InvestmentDefaultData> GetInvestmentDefaultData();
    }
}
