using System.Collections.Generic;
using Microsoft.Graph.Models;

namespace BridgeCareCore.Services.SummaryReport.CommittedProjects
{
    public class CommittedProjectsColumnHeaders
    {
        public const string BRKey = "BRKEY_";
        public const string BMSID = "BMSID";
        public const string CRS = "CRS";
        public const string Treatment = "TREATMENT";
        public const string Year = "YEAR";
        public const string Budget = "BUDGET";
        public const string Cost = "COST";
        public const string ProjectSource = "PROJECTSOURCE";
        public const string ProjectSourceId = "PROJECTSOURCEID";
        public const string Category = "CATEGORY";

        public static readonly List<string> AllHeaders = new()
        {
            BRKey,
            BMSID,
            CRS,
            Treatment,
            Year,
            Budget,
            Cost,
            ProjectSource,
            ProjectSourceId,
            Category
        };

        public static readonly List<string> NoKeyHeaders = new()
        {
            Treatment,
            Year,
            Budget,
            Cost,
            ProjectSource,
            ProjectSourceId,
            Category
        };

        public static readonly List<string> BridgeKeyHeaders = new()
        {
            BRKey,
            BMSID,
        };

        public static readonly List<string> RoadKeyHeaders = new()
        {
            CRS
        };
    }
}
