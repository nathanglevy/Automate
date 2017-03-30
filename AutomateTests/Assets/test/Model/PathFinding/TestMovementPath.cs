using System;
using Automate.Model.src.MapModelComponents;
using Automate.Model.src.PathFinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Model.PathFinding
{
    [TestClass()]
    public class TestMovementPath
    {

        [TestMethod()]
        public void TestMovementPathNew_ExpectNotNull()
        {
            Assert.IsNotNull(new MovementPath(new Coordinate(0, 0, 0)));
        }

        [TestMethod()]
        public void TestAddMovement_ExpectSuccess()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
        }

        [TestMethod()]
        public void TestAddMovement_ExpectCorrectLastCoordinate()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.AreEqual(new Coordinate(0, 1, 3), movementPath.GetEndCoordinate());
        }

        [TestMethod()]
        public void TestGetStartCoordinate_ExpectStartToUpdateCorrectly()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.GetStartCoordinate();
            Assert.AreEqual(movementPath.GetStartCoordinate(), new Coordinate(0, 0, 0));
        }

        [TestMethod()]
        public void TestGetEndCoordinate_ExpectEqualStart()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            Assert.AreEqual(new Coordinate(0, 0, 0), movementPath.GetEndCoordinate());
        }

        [TestMethod()]
        public void TestGetNextMovement_ExpectCorrectValues()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.AreEqual(new Movement(new Coordinate(0, 0, 1), 1),
                movementPath.GetNextMovement(new Coordinate(0, 0, 0)));
            Assert.AreEqual(new Movement(new Coordinate(0, 0, 1), 1),
                movementPath.GetNextMovement(new Coordinate(0, 0, 1)));
            Assert.AreEqual(new Movement(new Coordinate(0, 1, 1), 1),
                movementPath.GetNextMovement(new Coordinate(0, 0, 2)));
        }

        [TestMethod()]
        public void TestGetNextCoordinate_ExpectCorrectValues()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.AreEqual(new Coordinate(0, 0, 1), movementPath.GetNextCoordinate(new Coordinate(0, 0, 0)));
            Assert.AreEqual(new Coordinate(0, 0, 2), movementPath.GetNextCoordinate(new Coordinate(0, 0, 1)));
            Assert.AreEqual(new Coordinate(0, 1, 3), movementPath.GetNextCoordinate(new Coordinate(0, 0, 2)));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetNextCoordinate_OutOfBounds_ExpectArgumentsOutOfRangeException()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.AreEqual(new Coordinate(0, 1, 3), movementPath.GetNextCoordinate(new Coordinate(1, 1, 2)));
        }

        [TestMethod()]
        public void TestEquality_ExpectEquals()
        {
            MovementPath movementPath1 = new MovementPath(new Coordinate(0, 0, 0));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            MovementPath movementPath2 = new MovementPath(new Coordinate(0, 0, 0));
            movementPath2.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath2.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath2.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.AreEqual(movementPath2, movementPath1);
        }

        [TestMethod()]
        public void TestEquality_ExpectNotEquals()
        {
            MovementPath movementPath1 = new MovementPath(new Coordinate(0, 0, 0));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            movementPath1.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            MovementPath movementPath2 = new MovementPath(new Coordinate(0, 0, 0));
            movementPath2.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath2.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath2.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            Assert.AreNotEqual(movementPath2, movementPath1);
        }

        [TestMethod()]
        public void TestClone_ExpectUniqueness()
        {
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 0, 1), 1));
            movementPath.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));
            MovementPath movementPathClone = new MovementPath(movementPath);

            //they are cloned, should be equal
            Assert.AreEqual(movementPath, movementPathClone);
            movementPathClone.AddMovement(new Movement(new Coordinate(0, 1, 1), 1));

            //now they should not be equal
            Assert.AreNotEqual(movementPath, movementPathClone);
        }
    }
}