using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Represents information about a report that has been generated for a specific simulation
    /// </summary>
    public class ReportIndexDTO : BaseDTO
    {
        /// <summary>
        /// The simulation that provides data for this report
        /// </summary>
        public Guid? SimulationId { get; set; }

        /// <summary>
        /// The name of the report definition used for this report
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The results of the report.  For HTML reports, this contains the
        /// actual HTML markup.  For file reports, this contains the location
        /// of the file containing the results.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Date when the result expires
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Date when the report was run
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
