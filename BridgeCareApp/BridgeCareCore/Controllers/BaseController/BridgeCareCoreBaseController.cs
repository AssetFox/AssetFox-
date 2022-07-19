using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers.BaseController
{
    public class BridgeCareCoreBaseController : ControllerBase
    {
        protected readonly IEsecSecurity EsecSecurity;

        protected readonly IUnitOfWork UnitOfWork;

        protected readonly IHubService HubService;

        protected readonly IHttpContextAccessor ContextAccessor;

        private readonly IReadOnlyCollection<string> PathsToIgnore = new List<string>
        {
            "UserTokens", "RevokeToken", "RefreshToken"
        };

        public BridgeCareCoreBaseController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor)
        {
            EsecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            HubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            if (RequestHasBearer()) // WjTodo -- test the behavior here.
            {
                SetUserInfo(ContextAccessor?.HttpContext?.Request);
            }
        }

        private bool RequestHasBearer()
        {
            if (ContextAccessor?.HttpContext?.Request != null)
            {
                return !PathsToIgnore.Any(pathToIgnore =>
                    ContextAccessor.HttpContext.Request.Path.Value.Contains(pathToIgnore));
            }

            return false;
        }

        public void SetUserInfo(HttpRequest request) => UserInfo = EsecSecurity.GetUserInformation(request);

        private UserInfo _userInfo;
        protected UserInfo UserInfo
        {
            get => _userInfo ?? new UserInfo();
            private set
            {
                if (_userInfo != value)
                {
                    _userInfo = value;
                    CheckIfUserExist();
                    
                }
            }
        }

        private void CheckIfUserExist()
        {
                if (!string.IsNullOrEmpty(UserInfo.Name))
                {
                    if (!UnitOfWork.UserRepo.UserExists(UserInfo.Name))
                    {
                        UnitOfWork.AddUser(UserInfo.Name, UserInfo.Role);
                    }

                    UnitOfWork.SetUser(_userInfo.Name);
                }
        }

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public void CheckUserSimulationReadAuthorization(Guid simulationId)
        {
            var simulation = GetSimulationWithUsers(simulationId);

            if (!simulation.Users.Any(_ => _.UserId == UserId))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this simulation's data.");
            }
        }

        public void CheckUserSimulationModifyAuthorization(Guid simulationId)
        {
            var simulation = GetSimulationWithUsers(simulationId);

            if (!simulation.Users.Any(_ => _.UserId == UserId && _.CanModify))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this simulation's data.");
            }
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
    }
}
