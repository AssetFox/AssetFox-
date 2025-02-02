﻿using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IInventoryRepository
    {
        InventoryModel GetInventoryByBMSId(string bmsId, BridgeCareContext db);

        InventoryModel GetInventoryByBRKey(string brKey, BridgeCareContext db);

        List<InventorySelectionModel> GetInventorySelectionModels(BridgeCareContext db, UserInformationModel userInformation);
    }
}
