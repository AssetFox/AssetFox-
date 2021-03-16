namespace AppliedResearchAssociates.iAM.DataPersistenceCore
{
    public static class BridgeCareCoreConstants
    {
        public const string PennDotNetworkId = "D7B54881-DD44-4F93-8250-3D4A630A4D3B";
        public const string TestSimulationId = "DF71AC9B-B90A-425C-A519-7B2D6B531DDC";
    }

    public static class Role
    {
        public const string Administrator = "PD-BAMS-Administrator";
        public const string DistrictEngineer = "PD-BAMS-DBEngineer";
        public const string PlanningPartner = "PD-BAMS-PlanningPartner";
        public const string Cwopa = "PD-BAMS-CWOPA";
        public static string[] AllValidRoles = { Administrator, DistrictEngineer, PlanningPartner, Cwopa };
    }
}
