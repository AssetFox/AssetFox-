﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AggregatedResultTestSetup
    {
        public static void AddAggregatedResultsToDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, List<IamAttribute> resultAttributes, List<IAggregatedResult> results)
        {
            foreach (var asset in maintainableAssets)
            {
                var resultId = Guid.NewGuid();
                var resultData = new List<(IamAttribute, (int, double))>();

                foreach (var attribute in resultAttributes)
                {
                    var resultDatum = (
                        attribute, (2022, 1.23));
                    resultData.Add(resultDatum);
                }
                var result = new AggregatedResult<double>(
                    resultId,
                    asset,
                    resultData
                    );

                results.Add(result);
            }
            unitOfWork.AggregatedResultRepo.AddAggregatedResults(results);
        }

    }
}
