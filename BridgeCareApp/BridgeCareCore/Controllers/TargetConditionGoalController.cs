using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TargetConditionGoalController(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        [HttpGet]
        [Route("GetTargetConditionGoalLibraries")]
        public async Task<IActionResult> TargetConditionGoalLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.TargetConditionGoalRepo
                    .TargetConditionGoalLibrariesWithTargetConditionGoals();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("AddOrUpdateTargetConditionGoalLibrary/{simulationId}")]
        public async Task<IActionResult> AddOrUpdateTargetConditionGoalLibrary(Guid simulationId, TargetConditionGoalLibraryDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.TargetConditionGoalRepo
                        .AddOrUpdateTargetConditionGoalLibrary(dto, simulationId);
                    _unitOfDataPersistenceWork.TargetConditionGoalRepo
                        .AddOrUpdateOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
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
        [Route("DeleteTargetConditionGoalLibrary/{libraryId}")]
        public async Task<IActionResult> DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.TargetConditionGoalRepo
                    .DeleteTargetConditionGoalLibrary(libraryId));
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
