using System;
using System.Collections.Generic;
using Automate.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Model.MapModelComponents {
    [TestClass()]
    public class TestBoundary
    {

        [TestMethod()]
        public void TestBoundaryConstructor_ExpectNotNull()
        {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.IsNotNull(boundary);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBoundaryConstructor_ExpectArgumentException()
        {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(-1, -1, 1));
        }

        [TestMethod()]
        public void TestBoundaryBorders_ExpectBoundaryValues()
        {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.AreEqual(boundary.topLeft, new Coordinate(0, 0, 0));
            Assert.AreEqual(boundary.bottomRight, new Coordinate(1, 1, 1));
        }

        [TestMethod()]
        public void TestBoundaryEquality_ExpectAreEqual()
        {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1));
            Assert.AreEqual(boundary, new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void TestBoundaryEquality_ExpectAreNotEqual()
        {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(2, 2, 2));
            Assert.AreNotEqual(boundary, new Boundary(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void TestIsCoordinateInBoundary_ExpectCorrectValues()
        {
            Boundary boundary = new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(0, 0, 0)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(5, 5, 1)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(10, 10, 2)));
            Assert.AreEqual(true, boundary.IsCoordinateInBoundary(new Coordinate(10, 10, 1)));
            Assert.AreEqual(false, boundary.IsCoordinateInBoundary(new Coordinate(11, 11, 3)));
        }

        [TestMethod()]
        public void TestIsBoundaryDisjoint_ExpectFalse()
        {
            Boundary boundary1 = new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2));
            Boundary boundary2 = new Boundary(new Coordinate(5, 5, 1), new Coordinate(15, 15, 3));
            Assert.IsFalse(boundary1.IsBoundaryDisjointToBoundary(boundary2));
        }

        [TestMethod()]
        public void TestIsBoundaryDisjoint_Inclusive_ExpectFalse() {
            Boundary boundary1 = new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2));
            Boundary boundary2 = new Boundary(new Coordinate(5, 5, 1), new Coordinate(8, 8, 1));
            Assert.IsFalse(boundary1.IsBoundaryDisjointToBoundary(boundary2));
        }

        [TestMethod()]
        public void TestIsBoundaryDisjoint_BottomLeft_ExpectFalse() {
            Boundary boundary1 = new Boundary(new Coordinate(2, 2, 0), new Coordinate(10, 10, 2));
            Boundary boundary2 = new Boundary(new Coordinate(0, 4, 1), new Coordinate(8, 12, 3));
            Assert.IsFalse(boundary1.IsBoundaryDisjointToBoundary(boundary2));
        }

        [TestMethod()]
        public void TestIsBoundaryDisjoint_ExpectTrue() {
            Boundary boundary1 = new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2));
            Boundary boundary2 = new Boundary(new Coordinate(11, 11, 3), new Coordinate(13, 13, 4));
            Assert.IsTrue(boundary1.IsBoundaryDisjointToBoundary(boundary2));
        }

        [TestMethod()]
        public void TestGetCoordinatesInBoundary() {
            Boundary boundary1 = new Boundary(new Coordinate(0, 0, 0), new Coordinate(2, 1, 0));
            HashSet<Coordinate> testHashSet = new HashSet<Coordinate>();
            testHashSet.Add(new Coordinate(0, 0, 0));
            testHashSet.Add(new Coordinate(1, 0, 0));
            testHashSet.Add(new Coordinate(2, 0, 0));
            testHashSet.Add(new Coordinate(0, 1, 0));
            testHashSet.Add(new Coordinate(1, 1, 0));
            testHashSet.Add(new Coordinate(2, 1, 0));
            Assert.IsTrue(testHashSet.IsSubsetOf(boundary1.GetListOfCoordinatesInBoundary()));
            Assert.IsTrue(boundary1.GetListOfCoordinatesInBoundary().IsSubsetOf(testHashSet));
        }
    }
}