using System.Data;

namespace AppliedResearchAssociates.iAM.DataAccess
{
    internal static class IDataReaderExtensions
    {
        public static bool? GetNullableBoolean(this IDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetBoolean(i).AsNullable();

        public static double? GetNullableDouble(this IDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetDouble(i).AsNullable();

        public static int? GetNullableInt32(this IDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetInt32(i).AsNullable();

        public static string GetNullableString(this IDataReader reader, int i) => reader.IsDBNull(i) ? null : reader.GetString(i);
    }
}
