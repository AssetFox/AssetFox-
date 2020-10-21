using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IBudgetReportRepository
    {
        YearlyBudgetAndCost GetData(SimulationModel data, string[] budgetTypes);

        string[] InvestmentData(SimulationModel data);
    }
}
