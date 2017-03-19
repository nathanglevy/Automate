    using System;
using Assets.src.PathFinding.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.PathFinding.MapModelComponents {
    [TestClass()]
    public class BoundaryTests {

        [TestMethod()]
        public void BoundaryTest_New() {
            Boundary boundary = new Boundary(new Coordinate(0,0,0), new Coordinate(1,1,1));
            Assert.IsNotNull(boundary);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BoundaryTest_NewWithBadBoundry_ExpectArgumentException() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(-1, -1, 1));
        }

        [TestMethod()]
        public void BoundaryTest_checkValues() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.AreEqual(boundary.topLeft, new Coordinate(0, 0, 0));
            Assert.AreEqual(boundary.bottomRight, new Coordinate(1, 1, 1));
        }

        [TestMethod()]
        public void BoundaryTest_checkEquality() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.AreEqual(boundary, new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void BoundaryTest_checkWithinBoundary() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(0, 0, 0)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(5, 5, 1)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(10, 10, 2)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(10, 10, 1)));
            Assert.AreEqual(false, boundary.IsCoordinateInBoundary(new Coordinate(11, 11, 3)));
        }

    }
}