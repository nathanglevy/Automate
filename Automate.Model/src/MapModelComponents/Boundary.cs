using System;
using System.Collections.Generic;

namespace Automate.Model.src.MapModelComponents
{
    /// <summary>
    /// Class which describes a 3D boundary.<para />
    /// Contains: Two Coordinates which describe the boundary limits.
    /// </summary>
    /// <remarks><b>This class is immutable!</b></remarks>
    [Serializable]
    public class Boundary
    {
        public readonly Coordinate topLeft;
        public readonly Coordinate bottomRight;

        /// <summary>
        /// Constructor accepts two coordinates and defines a boundary with them <para />
        /// Throws an Argument Exception if the first argument is not top left relative to the other.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        public Boundary(Coordinate topLeft, Coordinate bottomRight) {
            if (!(bottomRight >= topLeft))
                throw new ArgumentException("Bottom Right Is Not Greater Or Equal To TopLeft");
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
            //TODO: Make the boundary constructor smarter so it can overcome the exception
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            Boundary boundary = (Boundary)obj;
            return (topLeft.Equals(boundary.topLeft) && (bottomRight.Equals(boundary.bottomRight)));
        }

        public override int GetHashCode() {
            return topLeft.GetHashCode() + bottomRight.GetHashCode() * 2048;
        }

        /// <summary>
        /// Checks if a coordinate is within this boundary.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns>true if coordinate is within boundary, false otherwise</returns>
        public bool IsCoordinateInBoundary(Coordinate coordinate) {
            return (coordinate >= topLeft) && (coordinate <= bottomRight);
        }

        private HashSet<Coordinate> GetBoundaryCoordinatePoints()
        {
            HashSet<Coordinate> result = new HashSet<Coordinate>();
            result.Add(new Coordinate(topLeft.x,        topLeft.y,      topLeft.z));
            result.Add(new Coordinate(topLeft.x,        topLeft.y,      bottomRight.z));
            result.Add(new Coordinate(topLeft.x,        bottomRight.y,  bottomRight.z));
            result.Add(new Coordinate(bottomRight.x,    bottomRight.y,  bottomRight.z));
            result.Add(new Coordinate(bottomRight.x,    bottomRight.y,  topLeft.z));
            result.Add(new Coordinate(bottomRight.x,    topLeft.y,      topLeft.z));
            result.Add(new Coordinate(topLeft.x,        bottomRight.y,  topLeft.z));
            result.Add(new Coordinate(bottomRight.x,    topLeft.y,      bottomRight.z));
            return result;
        }

        public bool IsBoundaryDisjointToBoundary(Boundary boundary)
        {
            bool currentInclusive = false;
            foreach (Coordinate boundaryCoordinatePoint in GetBoundaryCoordinatePoints())
            {
                if (boundary.IsCoordinateInBoundary(boundaryCoordinatePoint))
                    currentInclusive = true;
            }
            bool otherInclusive = false;
            foreach (Coordinate boundaryCoordinatePoint in boundary.GetBoundaryCoordinatePoints()) {
                if (IsCoordinateInBoundary(boundaryCoordinatePoint))
                    otherInclusive = true;
            }
            return !(currentInclusive || otherInclusive);
        }

        public HashSet<Coordinate> GetListOfCoordinatesInBoundary()
        {
            HashSet<Coordinate> result = new HashSet<Coordinate>();
            for(int x = topLeft.x; x <= bottomRight.x; x++)
                for(int y = topLeft.y; y <= bottomRight.y; y++)
                    for (int z = topLeft.z; z <= bottomRight.z; z++)
                        result.Add(new Coordinate(x, y, z));
            return result;
        }


    }
}