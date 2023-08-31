using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<IReport> Generate(string reportName, string suffix = "");

        Task<IReport> GenerateInventoryReport(string reportName);

        /// <summary>
        /// Lists the names of all reports in a given scenario
        /// </summary>
        /// <param name="simulationId"></param>
        /// <returns></returns>
        List<ReportListItem> GetAllReportsForScenario(Guid simulationId);

        /// <summary>
        /// Recreate a persisted report
        /// </summary>
        /// <param name="reportId">GUID ID of persisted report</param>
        /// <returns>A report object previously persisted</returns>
        Task<IReport> GetExisting(Guid reportId);

        /// <summary>
        /// Removes old reports from the repository (if any exist)
        /// </summary>
        void Cleanup();
    }
}
