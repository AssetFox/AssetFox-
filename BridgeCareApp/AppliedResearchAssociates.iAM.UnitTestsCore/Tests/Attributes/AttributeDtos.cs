using System;
using System.Security.Cryptography.X509Certificates;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AttributeDtos
    {
        public static AttributeDTO ActionType
            => new()
            {
                Id = Guid.Parse("85e2b431-05ec-4ea9-92cd-4d663d657262"),
                Name = TestAttributeNames.ActionType,
                DefaultValue = "0",
                Minimum = 100.0,
                Maximum = 0.0,
                Type = "STRING",
                Command = "SELECT CAST(PennDot_Report_A.BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(PennDot_Report_A.BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, (ActionCode) AS DATA_ FROM dbo.PennDot_Report_A,PENNDOT_ActionItems WHERE dbo.PennDot_Report_A.BRKEY=PENNDOT_ActionItems.BRKEY group by dbo.PennDot_Report_A.BRKEY,BRIDGE_ID,CAST(INSPDATE AS DATETIME),ActionCode",
                AggregationRuleType = "PREDOMINANT",
                IsCalculated = false,
                IsAscending = true
            };


        public static AttributeDTO Interstate
            => new()
            {
                Id = Guid.Parse("5552b431-05ec-4ea9-92cd-4d663d657262"),
                Name = TestAttributeNames.Interstate,
                DefaultValue = "",
                Type = "STRING",
                Command = "",
                AggregationRuleType = "PREDOMINANT",
                IsCalculated = false,
                IsAscending = true
            };


        public static AttributeDTO InternetReport
            => new()
            {
                Id = Guid.Parse("4442b431-05ec-4ea9-92cd-4d663d657262"),
                Name = TestAttributeNames.InternetReport,
                DefaultValue = "",
                Type = "STRING",
                Command = "",
                AggregationRuleType = "PREDOMINANT",
                IsCalculated = false,
                IsAscending = true
            };

        public static AttributeDTO AdtTotal
            => new()
            {
                Id = Guid.Parse("6a473634-ce64-4cae-acda-7306a2495454"),
                Name = TestAttributeNames.AdtTotal,
                DefaultValue = "1000",
                Minimum = 0.0,
                Type = "NUMBER",
                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = false
            };

        public static AttributeDTO Age => new()
        {

            Id = Guid.Parse("d27f24d1-7f8a-4778-a2b2-e61911a58897"),
            Name = TestAttributeNames.Age,
            DefaultValue = "0",
            Minimum = 0.0,
            Maximum = 100.0,
            Type = "NUMBER",
            Command = "SELECT BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS datetime) AS DATE_, CAST(2021 - YEAR_BUILT + 1 AS float) AS DATA_ FROM dbo.PennDot_Report_A",
            AggregationRuleType = "AVERAGE",
            IsCalculated = false,
            IsAscending = false
        };

        public static AttributeDTO CulvSeeded
            => new()
            {
                Id = new Guid("80ad7772-af11-4f2c-8f48-e243ec872014"),
                Name = TestAttributeNames.CulvSeeded,
                DefaultValue = "10",
                Minimum = 0.0,
                Maximum = 10.0,
                Type = "NUMBER",
                Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.CULV_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_CULV_RAW AS R ON R.BR_KEY = A.BRKEY",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = true
            };

        public static AttributeDTO DeckSeeded
            => new()
            {
                Id = Guid.Parse("01b059ea-ac64-4bba-8da1-3ef6a466fa92"),
                Name = TestAttributeNames.DeckSeeded,
                DefaultValue = "10",
                Minimum = 0.0,
                Maximum = 10.0,
                Type = "NUMBER",
                Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.DECK_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_DECK_RAW AS R ON R.BR_KEY = A.BRKEY",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = true
            };

        public static AttributeDTO District(BaseDataSourceDTO dataSourceDTO) => new()
        {
            Name = TestAttributeNames.District,
            AggregationRuleType = TextAttributeAggregationRules.Predominant,
            Id = Guid.NewGuid(),
            Command = "DISTRICT",
            DefaultValue = "Default District",
            Type = "STRING",
            IsAscending = false,
            IsCalculated = false,
            DataSource = dataSourceDTO,
        };

        public static AttributeDTO SubSeeded => new()
        {
            Id = Guid.Parse("2ea9ef40-a59d-4cdf-ae75-f5596d4030a5"),
            Name = TestAttributeNames.SubSeeded,
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
            Name = TestAttributeNames.SupSeeded,
            DefaultValue = "10",
            Minimum = 0.0,
            Maximum = 10.0,
            Type = "NUMBER",
            Command = "SELECT TOP (100) PERCENT A.BRKEY AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(A.BRKEY AS VARCHAR(MAX)) AS FACILITY, A.BRIDGE_ID AS SECTION, NULL AS SAMPLE_, A.INSPDATE AS DATE_, R.SUP_INDEX AS DATA_ FROM dbo.PennDot_Report_A AS A INNER JOIN dbo.SEED_SUP_RAW AS R ON R.BR_KEY = A.BRKEY",
            AggregationRuleType = "AVERAGE",
            IsCalculated = false,
            IsAscending = true
        };

        public static AttributeDTO CulvDurationN
            => new()
            {
                Id = Guid.Parse("efca598b-9fca-4e3c-ac48-0d95a9eaa867"),
                Name = TestAttributeNames.CulvDurationN,
                DefaultValue = "1",
                Type = "NUMBER",
                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(CULV_DUR AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (CULV_DUR <> 'N')",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = false

            };

        public static AttributeDTO DeckDurationN =>
            new()
            {
                Id = Guid.Parse("497bb1c9-640d-4433-be88-13c524b9593b"),
                Name = TestAttributeNames.DeckDurationN,
                DefaultValue = "1",
                Type = "NUMBER",
                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(DECK_DUR AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (DECK_DUR <> 'N')",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = false
            };

        public static AttributeDTO SubDurationN =>
            new()
            {
                Id = Guid.Parse("df0c899f-4791-4418-b012-90146f4397f3"),
                Name = TestAttributeNames.SubDurationN,
                DefaultValue = "1",
                Type = "NUMBER",
                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(SUB_DUR AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (SUB_DUR <> 'N')",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = false
            };

        public static AttributeDTO SupDurationN
            => new()
            {
                Id = Guid.Parse("286965d4-ba60-44aa-a368-d6de784fa90e"),
                Name = TestAttributeNames.SupDurationN,
                DefaultValue = "1",
                Type = "NUMBER",
                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(SUP_DUR AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (SUP_DUR <> 'N')",
                AggregationRuleType = "AVERAGE",
                IsCalculated = false,
                IsAscending = false
            };

        public static AttributeDTO Text(
            string name,
            Guid id)
        {
            var dto = new AttributeDTO
            {
                AggregationRuleType = "PREDOMINANT",
                Id = id,
                IsAscending = true,
                Command = "",
                Name = name,
                Type = AttributeTypeNames.String,
                DefaultValue = "default attribute value",
                IsCalculated = false,
            };
            return dto;
        } 
    }
}
