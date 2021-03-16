namespace BridgeCareCore.Security
{
    public static class SecurityConstants
    {
        public static class Policy
        {
            public const string Admin = "UserIsAdmin";
            public const string AdminOrDistrictEngineer = "UserIsAdminOrDistrictEngineer";
        }
    }
}
