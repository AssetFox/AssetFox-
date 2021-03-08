using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCare.Security;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : ControllerBase
    {
        private readonly EsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BudgetPriorityController(EsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
        [RestrictAccess]
        public async Task<IActionResult> BudgetPriorityLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.BudgetPriorityRepo
                    .BudgetPriorityLibrariesWithBudgetPriorities();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("UpsertBudgetPriorityLibrary/{simulationId}")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public async Task<IActionResult> UpsertBudgetPriorityLibrary(Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);
                if (userInfo.Role != Role.ADMINISTRATOR)
                {

                }

                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BudgetPriorityRepo
                        .UpsertBudgetPriorityLibrary(dto, simulationId);
                    _unitOfDataPersistenceWork.BudgetPriorityRepo
                        .UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
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

        private async void Upsert(Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BudgetPriorityRepo
                        .UpsertBudgetPriorityLibrary(dto, simulationId);
                    _unitOfDataPersistenceWork.BudgetPriorityRepo
                        .UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
                });

                _unitOfDataPersistenceWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                throw;
            }
        }

        private void UpsertPermitted(Guid simulationId, UserInfo userInfo, BudgetPriorityLibraryDTO dto)
        {
            if (false)
            {
                throw new UnauthorizedAccessException("User is not authorized to edit scenario's budget priorities.");
            }
            Upsert(simulationId, dto);
        }

        [HttpDelete]
        [Route("DeleteBudgetPriorityLibrary/{libraryId}")]
        public async Task<IActionResult> DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.BudgetPriorityRepo
                    .DeleteBudgetPriorityLibrary(libraryId));
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
