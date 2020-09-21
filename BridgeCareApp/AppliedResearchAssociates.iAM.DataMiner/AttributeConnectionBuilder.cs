﻿using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public static class AttributeConnectionBuilder
    {
        public static AttributeConnection Create(Attribute attribute)
        {
            switch (attribute.ConnectionType)
            {
            case ConnectionType.MSSQL:
                return new SqlAttributeConnection(attribute);
            case ConnectionType.MONGO_DB:
                throw new NotImplementedException("Mongo Db data retrival has not been implemented");
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{attribute.ConnectionType}\".");
            }
        }
    }
}
