using System;

namespace Assets.src.PathFinding.MapModelComponents
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
                throw new ArgumentException();
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


    }
}