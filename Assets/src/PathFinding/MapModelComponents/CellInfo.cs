using System;
using System.Collections.Generic;

namespace Assets.src.PathFinding.MapModelComponents
{
    /// <summary>
    /// Class which holds information on a single cell in a map.
    /// Contains:
    /// <list type="bullet">
    /// <item><description>The weight of the cell for pathing algorithms</description></item>
    /// <item><description>Whether the cell is passable / blocked off</description></item>
    /// <item><description>What directions are accessible from the current cell</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class CellInfo
    {
        private bool Passable;
        private float Weight;
        private List<Coordinate> BlockedDirections;

        public CellInfo(bool isPassable, float weight, List<Coordinate> blockedDirections) {
            if (weight < 0)
                throw new ArgumentException();
            Passable = isPassable;
            Weight = weight;
            BlockedDirections = blockedDirections;
        }

        public float GetWeight()
        {
            return Weight;
        }

        public bool IsPassable() {
            return Passable;
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            CellInfo cellInfo = (CellInfo)obj;
            return (Math.Abs(Weight - cellInfo.Weight) < 0.0001) &&
                (Passable == cellInfo.Passable) &&
                ((BlockedDirections == null && cellInfo.BlockedDirections == null) ||
                (BlockedDirections != null && BlockedDirections.Equals(cellInfo.BlockedDirections)));
        }

        public override int GetHashCode() {
            return 1;
        }

    }
}

