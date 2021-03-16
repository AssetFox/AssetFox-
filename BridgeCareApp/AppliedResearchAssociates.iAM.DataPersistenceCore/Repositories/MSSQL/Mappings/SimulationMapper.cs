using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

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

        public static SimulationEntity ToEntity(this SimulationDTO dto, Guid networkId) =>
            new SimulationEntity
            {
                Id = dto.Id, NetworkId = networkId, Name = dto.Name, NumberOfYearsOfTreatmentOutlook = 100
            };

        public static void CreateSimulation(this SimulationEntity entity, Network network)
        {
            var simulation = network.AddSimulation();
            simulation.Id = entity.Id;
            simulation.Name = entity.Name;
            simulation.NumberOfYearsOfTreatmentOutlook = entity.NumberOfYearsOfTreatmentOutlook;
        }

        public static SimulationDTO ToDto(this SimulationEntity entity, UserEntity creator) =>
            new SimulationDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Creator = creator?.Username,
                Owner = entity.SimulationUserJoins?.FirstOrDefault(_ => _.IsOwner)?.User?.Username,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                LastRun = entity.SimulationAnalysisDetail?.LastRun,
                RunTime = entity.SimulationAnalysisDetail?.RunTime,
                Status = entity.SimulationAnalysisDetail?.Status,
                ReportStatus = entity.SimulationReportDetail?.Status,
                Users = entity.SimulationUserJoins.Any()
                    ? entity.SimulationUserJoins.Select(_ => _.ToDto()).ToList()
                    : new List<SimulationUserDTO>()
            };


    }
}
