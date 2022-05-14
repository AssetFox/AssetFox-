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
        /// Maps tyhe data source's details to a string for data persistence
        /// </summary>
        public abstract string MapDetails();

        /// <summary>
        /// Builds the concrete data source object with the details provided as a string
        /// </summary>
        /// <param name="details">
        /// Data source details provided as a string such as a connection string or JSON object
        /// </param>
        public abstract void PopulateDetails(string details);
    }
}
