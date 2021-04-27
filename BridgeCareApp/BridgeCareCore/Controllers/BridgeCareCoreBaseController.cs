using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    public class BridgeCareCoreBaseController : ControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;

        protected readonly UnitOfDataPersistenceWork UnitOfWork;

        protected readonly IHubService HubService;

        public BridgeCareCoreBaseController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            HubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
        }

        protected UserInfo UserInfo
        {
            get
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);
                UnitOfWork.SetUser(userInfo.Name);
                return userInfo;
            }
        }

        private Guid UserId =>
            UnitOfWork.Context.User.Where(_ => _.Username == UserInfo.Name)
                .Select(user => new UserEntity {Id = user.Id}).FirstOrDefault()?.Id ?? Guid.Empty;


        public void CheckUserSimulationAuthorization(Guid simulationId)
        {
            if (!UnitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (!UnitOfWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.UserId == UserId && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
            }
        }
    }
}
