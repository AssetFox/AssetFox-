using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenefitQuantifierController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;

        public BenefitQuantifierController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
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
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("UpsertBenefitQuantifier/")]
        [Authorize]
        public async Task<IActionResult> UpsertBenefitQuantifier([FromBody] BenefitQuantifierDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request).ToDto();
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BenefitQuantifierRepo.UpsertBenefitQuantifier(dto, userInfo);
                });

                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("DeleteBenefitQuantifier/{networkId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBenefitQuantifier(Guid networkId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                    _unitOfDataPersistenceWork.BenefitQuantifierRepo.DeleteBenefitQuantifier(networkId));
                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
