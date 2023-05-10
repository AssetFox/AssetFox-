using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{

    public interface ISiteRepository
    {
        string GetImplementationName();

        void SetImplementationName(string name);

        string GetAgencyLogo();

        void SetAgencyLogo(Image agencyLogo);

        string GetImplementationLogo();

        void SetImplementationLogo(Image productLogo);
    }
}
