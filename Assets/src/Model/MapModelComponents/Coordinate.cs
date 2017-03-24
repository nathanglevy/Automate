using System;

namespace Assets.src.Model.MapModelComponents
{
    /// <summary>
    /// Class which holds information on 3D coordinates.<para />
    /// Contains: 3 values, one for each axis.<para />
    /// Relative operations:
    /// <list type="bullet">
    /// <item>
    /// <description>Equality is not by reference, it is by value</description>
    /// </item>
    /// <item>
    /// <description>Bigger-Than (>=) means that it is in bottom-right quadrant relative to it</description>
    /// </item>
    /// <item>
    /// <description>Smaller than (&lt;=) means that it is in top-left quadrant relative to it</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>This class is immutable!</remarks>
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

        public static Coordinate operator -(Coordinate c2) {
            return new Coordinate(- c2.x, - c2.y, - c2.z);
        }

        //Operator is defined to check if the c1 coordinate is in the bottom right quad
        //relative to c2.
        public static bool operator >(Coordinate c1, Coordinate c2) {

            if ((object)c1 == null || (object)c2 == null)
                return false;
            return (c1.x > c2.x && c1.y > c2.y && c1.z > c2.z);
        }

        public static bool operator >=(Coordinate c1, Coordinate c2)
        {
            if ((object)c1 == null || (object)c2 == null)
                return false;
            return ((c1.x > c2.x || (c1.x == c2.x)) &&
                    (c1.y > c2.y || (c1.y == c2.y)) &&
                    (c1.z > c2.z || (c1.z == c2.z)));
        }

        //Operator is defined to check if the c1 coordinate is in the left top quad
        //relative to c2.
        public static bool operator <(Coordinate c1, Coordinate c2) {
            if ((object)c1 == null || (object)c2 == null)
                return false;
            return (c1.x < c2.x && c1.y < c2.y && c1.z < c2.z);
        }

        public static bool operator <=(Coordinate c1, Coordinate c2) {
            if ((object)c1 == null || (object)c2 == null)
                return false;
            return ((c1.x < c2.x || (c1.x == c2.x)) &&
                    (c1.y < c2.y || (c1.y == c2.y)) &&
                    (c1.z < c2.z || (c1.z == c2.z)));
        }


        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            if ((object)c1 == null && (object)c2 == null)
                return true;
            if ((object)c1 == null || (object)c2 == null)
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