using System;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Utils
{
    public static class EnumDeserializer
    {
        public static T Deserialize<T>(string serialized)
            where T: Enum
        {
            foreach (var t in EnumExtensions.GetValues<T>())
            {
                if (serialized.Equals(t.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return t;
                }
            };
            var errorMessage = $"Failed to deserialize {serialized} to type {typeof(T).Name}";
            throw new InvalidOperationException(errorMessage);
        }
    }
}
