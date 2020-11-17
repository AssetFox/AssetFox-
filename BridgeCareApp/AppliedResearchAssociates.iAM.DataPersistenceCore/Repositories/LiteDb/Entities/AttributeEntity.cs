﻿using System;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class AttributeEntity
    {
        public Guid Id { get; set; }

        public string DataType { get; set; }

        public string Name { get; set; }

        public string AggregationRuleType { get; set; }

        public string Command { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public string ConnectionString { get; set; }
        public string DefaultValue { get; set; }
        public double? Maximum { get; set; }
        public double? Minimum { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsAscending { get; set; }
    }
}
