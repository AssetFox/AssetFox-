using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Cache
{ 
    public class AggregatedSelectValuesResultDtoCache
    {
        public static TimeSpan ValidityDuration = TimeSpan.FromHours(12);
        private ConcurrentDictionary<string, AggregatedSelectValuesResultDtoCacheEntry> Cache { get; set; } = new();

        public void ClearInvalid()
        {
            var now = DateTime.Now;
            var keys = Cache.Keys;
            foreach (var key in keys)
            {
                if (Cache.TryGetValue(key, out var value)) {
                    if (value.ValidUntil > now)
                    {
                        Cache.Remove(key, out _);
                    }
                }
            }
        }

        /// <summary>Returns null if there is no valid entry for the attribute name</summary> 
        public AggregatedSelectValuesResultDTO TryGetCachedValue(string attributeName)
        {
            if (Cache.TryGetValue(attributeName, out var cacheEntry))
            {
                if (cacheEntry.ValidUntil > DateTime.Now)
                {
                    return cacheEntry.Dto;
                }
                else
                {
                    Cache.Remove(attributeName, out AggregatedSelectValuesResultDtoCacheEntry _);
                }
            }
            return null;
        }

        public void SaveToCache(AggregatedSelectValuesResultDTO dto)
        {
            var now = DateTime.Now;
            var cacheEntry = new AggregatedSelectValuesResultDtoCacheEntry
            {
                Dto = dto,
                ValidUntil = now + ValidityDuration,
            };
            var ___ = Cache.AddOrUpdate(dto.Attribute.Name, _ => cacheEntry, (_, __) => cacheEntry);
        }
    }
}
