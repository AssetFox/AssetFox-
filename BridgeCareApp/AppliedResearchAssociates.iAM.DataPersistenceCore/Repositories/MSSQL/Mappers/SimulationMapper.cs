using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
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
                Id = dto.Id,
                NetworkId = networkId,
                Name = dto.Name,
                NumberOfYearsOfTreatmentOutlook = 100,
                NoTreatmentBeforeCommittedProjects = dto.NoTreatmentBeforeCommittedProjects,
            };

        public static void CreateSimulation(this SimulationEntity entity, Network network, DateTime lastRun, DateTime lastModifiedDate)
        {
            var simulation = network.AddSimulation();
            simulation.Id = entity.Id;
            simulation.Name = entity.Name;
            simulation.NumberOfYearsOfTreatmentOutlook = entity.NumberOfYearsOfTreatmentOutlook;
            simulation.LastRun = lastRun;
            simulation.LastModifiedDate = lastModifiedDate;
        }

        public static SimulationDTO ToDto(this SimulationEntity entity, UserEntity creator) =>
            new SimulationDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                NetworkId = entity.NetworkId,
                NetworkName = entity.Network.Name,
                Creator = creator?.Username,
                NoTreatmentBeforeCommittedProjects = entity.NoTreatmentBeforeCommittedProjects,
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
