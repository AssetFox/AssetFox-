using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class ReportIndexRepository : IReportIndexRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public ReportIndexRepository(UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public bool Add(ReportIndexDTO dto)
        {
            var entity = dto.ToEntity();
            var returnValue = Add(entity);
            return returnValue;
        }

        private bool Add(ReportIndexEntity report)
        {
            // Ensure required fields are present
            if (report.Id == Guid.Empty || string.IsNullOrEmpty(report.ReportTypeName))
            {
                throw new ArgumentException("Report does not have required values");
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
            _unitOfDataPersistenceWork.Context.AddEntity(report, _unitOfDataPersistenceWork.UserEntity?.Id);

            return true;
        }


public List<ReportListItem> GetAllReportsInSystem()
{
    var scenarios = GetAllScenario();
    var reportList = new List<ReportListItem>();

    foreach (var scenario in scenarios)
    {
        var reports = _unitOfDataPersistenceWork.ReportIndexRepository.GetAllForScenario(scenario.Id);

        foreach (var report in reports)
        {
            var listEntry = new ReportListItem
            {
                ReportId = report.Id,
                ReportName = report.Type
            };
            reportList.Add(listEntry);
        }
    }

    return reportList;
}

public List<SimulationDTO> GetAllScenario()
{
    if (!_unitOfDataPersistenceWork.Context.Simulation.Any())
    {
        return new List<SimulationDTO>();
    }

    var users = _unitOfDataPersistenceWork.Context.User.ToList();

    var simulationEntities = _unitOfDataPersistenceWork.Context.Simulation
        .Include(_ => _.SimulationAnalysisDetail)
        .Include(_ => _.SimulationReportDetail)
        .Include(_ => _.SimulationUserJoins)
        .ThenInclude(_ => _.User)
        .Include(_ => _.Network)
        .ToList();

    return simulationEntities.Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
        .ToList();
}

        public bool DeleteAllSimulationReports(Guid simulationId)
        {
            var scenarioReports = _unitOfDataPersistenceWork.Context.ReportIndex.Where(_ => _.SimulationID == simulationId);
            if (scenarioReports.Count() > 0)
            {                
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

        public class ReportListItem
        {
            public Guid ReportId { get; set; }
            public string ReportName { get; set; }
        }

        public ReportIndexDTO Get(Guid reportId) => _unitOfDataPersistenceWork.Context.ReportIndex.FirstOrDefault(_ => _.Id == reportId).ToDTONullPropagating();
        public List<ReportIndexDTO> GetAllForScenario(Guid simulationId) =>
            _unitOfDataPersistenceWork.Context.ReportIndex.Where(_ => _.SimulationID == simulationId).Select(_ => _.ToDTO()).ToList();
    }
}
