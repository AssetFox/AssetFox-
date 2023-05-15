using System;
using System.Collections.Generic;
using System.Text;


namespace AppliedResearchAssociates.iAM.DTOs.Enums
{
    public class TreatmentDTOEnum
    {
        /// <summary>
        /// .
        /// </summary>
        public enum TreatmentType
        {
            /// <summary>
            /// .
            /// </summary>
            Preservation,

            /// <summary>
            /// .
            /// </summary>
            CapacityAdding,

            /// <summary>
            /// .
            /// </summary>
            Rehabilitation,

            /// <summary>
            /// .
            /// </summary>
            Replacement,

            /// <summary>
            /// .
            /// </summary>
            Maintenance,

            /// <summary>
            /// .
            /// </summary>
            Other
        }

        /// <summary>
        /// The types of assets to report.
        /// </summary>
        public enum AssetType
        {
            /// <summary>
            /// A bridge asset.
            /// </summary>
            Bridge,

            /// <summary>
            /// A box culvert asset.
            /// </summary>
            Culvert
        }
    }
}
