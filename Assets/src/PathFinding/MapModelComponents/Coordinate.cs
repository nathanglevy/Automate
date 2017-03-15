using System;

namespace Assets.src.PathFinding.MapModelComponents
{
    [Serializable]
    public class Coordinate : Object
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;
        public Coordinate(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Coordinate operator +(Coordinate c1, Coordinate c2) {
            return new Coordinate(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }

        public static Coordinate operator -(Coordinate c1, Coordinate c2) {
            return new Coordinate(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
        }

        public static bool operator >(Coordinate c1, Coordinate c2) {
            return (c1.x > c2.x && c1.y > c2.y && c1.z > c2.z);
        }

        public static bool operator >=(Coordinate c1, Coordinate c2)
        {
            return ((c1.x > c2.x || (c1.x == c2.x)) &&
                    (c1.y > c2.y || (c1.y == c2.y)) &&
                    (c1.z > c2.z || (c1.z == c2.z)));
        }

        public static bool operator <(Coordinate c1, Coordinate c2) {
            return (c1.x < c2.x && c1.y < c2.y && c1.z < c2.z);
        }

        public static bool operator <=(Coordinate c1, Coordinate c2) {
            return ((c1.x < c2.x || (c1.x == c2.x)) &&
                    (c1.y < c2.y || (c1.y == c2.y)) &&
                    (c1.z < c2.z || (c1.z == c2.z)));
        }


        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            if ((object) c1 == null)
                return false;
            return c1.Equals(c2);
        }

        public static bool operator !=(Coordinate c1, Coordinate c2) {
            if ((object)c1 == null)
                return true;
            return !c1.Equals(c2);
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Coordinate coordinate = (Coordinate)obj;
            return (x == coordinate.x) && (y == coordinate.y) && (z == coordinate.z);
        }

        public override int GetHashCode() {
            return x + y * 256 + z * 1024;
        }

        public Coordinate TranslateCoordinate(Coordinate translation)
        {
            return this + translation;
        }

        public override String ToString() {
            return x + "_" + y + "_" + z;
        }
    }
}