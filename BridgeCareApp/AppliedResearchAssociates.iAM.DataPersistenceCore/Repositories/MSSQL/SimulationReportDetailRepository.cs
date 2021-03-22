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
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SimulationReportDetailRepository(UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void UpsertSimulationReportDetail(SimulationReportDetailDTO dto, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == dto.SimulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.SimulationId}.");
            }

            var userEntity = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            _unitOfDataPersistenceWork.Context.Upsert(dto.ToEntity(), _ => _.SimulationId == dto.SimulationId, userEntity?.Id);
        }
    }
}
