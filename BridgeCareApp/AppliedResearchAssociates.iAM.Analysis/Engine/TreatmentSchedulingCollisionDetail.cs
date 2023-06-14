using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// Describes a specific scheduling collision for this treatment
    /// </summary>
    public sealed class TreatmentSchedulingCollisionDetail
    {
        public TreatmentSchedulingCollisionDetail(int year, string nameOfUnscheduledTreatment)
        {
            Year = year;
            NameOfUnscheduledTreatment = nameOfUnscheduledTreatment ?? throw new ArgumentNullException(nameof(nameOfUnscheduledTreatment));
        }

        /// <summary>
        /// Name of the treatment that was not applied
        /// </summary>
        public string NameOfUnscheduledTreatment { get; set; }

        /// <summary>
        /// Year where the conflict occured
        /// </summary>
        public int Year { get; set; }

        internal TreatmentSchedulingCollisionDetail(TreatmentSchedulingCollisionDetail original)
        {
            Year = original.Year;
            NameOfUnscheduledTreatment = original.NameOfUnscheduledTreatment;
        }
    }
}
