using System;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForTreatmentSupersedeRules
    {
        public static Guid NetworkId => Guid.Parse("7940d27c-c9b1-4ef2-b5e7-2f1d8240a011");

        public static Guid SimulationId => Guid.Parse("dcdacfde-02da-4109-b8aa-add932756d11");

        public static Guid TreatmentId => Guid.Parse("00dacfde-02da-4109-b8aa-add932756d11");

        public static Guid LocalBudgetId => Guid.Parse("93d59432-c9e5-4a4a-8f1b-d18dcfc05211");

        public static Guid InterstateBudgetId => Guid.Parse("4dcf6bbc-d135-458c-a6fc-db9bb0801b11");

        public static Guid PreventTreatmentId => Guid.Parse("11ddccde-02da-4109-b8aa-add932756d11");

        public static Guid CostId => Guid.Parse("100dacfe-02da-4109-b8aa-add932756d11");

        public const string NetworkName = "Primary";

        public const string Username = "pdsystbamsusr01";

        public const string InterstateBudgetName = "Interstate";

        public const string LocalBudgetName = "Local";
    }
}
