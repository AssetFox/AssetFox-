using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BudgetPriorityController(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
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
        [Route("AddOrUpdateBudgetPriorityLibrary/{simulationId}")]
        public async Task<IActionResult> AddOrUpdateBudgetPriorityLibrary(Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BudgetPriorityRepo
                        .AddOrUpdateBudgetPriorityLibrary(dto, simulationId);
                    _unitOfDataPersistenceWork.BudgetPriorityRepo
                        .AddOrUpdateOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
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
