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
    public class PerformanceCurveLibraryController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public PerformanceCurveLibraryController(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        [HttpGet]
        public async Task<IActionResult> GetPerformanceCurveLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.PerformanceCurveRepo
                    .GetPerformanceCurveLibrariesWithPerformanceCurves();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("GetScenarioPerformanceCurveLibrary/{simulationId}")]
        public async Task<IActionResult> GetScenarioPerformanceCurveLibrary(Guid simulationId)
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.PerformanceCurveRepo
                    .GetSimulationPerformanceCurveLibraryWithPerformanceCurves(simulationId);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("AddOrUpdatePerformanceCurveLibrary/{simulationId}")]
        public async Task<IActionResult> AddOrUpdatePerformanceCurveLibrary(Guid simulationId, PerformanceCurveLibraryDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.PerformanceCurveRepo.AddOrUpdatePerformanceCurveLibrary(dto,
                        simulationId);
                    _unitOfDataPersistenceWork.PerformanceCurveRepo.AddOrUpdateOrDeletePerformanceCurves(
                        dto.PerformanceCurves, dto.Id);
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
        [Route("DeletePerformanceCurveLibrary/{libraryId}")]
        public async Task<IActionResult> DeletePerformanceCurveLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId));
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
