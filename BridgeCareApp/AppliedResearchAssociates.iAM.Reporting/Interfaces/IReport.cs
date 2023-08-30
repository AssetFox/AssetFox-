using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public interface IReport
    {
        /// <summary>
        /// Unique ID of a specific report
        /// </summary>
        Guid ID { get; set; }

        /// <summary>
        /// ID for the simulation
        /// </summary>
        /// <remarks>
        /// May be null if not attached to a specific simulation
        /// </remarks>
        Guid? SimulationID { get; set; }

        /// <summary>
        /// Runs the report, populating all read only fields
        /// </summary>
        ///<param name="scenarioId"></param>
        ///<param name="criteria"></param>
        ///<param name="cancellationToken"></param>
        ///<param name="workQueueLog"></param>
        Task Run(string scenarioId, string criteria = null, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null);

        /// <summary>
        /// Report results in format specified by ReportType
        /// </summary>
        /// <remarks>
        /// Populated after the Run method is successfully invoked
        /// </remarks>
        string Results { get; }

        /// <summary>
        /// Report suffix for deciding whether to use raw or primary data network
        /// </summary>
        /// <remarks>
        /// </remarks>
        string Suffix { get; }


        /// <summary>
        /// Describes the format of the results field
        /// </summary>
        ReportType Type { get; }

        /// <summary>
        /// String used by factory to generate report
        /// </summary>
        string ReportTypeName { get; }

        /// <summary>
        /// List of errors that occured during the invocation of the Run method
        /// </summary>
        List<string> Errors { get; }

        /// <summary>
        /// Indicates if the report is compelte
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Status message to be shown in UI
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Report criteria to filter report output based on criteria compatible assets
        /// </summary>
        string Criteria { get; set; }
    }
}
