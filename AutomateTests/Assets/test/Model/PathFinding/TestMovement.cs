using System;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Model.PathFinding
{
    [TestClass()]
    public class TestMovement
    {
        [TestMethod()]
        public void TestMovementNew_ExpectNotNull()
        {
            Assert.IsNotNull(new Movement(new Coordinate(0, 0, 1), 1));
            Assert.IsNotNull(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.IsNotNull(new Movement(new Coordinate(0, 0, -1), 1));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMovementNew_InvalidMoveDirection_ExpectArgumentException()
        {
            Assert.IsNotNull(new Movement(new Coordinate(0, 1, 2), 1));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMovementNew_InvalidMoveCost_ExpectArgumentException()
        {
            Assert.IsNotNull(new Movement(new Coordinate(0, 1, 1), -1));
        }

        [TestMethod()]
        public void TestGetMoveDirection_ExpectNotNull()
        {
            Movement movement = new Movement(new Coordinate(0, 0, 1), 1);
            Assert.IsNotNull(movement.GetMoveDirection());
        }

        [TestMethod()]
        public void TestGetMoveDirection_ExpectCorrectValue()
        {
            Movement movement = new Movement(new Coordinate(0, 0, 1), 1);
            Assert.AreEqual(new Coordinate(0, 0, 1), movement.GetMoveDirection());
        }

        [TestMethod()]
        public void TestGetMoveCost_ExpectNotNull()
        {
            Movement movement = new Movement(new Coordinate(0, 0, 1), 1);
            Assert.IsNotNull(movement.GetMoveCost());
        }

        [TestMethod()]
        public void TestGetMoveCost_ExpectCorrectValue() {
            Movement movement = new Movement(new Coordinate(0, 0, 1), 2);
            Assert.AreEqual(2, movement.GetMoveCost());
        }

        [TestMethod()]
        public void TestMovementEquality_ExpectEquals() {
            Movement movement1 = new Movement(new Coordinate(0, 0, 1), 2);
            Movement movement2 = new Movement(new Coordinate(0, 0, 1), 2);
            Assert.AreEqual(movement1, movement2);
        }

        [TestMethod()]
        public void TestMovementEquality_UnmatchingCoords_ExpectNotEquals() {
            Movement movement1 = new Movement(new Coordinate(0, 0, 1), 2);
            Movement movement2 = new Movement(new Coordinate(0, 1, 1), 2);
            Assert.AreNotEqual(movement1, movement2);
        }

        [TestMethod()]
        public void TestMovementEquality_UnmatchingCosts_ExpectNotEquals() {
            Movement movement1 = new Movement(new Coordinate(0, 0, 1), 2);
            Movement movement2 = new Movement(new Coordinate(0, 0, 1), 3);
            Assert.AreNotEqual(movement1, movement2);
        }
    }
}