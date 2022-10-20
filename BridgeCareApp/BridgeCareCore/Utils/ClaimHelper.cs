using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Security;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Utils
{
    public class ClaimHelper: IClaimHelper
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IHttpContextAccessor ContextAccessor;

        public const string LibraryModifyUnauthorizedMessage = "You are not authorized to modify this library's data.";
        public const string LibraryDeleteUnauthorizedMessage = "You are not authorized to delete this library.";
        public const string LibraryRecreateUnauthorizedMessage = "You are not authorized to recreate this library.";
        public const string CantDeleteNonexistentLibraryMessage = "We can't delete a nonexistent library.";

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
            // Amruta: comment for Todo: keep the check for checkSimulationAccess(it has its purpose, its for CWOPA user with full simulation access.
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
        public void ObsoleteCheckUserLibraryModifyAuthorization(Guid owner, Guid userId)
        {
            if (RequirePermittedCheck() && owner != userId)
            {
                throw new UnauthorizedAccessException(LibraryModifyUnauthorizedMessage);
            }
        }

        public void CheckUserLibraryModifyAuthorization(LibraryAccessModel accessModel, Guid userId)
        {
            if (RequirePermittedCheck() && accessModel.LibraryExists)
            {
                var unauthorized = accessModel.Unauthorized(userId, LibraryAccessLevel.Modify);
                if (unauthorized)
                {
                    throw new UnauthorizedAccessException(LibraryModifyUnauthorizedMessage);
                }
            }
        }
                
        private void CheckIfLibraryExists(LibraryAccessModel accessModel, string failureMessage)
        {
            if (!accessModel.LibraryExists)
            {
                throw new UnauthorizedAccessException(failureMessage);
            }
        }


        public void CheckUserLibraryDeleteAuthorization(LibraryAccessModel accessModel, Guid userId)
        {
            CheckIfLibraryExists(accessModel, CantDeleteNonexistentLibraryMessage);
            if (RequirePermittedCheck())
            {
                var unauthorized = accessModel.Unauthorized(userId, LibraryAccessLevel.Owner);
                if (unauthorized)
                {
                    throw new UnauthorizedAccessException(LibraryDeleteUnauthorizedMessage);
                }
            }
        }

        public void CheckUserLibraryRecreateAuthorization(LibraryAccessModel accessModel, Guid userId)
        {
            if (RequirePermittedCheck()) {
                var unauthorized = accessModel.Unauthorized(userId, LibraryAccessLevel.Owner);
                if (unauthorized)
                {
                    throw new UnauthorizedAccessException(LibraryRecreateUnauthorizedMessage);
                }
            }
        }

        /// <summary>Returns true if the user can change the access levels of
        /// users to the library. Does not throw.</summary>
        public bool CanModifyAccessLevels(LibraryAccessModel accessModel, Guid userId)
        {
            bool canModify = HasAdminAccess() || accessModel.HasAccess(userId, LibraryAccessLevel.Owner);
            return canModify;
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
