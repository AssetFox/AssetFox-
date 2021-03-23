using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class ReportIndexRepository : IReportIndexRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public ReportIndexRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public bool Add(ReportIndex report)
        {
            // Ensure required fields are present
            if (report.ID == null || String.IsNullOrEmpty(report.ReportTypeName))
            {
                throw new ArgumentException($"Report does not have required values");
            }

            // Remove the old report if it exists
            if (_unitOfDataPersistenceWork.Context.ReportIndex.Any(_ => _.ID == report.ID))
            {
                var oldReport = _unitOfDataPersistenceWork.Context.ReportIndex.FirstOrDefault(_ => _.ID == report.ID);
                if (oldReport != null)
                {
                    _unitOfDataPersistenceWork.Context.ReportIndex.Remove(oldReport);
                    _unitOfDataPersistenceWork.Context.SaveChanges();
                }
            }

            // Add the new report
            _unitOfDataPersistenceWork.Context.ReportIndex.Add(report);
            _unitOfDataPersistenceWork.Context.SaveChanges();

            return true;
        }

        public bool DeleteAllScenarioReports(Guid scenarioId)
        {
            var scenarioReports = _unitOfDataPersistenceWork.Context.ReportIndex.Where(_ => _.SimulationID == scenarioId);
            if (scenarioReports != null)
            {
                _unitOfDataPersistenceWork.Context.ReportIndex.RemoveRange(scenarioReports);
                _unitOfDataPersistenceWork.Context.SaveChanges();
                return true;
            }
            return false;
        }


        public bool DeleteExpiredReports()
        {
            var expiredReports = _unitOfDataPersistenceWork.Context.ReportIndex.Where(_ => _.ExpirationDate < DateTime.Now);
            if (expiredReports != null)
            {
                _unitOfDataPersistenceWork.Context.ReportIndex.RemoveRange(expiredReports);
                _unitOfDataPersistenceWork.Context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool DeleteReport(Guid reportId)
        {
            var specificReport = _unitOfDataPersistenceWork.Context.ReportIndex.FirstOrDefault(_ => _.ID == reportId);
            if (specificReport != null)
            {
                _unitOfDataPersistenceWork.Context.ReportIndex.Remove(specificReport);
                _unitOfDataPersistenceWork.Context.SaveChanges();
                return true;
            }

            return false;
        }

        public ReportIndex Get(Guid reportId) => _unitOfDataPersistenceWork.Context.ReportIndex.FirstOrDefault(_ => _.ID == reportId);
    }
}
