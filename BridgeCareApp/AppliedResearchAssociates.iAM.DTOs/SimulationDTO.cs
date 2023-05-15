using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Serialization-friendly aggregate of common values of a simulation.
    /// </summary>
    public class SimulationDTO : BaseDTO
    {
        /// <summary>
        /// The name of the simulation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the network the simulation is in.
        /// </summary>
        public Guid NetworkId { get; set;}

        /// <summary>
        /// The name of the network the simulation is in.
        /// </summary>
        public string NetworkName { get; set; }

        /// <summary>
        /// The user who owns the simulation.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The user who created the simulation.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Verifies the simulation has no treatments.
        /// </summary>
        public bool NoTreatmentBeforeCommittedProjects { get; set; }

        /// <summary>
        /// The timestamp of the simulation creation.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// The timestamp of the latest simulation update.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// The timestamp of the latest simulation run.
        /// </summary>
        public DateTime? LastRun { get; set; }

        /// <summary>
        /// A list of the users who can run the simulation.
        /// </summary>
        public List<SimulationUserDTO> Users { get; set; } = new List<SimulationUserDTO>();

        /// <summary>
        /// The current status of the simulation -- Running / Failed / Complete.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The current status of the simulation analysis -- Generating / Failed / Complete.
        /// </summary>
        public string ReportStatus { get; set; }

        /// <summary>
        /// The amount of time it takes for the simulation to run.
        /// </summary>
        public string RunTime { get; set; }
    }
}
