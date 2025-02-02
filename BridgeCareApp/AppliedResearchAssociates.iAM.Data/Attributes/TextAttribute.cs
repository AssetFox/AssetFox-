﻿using System;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class TextAttribute : Attribute
    {
        public TextAttribute(string defaultValue,
            Guid id,
            string name,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString,
            bool isCalculated,
            bool isAscending,
            Guid? dataSourceId)
            : base(id, name, "STRING", ruleType, command, connectionType, connectionString, dataSourceId, isCalculated, isAscending) =>
            DefaultValue = defaultValue;

        public string DefaultValue { get; }
    }
}
