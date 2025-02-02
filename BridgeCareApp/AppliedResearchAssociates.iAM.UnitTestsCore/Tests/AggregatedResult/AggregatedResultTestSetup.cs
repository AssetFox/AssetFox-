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
        public static void AddNumericAggregatedResultsToDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, List<IamAttribute> resultAttributes, double value = 1.23)
        {
            var results = new List<IAggregatedResult>();
            foreach (var asset in maintainableAssets)
            {
                var resultId = Guid.NewGuid();
                var resultData = new List<(IamAttribute, (int, double))>();

                foreach (var attribute in resultAttributes)
                {
                    var resultDatum = (
                        attribute, (2022, value));
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
        /// <summary>This deletes any former aggregated results.
        /// Therefore, you can only call something like it once per test.</summary> 
        public static void SetTextAggregatedResultsInDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, List<IamAttribute> resultAttributes, string text = "AggregatedResult")
        {
            var results = new List<IAggregatedResult>();            
            foreach (var asset in maintainableAssets)
            {
                var resultId = Guid.NewGuid();
                var resultData = new List<(IamAttribute, (int, string))>();

                foreach (var attribute in resultAttributes)
                {
                    var resultDatum = (
                        attribute, (2022, text));
                    resultData.Add(resultDatum);
                }
                var result = new AggregatedResult<string>(
                    resultId,
                    asset,
                    resultData
                    );

                results.Add(result);
            }
            unitOfWork.AggregatedResultRepo.AddAggregatedResults(results);
        }

        public static void SetTextAggregatedResultsInDb(
            IUnitOfWork unitOfWork,
            List<MaintainableAsset> maintainableAssets,
            Dictionary<string, List<IamAttribute>> resultsForAttributes
            )
        {
            var results = new List<IAggregatedResult>();
            foreach (var asset in maintainableAssets)
            {
                var resultId = Guid.NewGuid();
                var resultData = new List<(IamAttribute, (int, string))>();

                foreach (var key in resultsForAttributes.Keys)
                {
                    foreach (var attribute in resultsForAttributes
                        [key])
                    {
                        var resultDatum = (
                            attribute, (2022, key));
                        resultData.Add(resultDatum);
                    }
                }
                var result = new AggregatedResult<string>(
                    resultId,
                    asset,
                    resultData
                    );

                results.Add(result);
            }
            unitOfWork.AggregatedResultRepo.AddAggregatedResults(results);
        }

        public static void SetBothAggregatedResultsInDb(IUnitOfWork unitOfWork, List<MaintainableAsset> maintainableAssets, List<IamAttribute> resultNumericAttributes, List<IamAttribute> resultTextAttributes, string text = "AggregatedResult")
        {
            var results = new List<IAggregatedResult>();
            foreach (var asset in maintainableAssets)
            {
                var resultId = Guid.NewGuid();
                var resultNumericData = new List<(IamAttribute, (int, double))>();
                var resultTextData = new List<(IamAttribute, (int, string))>();

                foreach (var attribute in resultNumericAttributes)
                {
                    var resultDatum = (
                        attribute, (2022, 1.23));
                    resultNumericData.Add(resultDatum);
                }
                foreach (var attribute in resultTextAttributes)
                {
                    var resultDatum = (
                        attribute, (2022, text));
                    resultTextData.Add(resultDatum);
                }
                var resultNumeric = new AggregatedResult<double>(
                    resultId,
                    asset,
                    resultNumericData
                    );
                results.Add(resultNumeric);
                var resultText = new AggregatedResult<string>(
                    resultId,
                    asset,
                    resultTextData
                    );
                results.Add(resultText);
            }
            unitOfWork.AggregatedResultRepo.AddAggregatedResults(results);
        }
    }
}
