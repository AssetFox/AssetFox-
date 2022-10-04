using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Security;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Utils
{
    public class ClaimHelper: IClaimHelper
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IHttpContextAccessor ContextAccessor; 

        public ClaimHelper(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            ContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));          
        }

        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="userId"></param>
        /// <param name="checkSimulationAccess"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void CheckUserSimulationReadAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess)
        {
            if (RequirePermittedCheck() && !(checkSimulationAccess && HasSimulationAccess()))
            {
                var simulation = GetSimulationWithUsers(simulationId);
                if (!simulation.Users.Any(_ => _.UserId == userId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this simulation's data.");                    
                }
            }
        }

        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="userId"></param>
        /// <param name="checkSimulationAccess"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void CheckUserSimulationModifyAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess)
        {
            if (RequirePermittedCheck() && !(checkSimulationAccess && HasSimulationAccess()))
            {
                var simulation = GetSimulationWithUsers(simulationId);
                if (!simulation.Users.Any(_ => _.UserId == userId && _.CanModify))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
                }
            }
        }

        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="owner"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void CheckUserLibraryModifyAuthorization(Guid owner, Guid userId)
        {
            if (RequirePermittedCheck() && owner != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
            }
        }

        public bool RequirePermittedCheck()
        {
            return !HasAdminAccess();
        }

        private SimulationDTO GetSimulationWithUsers(Guid simulationId)
        {
            SimulationDTO simulation;
            try
            {
                simulation = UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            }
            catch
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (simulation.Users == null)
            {
                throw new RowNotInTableException($"No users assigned to requested simulation");
            }

            return simulation;
        }      

        private bool HasAdminAccess()
        {
            return ContextAccessor.HttpContext.User.HasClaim(claim => claim.Value == SecurityConstants.Claim.AdminAccess);
        }

        private bool HasSimulationAccess()
        {
            return ContextAccessor.HttpContext.User.HasClaim(claim => claim.Value == SecurityConstants.Claim.SimulationAccess);
        }
    }
}
