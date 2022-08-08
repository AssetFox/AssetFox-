﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSchedulingCollisionDetailMapper
    {
        public static TreatmentSchedulingCollisionDetailEntity ToEntity(
            TreatmentSchedulingCollisionDetail domain,
            Guid assetDetailId)
        {
            var id = Guid.NewGuid();
            var entity = new TreatmentSchedulingCollisionDetailEntity
            {
                Id = id,
                NameOfUnscheduledTreatment = domain.NameOfUnscheduledTreatment,
                AssetDetailId = assetDetailId,
            };
            return entity;
        }

        public static List<TreatmentSchedulingCollisionDetailEntity> ToEntityList(
            List<TreatmentSchedulingCollisionDetail> domainList,
            Guid assetDetailId)
        {
            var entityList = new List<TreatmentSchedulingCollisionDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, assetDetailId);
                entityList.Add(entity);
            }
            return entityList;
        }
    }
}
