using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationReportDetailRepository : ISimulationReportDetailRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SimulationReportDetailRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void UpsertSimulationReportDetail(SimulationReportDetailDTO dto)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == dto.SimulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.SimulationId}.");
            }

            _unitOfWork.Context.Upsert(dto.ToEntity(), _ => _.SimulationId == dto.SimulationId,
                _unitOfWork.UserEntity?.Id);
        }
    }
}
