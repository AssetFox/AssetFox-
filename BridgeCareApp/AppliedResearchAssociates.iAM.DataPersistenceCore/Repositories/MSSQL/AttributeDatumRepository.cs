using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository : IAttributeDatumRepository
    {
        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public AttributeDatumRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public int UpdateAssignedData(List<MaintainableAsset> maintainableAssets)
        {
            // get all the configurable attributes
            var configurableAttributes = _unitOfWork.AttributeMetaDataRepo.GetAllAttributes();

            // insert/update configurable attributes
            _unitOfWork.AttributeRepo.UpsertAttributes(configurableAttributes);

            // get the attribute ids off of the assigned data on the maintainable assets that have
            // not been modified yet
            var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData.Any())
                .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct());

            // get the attribute ids that will be used to delete the assigned data where there is an
            // attribute match in both the meta data file and the data source
            var attributeIdsToBeUpdatedWithAssignedData = configurableAttributes.Select(_ => _.Id).ToList()
                .Intersect(networkAttributeIds).Distinct().ToList();

            if (attributeIdsToBeUpdatedWithAssignedData.Any())
            {
                // use a raw sql query to delete AssignedData
                var query =
                    $"DELETE FROM dbo.AttributeDatum WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAsset WHERE NetworkId = '{maintainableAssets.First().NetworkId}') AND AttributeId IN ('{string.Join("','", attributeIdsToBeUpdatedWithAssignedData)}')";
                _unitOfWork.Context.Database.ExecuteSqlRaw(query);
                _unitOfWork.Context.SaveChanges();
            }

            // convert any assigned data to their equivalent entity object types
            var assignedData = maintainableAssets
                .SelectMany(_ => _.AssignedData.Select(__ => __.ToEntity(_.Id))).ToList();

            // save any assigned data to the data source and return the count of objects indicating
            // the number of inserted rows
            if (!assignedData.Any())
            {
                return 0;
            }

            _unitOfWork.Context.BulkInsertOrUpdate(assignedData);
            _unitOfWork.Context.BulkInsertOrUpdate(assignedData.Select(_ => _.AttributeDatumLocation).ToList());
            _unitOfWork.Context.SaveChanges();

            return assignedData.Count();
        }
    }
}
