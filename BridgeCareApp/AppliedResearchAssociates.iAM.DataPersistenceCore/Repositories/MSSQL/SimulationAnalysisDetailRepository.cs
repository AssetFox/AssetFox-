using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationAnalysisDetailRepository : MSSQLRepository, ISimulationAnalysisDetailRepository
    {
        public SimulationAnalysisDetailRepository(IAMContext context) : base(context)
        {
        }

        public void UpsertSimulationAnalysisDetail(SimulationAnalysisDetailDTO dto)
        {
            if (!Context.Simulation.Any(_ => _.Id == dto.SimulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.SimulationId}.");
            }

            Context.AddOrUpdate(dto.ToEntity(), dto.SimulationId);

            Context.SaveChanges();
        }

        public SimulationAnalysisDetailDTO GetSimulationAnalysisDetail(Guid simulationId)
        {
            if (!Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!Context.SimulationAnalysisDetail.Any(_ => _.SimulationId == simulationId))
            {
                return new SimulationAnalysisDetailDTO();
            }

            return Context.SimulationAnalysisDetail.Single(_ => _.SimulationId == simulationId).ToDto();
        }
    }
}
