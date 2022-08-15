namespace BridgeCareCore.Security
{
    public static class SecurityConstants
    {
        public static class SecurityTypes
        {
            public const string Esec = "ESEC";
            public const string B2C = "B2C";
        }

        public static class Policy
        {
            public const string Admin = "UserIsAdmin";
            public const string AdminOrDistrictEngineer = "UserIsAdminOrDistrictEngineer";
        }

        public static class Role
        {            
            public const string Administrator = "Administrator";

            // TODO remove later:
            public const string BAMSAdmin = "PD-BAMS-Administrator";            
        }
        public static class Claim
        {
            public static string AttributesUpdateAccess = "AttributesUpdateAccess";

        }
    }
}
