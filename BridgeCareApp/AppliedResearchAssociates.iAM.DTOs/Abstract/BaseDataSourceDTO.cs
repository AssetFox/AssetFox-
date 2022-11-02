using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public abstract class BaseDataSourceDTO : BaseDTO
    {
        private string _type;

        public BaseDataSourceDTO(string typeName)
        {
            _type = typeName;
        }

        /// <summary>
        /// Name to be displayed by the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of data source, provided by the specific instance of the data source
        /// </summary>
        public virtual string Type {
            get { return _type; }
            set { /* Do Nothing */ }
        }

        /// <summary>
        /// Indicates if the details should be obscured in the database
        /// </summary>
        /// <example>
        /// A connection string that contains passwords should be secured
        /// </example>
        public bool Secure { get; protected set; }

        /// <summary>
        /// Owner
        /// </summary>
        public Guid CreatedBy { get; set;}

        /// <summary>
        /// Validates the details on the datasource
        /// </summary>
        /// <returns>
        /// True if the data source details are valid
        /// </returns>
        public abstract bool Validate();
    }
}
