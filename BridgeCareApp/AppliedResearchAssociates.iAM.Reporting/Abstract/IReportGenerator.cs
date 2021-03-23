﻿using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Reporting
{
    /// <summary>
    /// Creates objects that run specific reports
    /// </summary>
    public interface IReportGenerator
    {
        /// <summary>
        /// Generate a specific report object
        /// </summary>
        /// <param name="reportName">Name of the object to create</param>
        /// <returns>A report object based on the provided name</returns>
        Task<IReport> Generate(string reportName);

        /// <summary>
        /// Removes old reports from the repository (if any exist)
        /// </summary>
        void Cleanup();
    }
}
