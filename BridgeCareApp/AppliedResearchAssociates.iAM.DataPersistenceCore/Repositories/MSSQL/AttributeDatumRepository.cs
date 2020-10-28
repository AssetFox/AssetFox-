using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository : MSSQLRepository, IAttributeDatumRepository
    {
        private readonly IMaintainableAssetRepository _maintainableAssetRepo;
        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;
        private readonly IAttributeRepository _attributeRepo;
        public AttributeDatumRepository(IMaintainableAssetRepository maintainableAssetRepo, IAttributeMetaDataRepository attributeMetaDataRepo,
            IAttributeRepository attributeRepo, IAMContext context) : base(context)
        {
            _maintainableAssetRepo =
                maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            _attributeMetaDataRepo =
                attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
        }

        public int UpdateMaintainableAssetsAssignedData(List<MaintainableAsset> maintainableAssets)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                "MetaData//AttributeMetaData", "AttributemetaData.json");
            var configurableAttributeIds = _attributeMetaDataRepo.GetAllAttributes(filePath).Select(_ => _.Id);

            // get the attribute ids off of the assigned data on the maintainable assets that have not been modified yet
            var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData.Any())
                .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct());

            // get the attribute ids that will be used to delete the assigned data where there is an attribute match in both
            // the meta data file and the data source
            var attributeIdsToBeUpdatedWithAssignedData = configurableAttributeIds
                .Intersect(networkAttributeIds).Distinct().ToList();

            if (attributeIdsToBeUpdatedWithAssignedData.Any())
            {
                // use a raw sql query to delete AttributeData
                var query =
                    $"DELETE FROM dbo.AttributeData WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAssets WHERE NetworkId = '{maintainableAssets.First().NetworkId}') AND AttributeId IN ('{string.Join("','", attributeIdsToBeUpdatedWithAssignedData)}')";
                Context.Database.ExecuteSqlRaw(query);
            }

            // convert any assigned data to their equivalent entity object types
            var assignedData = maintainableAssets
                .SelectMany(_ => _.AssignedData.Select(__ => __.ToEntity(_.Id))).ToList();

            // save any assigned data to the data source and return the count of objects indicating the number of inserted rows
            if (!assignedData.Any())
            {
                return 0;
            }

            Context.BulkInsert(assignedData);
            Context.BulkInsert(assignedData.Select(_ => _.AttributeDatumLocation).ToList());
            Context.SaveChanges();

            return assignedData.Count();
        }
    }
}
