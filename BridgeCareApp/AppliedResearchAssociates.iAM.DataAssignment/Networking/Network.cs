﻿using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataAssignment.Segmentation
{
    public class Network
    {
        public ICollection<MaintainableAsset> MaintainableAssets { get; }

        public Guid Id { get; }

        public string Name { get; set; }

        public Network(ICollection<MaintainableAsset> maintainableAssets, Guid id, string name = "")
        {
            MaintainableAssets = maintainableAssets;
            Id = id;
            Name = name;
        }
    }
}
