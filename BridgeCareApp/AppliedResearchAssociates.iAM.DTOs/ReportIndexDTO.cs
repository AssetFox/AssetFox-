using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class ReportIndexDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public Guid? SimulationId { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
