using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AttributeDtos
    {
        public static AttributeDTO BrActionType
            = new()
            {
                Id = Guid.Parse("85e2b431-05ec-4ea9-92cd-4d663d657262"),
                Name = "ACTIONTYPE",
                DefaultValue = "0",
                Minimum = 100.0,
                Maximum = 0.0,
                Type = "STRING",
                Command = "SELECT CAST(PennDot_Report_A.BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(PennDot_Report_A.BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, (ActionCode) AS DATA_ FROM dbo.PennDot_Report_A,PENNDOT_ActionItems WHERE dbo.PennDot_Report_A.BRKEY=PENNDOT_ActionItems.BRKEY group by dbo.PennDot_Report_A.BRKEY,BRIDGE_ID,CAST(INSPDATE AS DATETIME),ActionCode",
                AggregationRuleType = "PREDOMINANT",
                IsCalculated = false,
                IsAscending = true
            };
            
    }
}
