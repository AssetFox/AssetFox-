using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

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
            var configurableAttributeIds = _attributeMetaDataRepo.All(filePath).Select(_ => _.Id);

            // get the attribute ids off of the assigned data on the maintainable assets that have not been modified yet
            var networkAttributeIds = maintainableAssets
                .SelectMany(_ =>
                {
                    return _.AssignedData != null
                        ? _.AssignedData.Select(__ => __.Attribute.Id).Distinct()
                        : new List<Guid>();
                });

            // get the attribute ids that will be used to delete the assigned data where there is an attribute match in both
            // the meta data file and the data source
            var attributeIdsToBeUpdatedWithAssignedData = configurableAttributeIds
                .Intersect(networkAttributeIds).Distinct().ToList();

            if (attributeIdsToBeUpdatedWithAssignedData.Any())
            {
                var query =
                    $"DELETE FROM dbo.AttributeData WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAssets WHERE NetworkId = '{maintainableAssets.First().NetworkId}') AND AttributeId IN ('{string.Join("','", attributeIdsToBeUpdatedWithAssignedData)}')";
                Context.Database.ExecuteSqlRaw(query);
            }

            var assignedData = maintainableAssets
                .SelectMany(_ => _.AssignedData.Select(__ =>
                {
                    // this must be done to ensure the assigned data receives a matching location with its maintainable asset in the data source
                    if (__ is AttributeDatum<double> numericAttributeDatum)
                    {
                        return new AttributeDatum<double>(__.Attribute, numericAttributeDatum.Value, _.Location, __.TimeStamp)
                            .ToEntity(_.Id);
                    }

                    // this must be done to ensure the assigned data receives a matching location with its maintainable asset in the data source
                    if (__ is AttributeDatum<string> textAttributeDatum)
                    {
                        return new AttributeDatum<string>(__.Attribute, textAttributeDatum.Value, _.Location, __.TimeStamp)
                            .ToEntity(_.Id);
                    }

                    return null;
                }))
                .Where(_ => _ != null)
                .ToList();

            if (!assignedData.Any())
            {
                return 0;
            }

            Context.BulkInsert(assignedData);
            Context.SaveChanges();

            return assignedData.Count();
        }
    }
}
