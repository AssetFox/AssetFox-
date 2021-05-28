using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationLogRepository : ISimulationLogRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public SimulationLogRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
           _unitOfWork = unitOfWork ??
                                 throw new ArgumentNullException(nameof(unitOfWork));

        public void ClearLog(Guid simulationId)
        {
            _unitOfWork.Context.DeleteAll<SimulationLogEntity>(_ => _.SimulationId == simulationId);
        }

        public void CreateLogs(IList<SimulationLogDTO> dtos)
        {
            var entities = new List<SimulationLogEntity>();
            foreach (var dto in dtos)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == dto.SimulationId))
                {
                    throw new RowNotInTableException($"No simulation with id {dto.SimulationId}");
                }
                var entity = dto.ToEntity();
                entities.Add(entity);
            }
            _unitOfWork.Context.AddAll(entities);
        }

        public Task<List<SimulationLogDTO>> GetLog(Guid simulationId)
        {
            var r = _unitOfWork.Context.SimulationLog.Where(
                sl => sl.SimulationId == simulationId)
                .OrderBy(sl => sl.CreatedDate)
                .Select(SimulationLogMapper.ToDTO)
                .ToList();
            return Task.FromResult(r);
        }
    }
}
