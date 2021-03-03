using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class SimulationDTO
    {
        [Required]
        public int SimulationId { get; set; }

        public string SimulationName { get; set; }

        public string NetworkName { get; set; }

        public string Owner { get; set; }

        public string Creator { get; set; }

        [Required]
        public int NetworkId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? LastRun { get; set; }

        // TODO: default value for alpha 1
        public List<SimulationUserDTO> Users { get; set; } = new List<SimulationUserDTO>();

        // TODO: will replace int simulationId after alpha 1
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string RunTime { get; set; }

        public SimulationDTO() { }

        public SimulationDTO(SimulationEntity entity)
        {
            Owner = "[ Unknown ]";
            Creator = "[ Unknown ]";
            SimulationId = 1171;
            Id = entity.Id;
            SimulationName = entity.Name;
            NetworkId = 13;
            CreatedDate = entity.CreatedDate;
            LastModifiedDate = entity.LastModifiedDate;
            LastRun = entity.SimulationOutput?.LastModifiedDate;
            NetworkName = "PENNDOT";
        }
    }
}
