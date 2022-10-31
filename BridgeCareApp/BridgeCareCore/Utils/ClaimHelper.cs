using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Utils
{
    public class ClaimHelper: IClaimHelper
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IHttpContextAccessor ContextAccessor;
        private readonly ISimulationQueueService _simulationQueueService;

        public const string LibraryModifyUnauthorizedMessage = "You are not authorized to modify this library's data.";
        public const string LibraryDeleteUnauthorizedMessage = "You are not authorized to delete this library.";
        public const string LibraryRecreateUnauthorizedMessage = "You are not authorized to recreate this library.";
        public const string LibraryAccessModificationUnauthorizedMessage = "You are not authorized to modify access to this library.";
        public const string LibraryUserListGetUnauthorizedMessage = "You are not authorized to get the users of this library.";
        public const string SimulationModifyUnauthorizedMessage = "You are not authorized to modify this simulation's data.";
        public const string CantDeleteNonexistentLibraryMessage = "Cannot delete library. Not in system.";
        public const string AddingOwnersIsNotAllowedMessage = "Adding owners to a library is not allowed.";
        public const string RemovingOwnersIsNotAllowedMessage = "Removing owners of a library is not allowed.";

        public ClaimHelper(IUnitOfWork unitOfWork, ISimulationQueueService simulationQueueService, IHttpContextAccessor contextAccessor)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            ContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _simulationQueueService = simulationQueueService ?? throw new ArgumentNullException(nameof(simulationQueueService));
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
                    throw new UnauthorizedAccessException(SimulationModifyUnauthorizedMessage);
                }
            }
        }


        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="userName"></param>
        /// <param name="checkSimulationAccess"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void CheckUserSimulationCancelAnalysisAuthorization(Guid simulationId, string userName, bool checkSimulationAccess)
        {
            if (RequirePermittedCheck() && !(checkSimulationAccess && HasSimulationAccess()))
            {
                var simulation = GetQueuedSimulation(simulationId);
                if (simulation.QueueingUser == userName)
                {
                    throw new UnauthorizedAccessException("You are not authorized to cancel this simulation analysis.");
                }
            }
        }


        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="owner"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void OldWayCheckUserLibraryModifyAuthorization(Guid owner, Guid userId)
        {//
            if (RequirePermittedCheck() && owner != userId)
            {
                throw new UnauthorizedAccessException(LibraryModifyUnauthorizedMessage);
            }
        }

        public void CheckUserLibraryModifyAuthorization(LibraryUserAccessModel accessModel, Guid userId)
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
                
        private void CheckIfLibraryExists(LibraryUserAccessModel accessModel, string failureMessage)
        {
            if (!accessModel.LibraryExists)
            {
                throw new UnauthorizedAccessException(failureMessage);
            }
        }


        public void CheckUserLibraryDeleteAuthorization(LibraryUserAccessModel accessModel, Guid userId)
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

        public void CheckUserLibraryRecreateAuthorization(LibraryUserAccessModel accessModel, Guid userId)
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
        public bool CanModifyAccessLevels(LibraryUserAccessModel accessModel, Guid userId)
        {
            bool canModify = HasAdminAccess() || accessModel.HasAccess(userId, LibraryAccessLevel.Owner);
            return canModify;
        }

        public void CheckAccessModifyValidity(List<LibraryUserDTO> usersBefore, List<LibraryUserDTO> proposedUsersAfter, Guid userId)
        {
            var relevantUser = usersBefore.SingleOrDefault(u => u.UserId == userId);
            var accessModel = new LibraryUserAccessModel
            {
                UserId = userId,
                LibraryExists = true,
                Access = relevantUser,
            };
            var canModify = CanModifyAccessLevels(accessModel, userId);
            if (!canModify)
            {
                throw new UnauthorizedAccessException(LibraryAccessModificationUnauthorizedMessage);
            }
            var ownersBefore = usersBefore.Where(u => u.AccessLevel == LibraryAccessLevel.Owner).Select(u => u.UserId).ToList();
            var ownersAfter = proposedUsersAfter.Where(u => u.AccessLevel == LibraryAccessLevel.Owner).Select(u => u.UserId).ToList();
            var addedOwners = ownersAfter.Except(ownersBefore).ToList();
            var removedOwners = ownersBefore.Except(ownersAfter).ToList();
            if (addedOwners.Any())
            {
                var addedOwner = addedOwners.First();
                throw new InvalidOperationException($"{AddingOwnersIsNotAllowedMessage} This update added {addedOwner}");
            }
            if (removedOwners.Any())
            {
                var removedOwner = removedOwners.First();
                throw new InvalidOperationException($"{RemovingOwnersIsNotAllowedMessage} This update removed {removedOwner}");
            }
        }

        public void CheckGetLibraryUsersValidity(LibraryUserAccessModel accessModel, Guid userId)
        {
            if (RequirePermittedCheck())
            {
                if (!accessModel.HasAccess(userId, LibraryAccessLevel.Owner))
                {
                    throw new UnauthorizedAccessException(LibraryUserListGetUnauthorizedMessage);
                }
            }
        }

        public bool RequirePermittedCheck()
        {
            return !HasAdminAccess();
        }

        private QueuedSimulationDTO GetQueuedSimulation(Guid simulationId)
        {
            QueuedSimulationDTO simulation;
            try
            {
                simulation = _simulationQueueService.GetQueuedSimulation(simulationId);
            }
            catch
            {
                throw new RowNotInTableException($"No simulation analysis found in queue having id {simulationId}");
            }
            return simulation;
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
