using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;

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
    }
}
