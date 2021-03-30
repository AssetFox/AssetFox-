using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PennDOTMaintainableAssetDataRepository : IAssetData
    {
        UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public PennDOTMaintainableAssetDataRepository(UnitOfWork.UnitOfDataPersistenceWork uow)
        {
            _unitOfWork = uow;
            var network = _unitOfWork.NetworkRepo.GetPennDotNetwork();
            var assets = _unitOfWork.Context.MaintainableAsset.Where(_ => _.NetworkId == network.Id);
            KeyProperties = new Dictionary<string, List<KeySegmentDatum>>();
            var brkeyDatum = new List<KeySegmentDatum>();
            var bmsidDatum = new List<KeySegmentDatum>();

            foreach (var asset in assets)
            {
                brkeyDatum.Add(new KeySegmentDatum()
                {
                    SegmentId = asset.Id,
                    //KeyValue = new SegmentAttributeDatum("BRKEY_", asset.Fac)
                });
            }
        }

        public Dictionary<string, List<KeySegmentDatum>> KeyProperties { get; private set; }

        public List<SegmentAttributeDatum> GetAssetAttributes(string keyName, string keyValue) => throw new NotImplementedException();
        public Dictionary<int, SegmentAttributeDatum> GetAttributeValueHistory(string keyName, string keyValue, string attribute) => throw new NotImplementedException();
    }
}
