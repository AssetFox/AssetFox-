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
    public class AttributeDatumRepository : MSSQLRepository, IAttributeDatumRepository
    {
        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;
        private readonly IAttributeRepository _attributeRepo;

        public AttributeDatumRepository(IAttributeMetaDataRepository attributeMetaDataRepo,
            IAttributeRepository attributeRepo, IAMContext context) : base(context)
        {
            _attributeMetaDataRepo =
                attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
        }

        public int UpdateAssignedData(List<MaintainableAsset> maintainableAssets)
        {
            // get all the configurable attributes
            var configurableAttributes = _attributeMetaDataRepo.GetAllAttributes();

            // insert/update configurable attributes
            _attributeRepo.UpsertAttributes(configurableAttributes);

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
                Context.Database.ExecuteSqlRaw(query);
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

            Context.BulkInsertOrUpdate(assignedData);
            Context.BulkInsertOrUpdate(assignedData.Select(_ => _.AttributeDatumLocation).ToList());
            Context.SaveChanges();

            return assignedData.Count();
        }
    }
}
