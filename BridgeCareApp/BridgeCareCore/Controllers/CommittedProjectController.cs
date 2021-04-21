using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BridgeCareCore.Controllers
{
    using CommittedProjectGetMethod = Func<Guid, FileInfoDTO>;
    using CommittedProjectCreateMethod = Action<Guid, List<ExcelPackage>, bool>;
    using CommittedProjectDeleteMethod = Action<Guid>;

    [Route("api/[controller]")]
    [ApiController]
    public class CommittedProjectController : BridgeCareCoreBaseController
    {
        private static ICommittedProjectService _committedProjectService;

        private readonly IReadOnlyDictionary<string, CommittedProjectGetMethod> _committedProjectExportMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectCreateMethod> _committedProjectImportMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectDeleteMethod> _committedProjectDeleteMethods;

        public CommittedProjectController(ICommittedProjectService committedProjectService,
            IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService) : base(
            esecSecurity, unitOfWork, hubService)
        {
            _committedProjectService = committedProjectService ??
                                       throw new ArgumentNullException(nameof(committedProjectService));
            _committedProjectExportMethods = CreateExportMethods();
            _committedProjectImportMethods = CreateImportMethods();
            _committedProjectDeleteMethods = CreateDeleteMethods();
        }

        private Dictionary<string, CommittedProjectGetMethod> CreateExportMethods()
        {
            FileInfoDTO GetAny(Guid simulationId)
            {
                return _committedProjectService.ExportCommittedProjectsFile(simulationId);
            }

            FileInfoDTO GetPermitted(Guid simulationId)
            {
                CheckUserSimulationAuthorization(simulationId);
                return _committedProjectService.ExportCommittedProjectsFile(simulationId);
            }

            return new Dictionary<string, CommittedProjectGetMethod>
            {
                [Role.Administrator] = GetAny,
                [Role.DistrictEngineer] = GetPermitted,
                [Role.Cwopa] = GetPermitted,
                [Role.PlanningPartner] = GetPermitted
            };
        }

        private Dictionary<string, CommittedProjectCreateMethod> CreateImportMethods()
        {
            void CreateAny(Guid simulationId, List<ExcelPackage> excelPackages, bool applyNoTreatment)
            {
                _committedProjectService.ImportCommittedProjectFiles(simulationId, excelPackages, applyNoTreatment);
            }

            void CreatePermitted(Guid simulationId, List<ExcelPackage> excelPackages, bool applyNoTreatment)
            {
                CheckUserSimulationAuthorization(simulationId);
                _committedProjectService.ImportCommittedProjectFiles(simulationId, excelPackages, applyNoTreatment);
            }

            return new Dictionary<string, CommittedProjectCreateMethod>
            {
                [Role.Administrator] = CreateAny,
                [Role.DistrictEngineer] = CreatePermitted,
                [Role.Cwopa] = CreatePermitted,
                [Role.PlanningPartner] = CreatePermitted
            };
        }

        private Dictionary<string, CommittedProjectDeleteMethod> CreateDeleteMethods()
        {
            void DeleteAny(Guid simulationId)
            {
                UnitOfWork.CommittedProjectRepo.DeleteCommittedProjects(simulationId);
            }

            void DeletePermitted(Guid simulationId)
            {
                CheckUserSimulationAuthorization(simulationId);
                UnitOfWork.CommittedProjectRepo.DeleteCommittedProjects(simulationId);
            }

            return new Dictionary<string, CommittedProjectDeleteMethod>
            {
                [Role.Administrator] = DeleteAny,
                [Role.DistrictEngineer] = DeletePermitted,
                [Role.Cwopa] = DeletePermitted,
                [Role.PlanningPartner] = DeletePermitted
            };
        }

        [HttpPost]
        [Route("ImportCommittedProjects")]
        [Authorize]
        public async Task<IActionResult> ImportCommittedProjects()
        {
            try
            {
                if (!Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Committed project files were not found.");
                }

                if (!Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var simulationId = Guid.Parse(id.ToString());

                var excelPackages = Request.Form.Files.Select(file => new ExcelPackage(file.OpenReadStream())).ToList();

                Request.Form.TryGetValue("applyNoTreatment", out var applyNoTreatmentValue);
                var applyNoTreatment = applyNoTreatmentValue.ToString() == "1";

                await Task.Factory.StartNew(() =>
                    _committedProjectImportMethods[UserInfo.Role](simulationId, excelPackages, applyNoTreatment));
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportCommittedProjects/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportCommittedProjects(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _committedProjectExportMethods[UserInfo.Role](simulationId));

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteCommittedProjects/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommittedProjects(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                    _committedProjectDeleteMethods[UserInfo.Role](simulationId));

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }
    }
}