﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    /// <summary>
    /// A datum specifically representing an attribute on an asset
    /// </summary>
    public class SegmentAttributeDatum
    {
        public string Name { get; private set; }
        public string TextValue { get; private set; }
        public double? NumericValue { get; private set; }
        public SegmentAttributeType Type { get; private set; }
        public string Value
        {
            get
            {
                return NumericValue == null ? TextValue : NumericValue.ToString();
            }
        }

        public SegmentAttributeDatum(string name, string value)
        {
            Name = name;

            double numValue;
            if (double.TryParse(value, out numValue))
            {
                TextValue = String.Empty;
                NumericValue = numValue;
                Type = SegmentAttributeType.Number;
            }
            else
            {
                TextValue = value;
                NumericValue = null;
                Type = SegmentAttributeType.String;
            }
        }
    }

    public enum SegmentAttributeType
    {
        String,
        Number
    }
}