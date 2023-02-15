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
    public class RemainingLifeLimitPagingService : PagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>, IRemainingLifeLimitPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public RemainingLifeLimitPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override List<RemainingLifeLimitDTO> GetScenarioRows(Guid scenarioId)
        {
            return _unitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(scenarioId);
        }
        protected override List<RemainingLifeLimitDTO> GetLibraryRows(Guid libraryId)
        {
            return _unitOfWork.RemainingLifeLimitRepo.GetRemainingLifeLimitsByLibraryId(libraryId);
        }
        protected override List<RemainingLifeLimitDTO> CreateAsNewDataset(List<RemainingLifeLimitDTO>  rows)
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
