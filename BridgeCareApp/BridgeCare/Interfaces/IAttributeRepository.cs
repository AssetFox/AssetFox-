using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IAttributeRepository
    {
        List<AttributeModel> GetAttributes(BridgeCareContext db);
        List<AttributeSelectValuesResult> GetAttributeSelectValues(NetworkAttributes model, BridgeCareContext db);
    }
}
