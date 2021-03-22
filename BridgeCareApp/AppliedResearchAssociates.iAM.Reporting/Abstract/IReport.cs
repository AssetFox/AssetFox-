﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="parameters">JSON representation of data to be projected in report</param>
        Task Run(string parameters);

        /// <summary>
        /// Report results in format specified by ReportType
        /// </summary>
        /// <remarks>
        /// Populated after the Run method is successfully invoked
        /// </remarks>
        string Results { get; set; }

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
    }
}
