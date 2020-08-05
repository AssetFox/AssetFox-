using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class TreatmentSchedulingCollisionDetail
    {
        public TreatmentSchedulingCollisionDetail(int year, string nameOfUnscheduledTreatment)
        {
            Year = year;
            NameOfUnscheduledTreatment = nameOfUnscheduledTreatment ?? throw new ArgumentNullException(nameof(nameOfUnscheduledTreatment));
        }

        public string NameOfUnscheduledTreatment { get; set; }

        public int Year { get; set; }
    }
}
