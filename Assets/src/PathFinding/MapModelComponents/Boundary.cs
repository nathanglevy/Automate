using System;

namespace Assets.src.PathFinding.MapModelComponents
{
    [Serializable]
    public class Boundary
    {
        public readonly Coordinate topLeft;
        public readonly Coordinate bottomRight;

        public Boundary(Coordinate topLeft, Coordinate bottomRight) {
            if (!(bottomRight >= topLeft))
                throw new ArgumentException();
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
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

        public bool IsCoordinateInBoundary(Coordinate coordinate) {
            return (coordinate >= topLeft) && (coordinate <= bottomRight);
        }


    }
}