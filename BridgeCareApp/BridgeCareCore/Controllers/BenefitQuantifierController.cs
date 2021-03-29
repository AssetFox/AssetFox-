using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenefitQuantifierController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BenefitQuantifierController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        [HttpGet]
        [Route("GetBenefitQuantifier/{networkId}")]
        [Authorize]
        public async Task<IActionResult> GetBenefitQuantifier(Guid networkId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _unitOfDataPersistenceWork.BenefitQuantifierRepo.GetBenefitQuantifier(networkId));

                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Benefit Quantifier error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBenefitQuantifier")]
        [Authorize]
        public async Task<IActionResult> UpsertBenefitQuantifier([FromBody] BenefitQuantifierDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BeginTransaction();
                    _unitOfDataPersistenceWork.BenefitQuantifierRepo.UpsertBenefitQuantifier(dto);
                    _unitOfDataPersistenceWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Benefit Quantifier error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBenefitQuantifier/{networkId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBenefitQuantifier(Guid networkId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BeginTransaction();
                    _unitOfDataPersistenceWork.BenefitQuantifierRepo.DeleteBenefitQuantifier(networkId);
                    _unitOfDataPersistenceWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Benefit Quantifier error::{e.Message}");
                throw;
            }
        }
    }
}
