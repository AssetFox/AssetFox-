using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Http;
using static BridgeCareCore.Security.SecurityConstants;

namespace BridgeCareCore.Utils
{
    public class ClaimHelper: IClaimHelper
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IHttpContextAccessor ContextAccessor;
        // TODO check if it gets assigned here else move down
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public ClaimHelper(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            ContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void CheckUserSimulationReadAuthorization(Guid simulationId)
        {
            if (RequirePermittedCheck())
            {
                var simulation = GetSimulationWithUsers(simulationId);
                if (!simulation.Users.Any(_ => _.UserId == UserId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this simulation's data.");
                }
            }
        }

        /// <summary>
        /// Checks if user need permitted check, if so checks further if it is authorized to perform action.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void CheckUserSimulationModifyAuthorization(Guid simulationId)
        {
            if (RequirePermittedCheck())
            {
                var simulation = GetSimulationWithUsers(simulationId);
                if (!simulation.Users.Any(_ => _.UserId == UserId && _.CanModify))
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
        public void CheckUserLibraryModifyAuthorization(Guid owner)
        {
            if (RequirePermittedCheck() && owner != UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this library's data.");
            }
        }

        public bool RequirePermittedCheck()
        {
            return !IsUserInAdministratorRole();
        }

        private SimulationDTO GetSimulationWithUsers(Guid simulationId)
        {
            SimulationDTO simulation = null;
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

        private bool IsUserInAdministratorRole()
        {
            return ContextAccessor.HttpContext.User.IsInRole(Role.Administrator);
        }        
    }
}
