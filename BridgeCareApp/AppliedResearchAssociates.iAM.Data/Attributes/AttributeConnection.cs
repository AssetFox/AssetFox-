using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public abstract class AttributeConnection
    {
        public Attribute Attribute { get; }

        public abstract IEnumerable<IAttributeDatum> GetData<T>();

        public const string DateColumnName = "DATE_";
        public const string DataColumnName = "DATA_";
        public const string LocationIdentifierString = "LOCATION_IDENTIFIER";

        // Commands in metaData.json should now look more like this:
        // "SELECT CAST(BRKEY AS VARCHAR(MAX)) & '-' & BRIDGE_ID AS LOCATION_IDENTIFIER, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",

        protected AttributeConnection(Attribute attribute) => Attribute = attribute;
    }
}
