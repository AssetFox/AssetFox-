using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Paging.Generics;
using Org.BouncyCastle.Asn1.Ocsp;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services
{
    public class RemainingLifeLimitPagingService : IRemainingLifeLimitPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public RemainingLifeLimitPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        List<RemainingLifeLimitDTO> IPagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>.SearchRows(List<RemainingLifeLimitDTO> rows, string search) => throw new NotImplementedException();
        List<RemainingLifeLimitDTO> IPagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>.OrderByColumn(List<RemainingLifeLimitDTO> rows, string sortColumn, bool isDescending) => throw new NotImplementedException();
        List<RemainingLifeLimitDTO> IPagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>.GetScenarioRows(Guid scenarioId)
        {
            return _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(scenarioId);
        }
        List<RemainingLifeLimitDTO> IPagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>.GetLibraryRows(Guid libraryId)
        {
            return _unitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(libraryId);
        }
        List<RemainingLifeLimitDTO> IPagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>.CreateAsNewDataset(List<RemainingLifeLimitDTO>  rows)
        {
            rows.ForEach(_ =>
            {
                _.Id = Guid.NewGuid();
                _.CriterionLibrary.Id = Guid.NewGuid();
            });

            return rows;
        }

        
    }
}
