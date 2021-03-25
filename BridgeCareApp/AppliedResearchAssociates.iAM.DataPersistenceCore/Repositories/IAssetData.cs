using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    /// <summary>
    /// Retrieves current attribute data for a specific asset
    /// </summary>
    /// <remarks>
    /// In the future, this could be expanded to provide historical and future data (given a specifc scenario)
    /// </remarks>
    public interface IAssetData
    {
        /// <summary>
        /// A lookup table for the key values in the system
        /// </summary>
        /// <remarks>
        /// This does the work of a primary key(s) in an RDBMS and multiple can exist
        /// </remarks>
        Dictionary<string,List<KeySegmentDatum>> KeyProperties { get; }

        /// <summary>
        /// Provides all attributes for a given asset given a key value
        /// </summary>
        /// <param name="keyName">Name of the attribute that contains the key</param>
        /// <param name="keyValue">Value of the key attribute</param>
        /// <returns></returns>
        List<SegmentAttributeDatum> GetAssetAttributes(string keyName, string keyValue);
    }
}
