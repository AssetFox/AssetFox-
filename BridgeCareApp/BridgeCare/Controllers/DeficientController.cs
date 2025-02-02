﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    using DeficientLibraryGetMethod = Func<int, UserInformationModel, DeficientLibraryModel>;
    using DeficientLibrarySaveMethod = Func<DeficientLibraryModel, UserInformationModel, DeficientLibraryModel>;

    public class DeficientController: ApiController
    {
        private readonly IDeficientRepository repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching a target library</summary>
        private readonly IReadOnlyDictionary<string, DeficientLibraryGetMethod> DeficientLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving a target library</summary>
        private readonly IReadOnlyDictionary<string, DeficientLibrarySaveMethod> DeficientLibrarySaveMethods;

        public DeficientController() { }

        public DeficientController(IDeficientRepository repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            DeficientLibraryGetMethods = CreateGetMethods();
            DeficientLibrarySaveMethods = CreateSaveMethods();
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for getting deficient libraries
        /// </summary>
        private Dictionary<string, DeficientLibraryGetMethod> CreateGetMethods()
        {
            DeficientLibraryModel GetAnyLibrary(int id, UserInformationModel userInformation) =>
                repo.GetAnySimulationDeficientLibrary(id, db);
            DeficientLibraryModel GetPermittedLibrary(int id, UserInformationModel userInformation) =>
                repo.GetPermittedSimulationDeficientLibrary(id, db, userInformation.Name);

            return new Dictionary<string, DeficientLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyLibrary,
                [Role.DISTRICT_ENGINEER] = GetPermittedLibrary,
                [Role.CWOPA] = GetAnyLibrary,
                [Role.PLANNING_PARTNER] = GetPermittedLibrary
            };
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for saving deficient libraries
        /// </summary>
        private Dictionary<string, DeficientLibrarySaveMethod> CreateSaveMethods()
        {
            DeficientLibraryModel SaveAnyLibrary(DeficientLibraryModel model, UserInformationModel userInformation) =>
                repo.SaveAnySimulationDeficientLibrary(model, db);
            DeficientLibraryModel SavePermittedLibrary(DeficientLibraryModel model, UserInformationModel userInformation) =>
                repo.SavePermittedSimulationDeficientLibrary(model, db, userInformation.Name);

            return new Dictionary<string, DeficientLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyLibrary,
                [Role.DISTRICT_ENGINEER] = SavePermittedLibrary,
                [Role.CWOPA] = SavePermittedLibrary,
                [Role.PLANNING_PARTNER] = SavePermittedLibrary
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's deficient library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioDeficientLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationDeficientLibrary(int id)
        {
            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
            return Ok(DeficientLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's deficient library data
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioDeficientLibrary")]
        [ModelValidation("The deficient data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult SaveSimulationDeficientLibrary([FromBody] DeficientLibraryModel model)
        {
            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
            return Ok(DeficientLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}
