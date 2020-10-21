using BridgeCare.DataAccessLayer;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IDetailedReportRepository
    {
        List<YearlyDataModel> GetYearsData(SimulationModel data);

        IQueryable<DetailedReportRepository> GetRawQuery(SimulationModel data, BridgeCareContext db);
    }
}
