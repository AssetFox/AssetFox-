﻿using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationAnalysisDetailRepository : ISimulationAnalysisDetailRepository
    {
        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public SimulationAnalysisDetailRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void UpsertSimulationAnalysisDetail(SimulationAnalysisDetailDTO dto)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == dto.SimulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.SimulationId}.");
            }

            _unitOfWork.Context.AddOrUpdate(dto.ToEntity(), dto.SimulationId);
            _unitOfWork.Context.SaveChanges();
        }

        public SimulationAnalysisDetailDTO GetSimulationAnalysisDetail(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfWork.Context.SimulationAnalysisDetail.Any(_ => _.SimulationId == simulationId))
            {
                return new SimulationAnalysisDetailDTO();
            }

            return _unitOfWork.Context.SimulationAnalysisDetail.Single(_ => _.SimulationId == simulationId).ToDto();
        }
    }
}