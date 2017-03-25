using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.src.Model.MapModelComponents
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
        private bool _passable;
        private int _weight;
        //change this to blocked entry and blocked exit list
        private HashSet<Coordinate> BlockedExitDirections;
        private HashSet<Coordinate> BlockedEntranceDirections;
        //private Dictionary<string, int> DirectionalWeightRatio;
        //add "penalty" or "bonus" for direction

        public CellInfo(bool isPassable, int weight) {
            if (weight < 0)
                throw new ArgumentException();
            _passable = isPassable;
            _weight = weight;
            BlockedExitDirections = new HashSet<Coordinate>();
            BlockedEntranceDirections = new HashSet<Coordinate>();
        }

        [JsonConstructor]
        public CellInfo(bool isPassable, int weight, HashSet<Coordinate> blockedExitDirections, HashSet<Coordinate> blockedEntranceDirections) {
            if (weight < 0)
                throw new ArgumentException();
            _passable = isPassable;
            _weight = weight;
            if ((blockedExitDirections == null) || (blockedEntranceDirections == null))
                throw new ArgumentNullException();
            BlockedExitDirections = new HashSet<Coordinate>(blockedExitDirections);
            BlockedEntranceDirections = new HashSet<Coordinate>(blockedEntranceDirections);
        }

        public int GetWeight()
        {
            return _weight;
        }

        public bool IsPassable() {
            return _passable;
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            CellInfo cellInfo = (CellInfo)obj;
            return (_weight == cellInfo._weight) &&
                (_passable == cellInfo._passable) &&
                (BlockedExitDirections.SetEquals(cellInfo.BlockedExitDirections)) &&
                (BlockedEntranceDirections.SetEquals(cellInfo.BlockedEntranceDirections)) ;
        }

        public override int GetHashCode() {
            return 1;
        }

    }
}

