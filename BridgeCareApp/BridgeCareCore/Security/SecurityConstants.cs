namespace BridgeCareCore.Security
{
    public static class SecurityConstants
    {
        public static class Policy
        {
            public const string Admin = "UserIsAdmin";
            public const string AdminOrDistrictEngineer = "UserIsAdminOrDistrictEngineer";
        }

        public static class Role
        {
            public const string BAMSAdmin = "PD-BAMS-Administrator";
        }
    }
}
