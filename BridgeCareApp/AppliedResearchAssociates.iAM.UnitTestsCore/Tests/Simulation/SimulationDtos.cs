using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationDtos
    {
        public static SimulationDTO Dto(Guid? id = null, string name = null, Guid? owner = null)
        {
            var resolveName = name ?? RandomStrings.Length11();
            var resolveId = id ?? Guid.NewGuid();
            var users = new List<SimulationUserDTO>();
            if (owner != null)
            {
                var newUser = new SimulationUserDTO
                {
                    IsOwner = true,
                    UserId = owner.Value,
                };
                users.Add(newUser);
            }
            var returnValue = new SimulationDTO
            {
                Id = resolveId,
                NetworkId = NetworkTestSetup.NetworkId,
                Name = resolveName,
                Users = users,
            };
            return returnValue;
        }
    }
}
