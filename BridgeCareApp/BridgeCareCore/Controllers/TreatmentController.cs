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
    public class TreatmentController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TreatmentController(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        [HttpGet]
        [Route("GetTreatmentLibraries")]
        public async Task<IActionResult> TreatmentLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.SelectableTreatmentRepo
                    .TreatmentLibrariesWithTreatments();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("AddOrUpdateTreatmentLibrary/{simulationId}")]
        public async Task<IActionResult> AddOrUpdateTreatmentLibrary(Guid simulationId, TreatmentLibraryDTO dto)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.SelectableTreatmentRepo
                        .AddOrUpdateTreatmentLibrary(dto, simulationId);
                    _unitOfDataPersistenceWork.SelectableTreatmentRepo
                        .AddOrUpdateOrDeleteTreatments(dto.Treatments, dto.Id);
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
        [Route("DeleteTreatmentLibrary/{libraryId}")]
        public async Task<IActionResult> DeleteTreatmentLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.SelectableTreatmentRepo
                    .DeleteTreatmentLibrary(libraryId));
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
