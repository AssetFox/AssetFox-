using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashFlowController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public CashFlowController(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        [HttpGet]
        [Route("GetCashFlowRuleLibraries")]
        public async Task<IActionResult> CashFlowRuleLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.CashFlowRuleRepo
                    .CashFlowRuleLibrariesWithCashFlowRules();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("AddOrUpdateCashFlowRuleLibrary/{simulationId}")]
        public async Task<IActionResult> AddOrUpdateCashFlowRuleLibrary(Guid simulationId, CashFlowRuleLibraryDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.CashFlowRuleRepo
                        .AddOrUpdateCashFlowRuleLibrary(dto, simulationId);
                    _unitOfDataPersistenceWork.CashFlowRuleRepo
                        .AddOrUpdateOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);
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
        [Route("DeleteCashFlowRuleLibrary/{libraryId}")]
        public async Task<IActionResult> DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.CashFlowRuleRepo
                    .DeleteCashFlowRuleLibrary(libraryId));
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
