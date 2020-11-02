using System;
using System.Collections.Generic;
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
            Area = section.Area;
        }

        [JsonConstructor]
        public SectionSummaryDetail(double area, string facilityName, string sectionName)
        {
            Area = area;
            FacilityName = facilityName ?? throw new ArgumentNullException(nameof(facilityName));
            SectionName = sectionName ?? throw new ArgumentNullException(nameof(sectionName));
        }

        public double Area { get; }

        public string FacilityName { get; }

        public string SectionName { get; }

        public Dictionary<string, double> ValuePerNumericAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();

        internal SectionSummaryDetail(SectionSummaryDetail original)
        {
            FacilityName = original.FacilityName;
            SectionName = original.SectionName;
            Area = original.Area;

            ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute);
            ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);
        }
    }
}
