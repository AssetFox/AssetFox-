using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting;
using HotChocolate;
using HotChocolate.Data;

namespace BridgeCareCore.GraphQL
{
    public class Query
    {
        [UseFiltering]
        [UseSorting]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork) =>
            _unitOfWork.SimulationRepo.GetAllScenario();

        [UseFiltering]
        [UseSorting]
        public SimulationOutput GetSimulationOutputs([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId) =>
            _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJsonGraphQL(new Guid(simulationId));

    }
}
