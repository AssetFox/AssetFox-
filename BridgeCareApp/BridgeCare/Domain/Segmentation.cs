using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL;

namespace BridgeCare.Domain
{
    public class Segmentation
    {
        private readonly IRepository<SegmentationRule> SegmentationRepository;

        public Segmentation(IRepository<SegmentationRule> segmentationRepository)
        {
            SegmentationRepository = segmentationRepository;
        }

        public void Run()
        {
        }
    }
}
