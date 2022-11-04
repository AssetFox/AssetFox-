using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public static class LibraryUserDtoListExtensions
    {
        /// <summary>Returns null if no user's access has changed.</summary>
        public static Guid? IdOfAnyUserWithChangedAccess(IList<LibraryUserDTO> userList1, IList<LibraryUserDTO> userList2)
        {
            var dictionary1 = userList1.ToDictionary(dto => dto.UserId);
            var dictionary2 = userList2.ToDictionary(dto => dto.UserId);
            var allKeys = dictionary1.Keys.Union(dictionary2.Keys);
            foreach (var key in allKeys)
            {
                if (dictionary1.ContainsKey(key) && dictionary2.ContainsKey(key))
                {
                    var currentDto = dictionary1[key];
                    var newDto = dictionary2[key];
                    if (currentDto.AccessLevel == newDto.AccessLevel)
                    {
                        continue;
                    }
                    return key;
                }
            }
            return null;
        }
    }
}
