using System;
using System.Collections.Generic;

namespace Assets.src.PathFinding.MapModelComponents
{
    [Serializable]
    public class CellInfo
    {
        private bool Passable;
        private float Weight;
        public List<Coordinate> _blockedDirections;

        public CellInfo(bool isPassable, float weight, List<Coordinate> blockedDirections) {
            if (weight < 0)
                throw new ArgumentException();
            Passable = isPassable;
            Weight = weight;
            _blockedDirections = blockedDirections;
        }

        public float GetWeight()
        {
            return Weight;
        }

        public bool IsPassable() {
            return Passable;
        }

    }
}

