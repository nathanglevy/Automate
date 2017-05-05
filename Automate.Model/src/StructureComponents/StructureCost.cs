using System;
using System.Collections.Generic;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;

namespace Automate.Model.StructureComponents
{
    public static class StructureCost
    {
        private static Dictionary<StructureType, Dictionary<Component, int>> _structureCosts = new Dictionary<StructureType, Dictionary<Component, int>>()
        {
            { StructureType.SmallFire , new Dictionary<Component, int>()
                {
                    { Component.Wood, 10}
                }
            },
            { StructureType.Storage , new Dictionary<Component, int>()
                {
                    { Component.IronOre, 10}, { Component.Wood, 10 }
                }
            },
        };

        public static Dictionary<Component, int> GetStructureCost(StructureType structureType)
        {
            if (!_structureCosts.ContainsKey(structureType))
                throw new ArgumentException("Current structure does not have a defined cost!");
            return _structureCosts[structureType];
        }

    }
}