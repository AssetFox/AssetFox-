using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public abstract class BaseDataSourceDTO : BaseDTO
    {
        public BaseDataSourceDTO(string typeName)
        {
            Type = typeName;
        }

        /// <summary>
        /// Name to be displayed by the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of data source, provided by the specific instance of the data source
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Indicates if the details should be obscured in the database
        /// </summary>
        /// <example>
        /// A connection string that contains passwords should be secured
        /// </example>
        public bool Secure { get; protected set; }

        /// <summary>
        /// Validates the details on the datasource
        /// </summary>
        /// <returns>
        /// True if the data source details are valid
        /// </returns>
        public abstract bool Validate();
    }
}
