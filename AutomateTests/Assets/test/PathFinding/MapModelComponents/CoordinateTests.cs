using Assets.src.PathFinding.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.PathFinding.MapModelComponents
{
    [TestClass()]
    public class CoordinateTests {
        [TestMethod()]
        public void CoordinateTest() {
            Coordinate coordinate = new Coordinate(0,0,0);
            Assert.IsNotNull(coordinate);
        }

        [TestMethod()]
        public void CoordinateTest_CheckFields() {
            Coordinate coordinate1 = new Coordinate(1, 2, 3);
            Assert.AreEqual(coordinate1.x,1);
            Assert.AreEqual(coordinate1.y,2);
            Assert.AreEqual(coordinate1.z,3);
            Coordinate coordinate2 = new Coordinate(6, 5, 4);
            Assert.AreEqual(coordinate2.x, 6);
            Assert.AreEqual(coordinate2.y, 5);
            Assert.AreEqual(coordinate2.z, 4);
        }

        [TestMethod()]
        public void CoordinateTest_CheckOperators() {
            Coordinate coordinate1 = new Coordinate(1, 2, 3);
            Coordinate coordinate2 = new Coordinate(3, 4, 5);
            Coordinate coordinate3 = new Coordinate(0, 3, 4);
            Coordinate coordinate4 = new Coordinate(1, 2, 4);

            //check all math ops
            Assert.AreEqual(new Coordinate(4,6,8), coordinate1 + coordinate2);
            Assert.AreEqual(new Coordinate(2,2,2), coordinate2 - coordinate1);

            //check all equality combinations
            Assert.IsFalse(coordinate2 == coordinate1);
            Assert.IsTrue(coordinate1 != coordinate2);
            Assert.IsTrue(coordinate1 == new Coordinate(1, 2, 3));
            Assert.IsFalse(null == coordinate1);
            Assert.IsTrue(null != coordinate1);
            Assert.IsFalse(coordinate1 == null);
            Assert.IsTrue(coordinate1 != null);

            //check all relative operations
            //one is absolutely contained
            Assert.IsTrue(coordinate1 < coordinate2);
            Assert.IsTrue(coordinate1 <= coordinate2);
            Assert.IsTrue(coordinate2 > coordinate1);
            Assert.IsTrue(coordinate2 >= coordinate1);
            //one is absolutely not contained
            Assert.IsFalse(coordinate1 < coordinate3);
            Assert.IsFalse(coordinate1 <= coordinate3);
            Assert.IsFalse(coordinate3 > coordinate1);
            Assert.IsFalse(coordinate3 >= coordinate1);
            //one is inclusive (on the edge but not equal)
            Assert.IsFalse(coordinate1 < coordinate4);
            Assert.IsTrue(coordinate1 <= coordinate4);
            Assert.IsFalse(coordinate4 > coordinate1);
            Assert.IsTrue(coordinate4 >= coordinate1);
        }

        [TestMethod()]
        public void CoordinateTest_CheckTranslation() {
            Coordinate coordinate1 = new Coordinate(1, 2, 3);
            Coordinate coordinate2 = new Coordinate(3, 4, 5);
            Assert.AreEqual(new Coordinate(4, 6, 8), coordinate1.TranslateCoordinate(coordinate2));
        }
    }
}
