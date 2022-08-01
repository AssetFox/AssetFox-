using System;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class AttributeMetaDatum
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public string DefaultValue { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        public string ConnectionString { get; set; }

        public string DataSource { get; set; }

        public string Type { get; set; }

        public string Location { get; set; }

        public string Command { get; set; }

        public string AggregationRule { get; set; }

        public bool IsCalculated { get; set; }

        public bool IsAscending { get; set; }

        public ConnectionType ConnectionType { get; set; }

        /*      "Id": "6a473634-ce64-4cae-acda-7306a2495454",
      "Name": "ADTTOTAL",
      "DefaultValue": "1000",
      "Minimum": 0.0,
      "ConnectionString": "data source=iAM-Test-2\\sql2014;initial catalog=PennDOT_2021_Bridge_Data;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework",
      "DataSource": "MSSQL",
      "Type": "NUMBER",
      "Location": "section",
      "Command": "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",
      "AggregationRule": "AVERAGE",
      "IsCalculated": false,
      "IsAscending": true*/
    }
}
