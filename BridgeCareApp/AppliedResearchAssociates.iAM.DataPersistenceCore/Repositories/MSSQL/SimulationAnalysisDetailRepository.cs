using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationAnalysisDetailRepository : ISimulationAnalysisDetailRepository
    {
        private readonly IAMContext _context;

        public SimulationAnalysisDetailRepository(IAMContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public void UpsertSimulationAnalysisDetail(SimulationAnalysisDetailDTO dto)
        {
            if (!_context.Simulation.Any(_ => _.Id == dto.SimulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.SimulationId}.");
            }

            _context.AddOrUpdate(dto.ToEntity(), dto.SimulationId);
        }

        public SimulationAnalysisDetailDTO GetSimulationAnalysisDetail(Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_context.SimulationAnalysisDetail.Any(_ => _.SimulationId == simulationId))
            {
                return new SimulationAnalysisDetailDTO();
            }

            return _context.SimulationAnalysisDetail.Single(_ => _.SimulationId == simulationId).ToDto();
        }
    }
}
