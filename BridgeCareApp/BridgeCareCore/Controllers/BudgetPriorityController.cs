using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using BudgetPriorityUpsertMethod = Action<UserInfoDTO, Guid, BudgetPriorityLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IReadOnlyDictionary<string, BudgetPriorityUpsertMethod> _budgetPriorityUpsertMethods;

        public BudgetPriorityController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _budgetPriorityUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, BudgetPriorityUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, BudgetPriorityLibraryDTO dto)
            {
                _unitOfWork.BudgetPriorityRepo
                    .UpsertBudgetPriorityLibrary(dto, simulationId, userInfo);
                _unitOfWork.BudgetPriorityRepo
                    .UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, BudgetPriorityLibraryDTO dto) =>
                _unitOfWork.BudgetPriorityRepo.UpsertPermitted(userInfo, simulationId, dto);

            return new Dictionary<string, BudgetPriorityUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
        [Authorize]
        public async Task<IActionResult> BudgetPriorityLibraries()
        {
            try
            {
                var result = await _unitOfWork.BudgetPriorityRepo
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
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertBudgetPriorityLibrary(Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);
                _unitOfWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _budgetPriorityUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
                });
                _unitOfWork.Commit();
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                return Unauthorized(e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetPriorityLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfWork.BudgetPriorityRepo
                    .DeleteBudgetPriorityLibrary(libraryId));
                _unitOfWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
