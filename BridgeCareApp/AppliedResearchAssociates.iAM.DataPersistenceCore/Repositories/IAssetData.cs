using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;

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
        /// <returns>List of most recent values for all attributes on a requested segment</returns>
        List<SegmentAttributeDatum> GetAssetAttributes(string keyName, string keyValue);

        /// <summary>
        /// Provides the time value history for a given attribute on a given asset
        /// </summary>
        /// <param name="keyName">Name of the attribute that contains the key</param>
        /// <param name="keyValue">Value of the key attribute</param>
        /// <param name="attribute">Name of attribute</param>
        /// <returns>All values for a specific attribute on a specific asset in the database with a primary key of year</returns>
        Dictionary<int, SegmentAttributeDatum> GetAttributeValueHistory(string keyName, string keyValue, string attribute);

        /// <summary>
        /// Provides an ordered table of tuples to the user for use in lookups
        /// </summary>
        /// <param name="keyFieldNames">List of desired key fields</param>
        /// <returns></returns>
        List<List<string>> GetKeyPropertiesTable(List<string> keyFieldNames);

        /// <summary>
        /// Given a dictionary based query using key attributes, returns the key attributes matching that query
        /// </summary>
        /// <param name="queryParameters">Dictionary of attributes and filtering parameters</param>
        /// <param name="networkType">Main or Raw</param>
        /// <param name="previousQuery">Provides a previous query to base this on.  The previous query is pulled from all asssets in the provided network type if not specified</param>
        /// <returns>A list of assets and their key properties</returns>
        List<MaintainableAssetQueryDTO> QueryKeyAttributes(Dictionary<AttributeDTO, string> queryParameters, NetworkTypes networkType = NetworkTypes.Main, List<MaintainableAssetQueryDTO> previousQuery = null);
    }
}
