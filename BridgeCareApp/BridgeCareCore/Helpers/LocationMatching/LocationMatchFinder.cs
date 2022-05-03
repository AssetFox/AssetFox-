using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCareCore.Helpers
{
    public static class LocationMatchFinder
    {
        /// <summary>
        /// The searchList is expected to be a list of strings, each containing
        /// exactly one hyphen ('-'). An input is a match if any of the following is true:
        /// 1. It has an empty requestedAfterHyphen value, a nonempty requestedBeforeHyphen value, and the search list has exactly one entry with the requested value before the hyphen.
        /// 2. It has an empty requestedBeforeHyphen value, a nonempty requestedAfterHyphen value, and the search list has exactly one entry with the requested value after the hyphen.
        /// 3. It has both requestedBeforeHyphen and requestedAfterHyphen values, and the search list has exactly one matching entry.
        /// </summary>
        /// <param name="searchList"></param>
        /// <param name="requestedBeforeHyphen"></param>
        /// <param name="requestedAfterHyphen"></param>
        /// <returns></returns>
        public static LocationMatchSearchResult FindUniqueMatch(
            List<string> searchList,
            string requestedBeforeHyphen,
            string requestedAfterHyphen)
        {
            var emptyBefore = string.IsNullOrWhiteSpace(requestedBeforeHyphen);
            var emptyAfter = string.IsNullOrWhiteSpace(requestedAfterHyphen);
            if (emptyBefore && emptyAfter)
            {
                return new LocationMatchSearchResult
                {
                    LocationIdentifier = null,
                    Message = "There is no BRKey and no BMSID",
                };
            }
            if (emptyBefore && !emptyAfter)
            {
                var matchingKeys = searchList.Where(x => x.Substring(1+x.IndexOf('-')) == requestedAfterHyphen).ToList();
                return matchingKeys.Count switch
                {
                    0 => new LocationMatchSearchResult
                    {
                        LocationIdentifier = null,
                        Message = $"There is no location with a BMSID of {requestedAfterHyphen}",
                    },
                    1 => new LocationMatchSearchResult
                    {
                        LocationIdentifier = matchingKeys[0],
                    },
                    _ => new LocationMatchSearchResult
                    {
                        LocationIdentifier = null,
                        Message = $"There are {matchingKeys.Count} locations with a BMSID of {requestedAfterHyphen}.",
                    },
                };
            }
            if (!emptyBefore && emptyAfter)
            {
                var matchingKeys = searchList.Where(x => x[..x.IndexOf('-')] == requestedBeforeHyphen).ToList();
                return matchingKeys.Count switch
                {
                    0 => new LocationMatchSearchResult
                    {
                        LocationIdentifier = null,
                        Message = $"There are no locations with a BMSID of {requestedAfterHyphen}",
                    },
                    1 => new LocationMatchSearchResult
                    {
                        LocationIdentifier = matchingKeys.Single(),

                    },
                    _ => new LocationMatchSearchResult
                    {
                        LocationIdentifier = null,
                        Message = $"There are {matchingKeys.Count} entries with a BRKey of {requestedBeforeHyphen}.",
                    },
                };
            }
            if (!emptyBefore && !emptyAfter)
            {
                var fullRequestedString = $"{requestedBeforeHyphen}-{requestedAfterHyphen}";
                if (searchList.Contains(fullRequestedString))
                {
                    return new LocationMatchSearchResult
                    {
                        LocationIdentifier = fullRequestedString,
                    };
                } else
                {
                    return new LocationMatchSearchResult {
                        LocationIdentifier = null,
                        Message = $"No location found with a BRKEY of {requestedBeforeHyphen} and a BMSID of {requestedAfterHyphen}",
                    };
                }
            }
            throw new InvalidProgramException("This line should never be reached");
        }
    }
}
