using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CompleteSimulationMapper
    {
        public static SimulationEntity ToNewEntity(this CompleteSimulationDTO dto)
        {
            var analysisMethod = AnalysisMethodMapper.ToEntity(dto.AnalysisMethod, dto.Id);
            var entity = new SimulationEntity
            {
                Name = dto.Name,
                Id = dto.Id,
                NetworkId = dto.NetworkId,
                
            };


            return entity;
        }

    }
}
