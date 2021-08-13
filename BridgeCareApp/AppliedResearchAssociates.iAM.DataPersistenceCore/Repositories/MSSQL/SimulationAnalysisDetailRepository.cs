using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationAnalysisDetailRepository : ISimulationAnalysisDetailRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SimulationAnalysisDetailRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void UpsertSimulationAnalysisDetail(SimulationAnalysisDetailDTO dto)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == dto.SimulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            _unitOfWork.Context.Upsert(dto.ToEntity(), dto.SimulationId);
        }

        public SimulationAnalysisDetailDTO GetSimulationAnalysisDetail(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.SimulationAnalysisDetail.Any(_ => _.SimulationId == simulationId))
            {
                return new SimulationAnalysisDetailDTO();
            }

            return _unitOfWork.Context.SimulationAnalysisDetail.Single(_ => _.SimulationId == simulationId).ToDto();
        }
    }
}
