using System;
using Assets.src.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Model.MapModelComponents {
    [TestClass()]
    public class TestBoundary {

        [TestMethod()]
        public void TestBoundaryConstructor_ExpectNotNull() {
            Boundary boundary = new Boundary(new Coordinate(0,0,0), new Coordinate(1,1,1));
            Assert.IsNotNull(boundary);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBoundaryConstructor_ExpectArgumentException() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(-1, -1, 1));
        }

        [TestMethod()]
        public void TestBoundaryBorders_ExpectBoundaryValues() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.AreEqual(boundary.topLeft, new Coordinate(0, 0, 0));
            Assert.AreEqual(boundary.bottomRight, new Coordinate(1, 1, 1));
        }

        [TestMethod()]
        public void TestBoundaryEquality_ExpectAreEqual() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.AreEqual(boundary, new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void TestBoundaryEquality_ExpectAreNotEqual() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(2, 2, 2));
            Assert.AreNotEqual(boundary, new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void TestIsCoordinateInBoundary_ExpectCorrectValues() {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(0, 0, 0)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(5, 5, 1)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(10, 10, 2)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(10, 10, 1)));
            Assert.AreEqual(false, boundary.IsCoordinateInBoundary(new Coordinate(11, 11, 3)));
        }

    }
}