using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public class SectionSummaryDetail
    {
        public SectionSummaryDetail(Section section)
        {
            if (section is null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            FacilityName = section.Facility.Name;
            SectionName = section.Name;
        }

        [JsonConstructor]
        public SectionSummaryDetail(string facilityName, string sectionName)
        {
            FacilityName = facilityName ?? throw new ArgumentNullException(nameof(facilityName));
            SectionName = sectionName ?? throw new ArgumentNullException(nameof(sectionName));
        }

        public string FacilityName { get; }

        public string SectionName { get; }

        public Dictionary<string, double> ValuePerNumericAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();

        internal SectionSummaryDetail(SectionSummaryDetail original)
        {
            FacilityName = original.FacilityName;
            SectionName = original.SectionName;

            ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute);
            ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);
        }
    }
}
