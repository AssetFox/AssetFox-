using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class TreatmentSchedulingCollisionDetail
    {
        public TreatmentSchedulingCollisionDetail(int year, string nameOfUnscheduledTreatment)
        {
            Year = year;
            NameOfUnscheduledTreatment = nameOfUnscheduledTreatment ?? throw new ArgumentNullException(nameof(nameOfUnscheduledTreatment));
        }

        /// <summary>
        /// .
        /// </summary>
        public string NameOfUnscheduledTreatment { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int Year { get; set; }

        internal TreatmentSchedulingCollisionDetail(TreatmentSchedulingCollisionDetail original)
        {
            Year = original.Year;
            NameOfUnscheduledTreatment = original.NameOfUnscheduledTreatment;
        }
    }
}
