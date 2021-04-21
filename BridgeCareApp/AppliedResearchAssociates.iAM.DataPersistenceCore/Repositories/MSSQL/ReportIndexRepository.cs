using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class ReportIndexRepository : IReportIndexRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public ReportIndexRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public bool Add(ReportIndexEntity report)
        {
            // Ensure required fields are present
            if (report.Id == Guid.Empty || String.IsNullOrEmpty(report.ReportTypeName))
            {
                throw new ArgumentException($"Report does not have required values");
            }

            // Remove the old report if it exists
            if (_unitOfDataPersistenceWork.Context.ReportIndex.Any(_ => _.Id == report.Id))
            {
                var oldReport = _unitOfDataPersistenceWork.Context.ReportIndex.FirstOrDefault(_ => _.Id == report.Id);
                if (oldReport != null)
                {
                    _unitOfDataPersistenceWork.Context.DeleteEntity<ReportIndexEntity>(_ => _.Id == oldReport.Id);
                }
            }

            // Add the new report
            _unitOfDataPersistenceWork.Context.AddEntity(report, report.Id);

            return true;
        }

        public bool DeleteAllSimulationReports(Guid simulationId)
        {
            var scenarioReports = _unitOfDataPersistenceWork.Context.ReportIndex.Where(_ => _.SimulationID == simulationId);
            if (scenarioReports.Count() > 0)
            {
                //_unitOfDataPersistenceWork.Context.ReportIndex.RemoveRange(scenarioReports);
                _unitOfDataPersistenceWork.Context.DeleteAll<ReportIndexEntity>(_ => _.SimulationID == simulationId);
                return true;
            }
            return false;
        }


        public bool DeleteExpiredReports()
        {
            _unitOfDataPersistenceWork.Context.DeleteAll<ReportIndexEntity>(_ => _.ExpirationDate < DateTime.Now);
            return true;
        }

        public bool DeleteReport(Guid reportId)
        {
            _unitOfDataPersistenceWork.Context.DeleteEntity<ReportIndexEntity>(_ => _.Id == reportId);
            return true;
        }

        public ReportIndexEntity Get(Guid reportId) => _unitOfDataPersistenceWork.Context.ReportIndex.FirstOrDefault(_ => _.Id == reportId);
        public List<ReportIndexEntity> GetAllForScenario(Guid simulationId) => _unitOfDataPersistenceWork.Context.ReportIndex.Where(_ => _.SimulationID == simulationId).ToList();
    }
}
