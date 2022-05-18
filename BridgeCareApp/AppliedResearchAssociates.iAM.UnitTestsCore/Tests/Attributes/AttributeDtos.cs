using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AttributeDtos
    {
        public static AttributeDTO ActionType
            => new()
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
        public static AttributeDTO AdtTotal
            => new()
            {
                Id = Guid.Parse("6a473634-ce64-4cae-acda-7306a2495454"),
                Name = "ADTTOTAL",
                DefaultValue = "1000",
                Minimum = 0.0,
                Type = "NUMBER",
                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = false
            };

        public static AttributeDTO DeckSeeded
            => new()
            {
                Id = Guid.Parse("01b059ea-ac64-4bba-8da1-3ef6a466fa92"),
                Name = "DECK_SEEDED",
                DefaultValue = "10",
                Minimum = 0.0,
                Maximum = 10.0,
                Type = "NUMBER",
                Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.DECK_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_DECK_RAW AS R ON R.BR_KEY = A.BRKEY",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = true
            };
        public static AttributeDTO SubSeeded => new()
        {
            Id = Guid.Parse("2ea9ef40-a59d-4cdf-ae75-f5596d4030a5"),
            Name = "SUB_SEEDED",
            DefaultValue = "10",
            Minimum = 0.0,
            Maximum = 10.0,
            Type = "NUMBER",
            Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.DECK_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_DECK_RAW AS R ON R.BR_KEY = A.BRKEY",
            AggregationRuleType = "AVERAGE",
            IsCalculated = false,
            IsAscending = true
        };
        public static AttributeDTO SupSeeded => new()
        {
            Id = Guid.Parse("c8cfea90-9cdc-4e02-a19e-fdcf698776a4"),
            Name = "SUP_SEEDED",
            DefaultValue = "10",
            Minimum = 0.0,
            Maximum = 10.0,
            Type = "NUMBER",
            Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.SUP_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_SUP_RAW AS R ON R.BR_KEY = A.BRKEY",
            AggregationRuleType = "AVERAGE",
            IsCalculated = false,
            IsAscending = true
        };

        public static AttributeDTO CulvSeeded => new()
        {
            Id = Guid.Parse("2ea9ef40-a59d-4cdf-ae75-f5596d4030a5"),
            Name = "SUB_SEEDED",
            DefaultValue = "10",
            Minimum = 0.0,
            Maximum = 10.0,
            Type = "NUMBER",
            Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.DECK_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_DECK_RAW AS R ON R.BR_KEY = A.BRKEY",
            AggregationRuleType = "AVERAGE",
            IsCalculated = false,
            IsAscending = true
        };

    }
}
