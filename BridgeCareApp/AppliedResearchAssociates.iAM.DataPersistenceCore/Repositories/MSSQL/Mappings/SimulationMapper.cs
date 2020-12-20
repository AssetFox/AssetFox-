using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class SimulationMapper
    {
        public static SimulationEntity ToEntity(this Simulation domain) =>
            new SimulationEntity
            {
                Id = domain.Id,
                NetworkId = domain.Network.Id,
                Name = domain.Name,
                NumberOfYearsOfTreatmentOutlook = domain.NumberOfYearsOfTreatmentOutlook
            };

        public static void CreateSimulation(this SimulationEntity entity, Network network)
        {
            var simulation = network.AddSimulation();
            simulation.Id = entity.Id;
            simulation.Name = entity.Name;
            simulation.NumberOfYearsOfTreatmentOutlook = entity.NumberOfYearsOfTreatmentOutlook;
        }

        public static SimulationDTO ToDto(this SimulationEntity entity)
        {
            var dto = new SimulationDTO(entity);
            if (entity.SimulationAnalysisDetail != null)
            {
                dto.LastRun = entity.SimulationAnalysisDetail.LastRun;
                dto.RunTime = entity.SimulationAnalysisDetail.RunTime;
                dto.Status = entity.SimulationAnalysisDetail.Status;
            }
            return dto;
        }
    }
}
