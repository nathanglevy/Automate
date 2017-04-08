using System;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Automate.Model.localTests {
    [TestClass()]
    public class TestMovable {
        [TestMethod()]
        public void TestMovableNew_ExpectNotNull() {
            Movable movable = new Movable(new Coordinate(0,0,0), MovableType.NormalHuman);
            Assert.IsNotNull(movable);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMovableNew_WithNull_ExpectArgumentNullException() {
            Movable movable = new Movable(null, MovableType.NormalHuman);
        }

        [TestMethod()]
        public void TestIsInMotion_ExpectNotInMotion() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsFalse(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestIsInMotion_ExpectInMotionToggle() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovementPath movementPath = new MovementPath(new Coordinate(0,0,0));
            Assert.IsFalse(movable.IsInMotion());
            movementPath.AddMovement(new Movement(1,1,0,1));
            movementPath.AddMovement(new Movement(1,1,0,1));
            movable.SetPath(movementPath);
            Assert.IsTrue(movable.IsInMotion());
            movable.MoveToNext();
            movable.MoveToNext();
            Assert.IsFalse(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestSetPath_EnsureMovementIsCloned() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(1, 1, 0, 1));
            movable.SetPath(movementPath);
            movementPath.AddMovement(new Movement(1, 1, 0, 1));
            movable.MoveToNext();
            //if it was not cloned, this would be true because it would be adding a movement
            //but since the addmovement is AFTER setpath, this should be false
            Assert.IsFalse(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestGetNextMovement() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(1, 1, 0, 1));
            movementPath.AddMovement(new Movement(0, 1, 0, 1));
            movementPath.AddMovement(new Movement(1, 0, 0, 1));
            movable.SetPath(movementPath);
            Assert.AreEqual(movable.GetNextMovement(), new Movement(1, 1, 0, 1));
            movable.MoveToNext();
            Assert.AreEqual(movable.GetNextMovement(), new Movement(0, 1, 0, 1));
            movable.MoveToNext();
            Assert.AreEqual(movable.GetNextMovement(), new Movement(1, 0, 0, 1));
            movable.MoveToNext();
            //check when there are no more moves
            Assert.AreEqual(movable.GetNextMovement(), new Movement(0, 0, 0, 0));

        }

        [TestMethod()]
        public void TestGetNextCoordinate() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(1, 1, 0, 1));
            movementPath.AddMovement(new Movement(0, 1, 0, 1));
            movementPath.AddMovement(new Movement(1, 0, 0, 1));
            movable.SetPath(movementPath);
            Assert.AreEqual(movable.GetNextCoordinate(), new Coordinate(1, 1, 0));
            movable.MoveToNext();
            Assert.AreEqual(movable.GetNextCoordinate(), new Coordinate(1, 2, 0));
            movable.MoveToNext();
            Assert.AreEqual(movable.GetNextCoordinate(), new Coordinate(2, 2, 0));
            movable.MoveToNext();
            //check when there are no more moves
            Assert.AreEqual(movable.GetNextCoordinate(), new Coordinate(2, 2, 0));
        }

        [TestMethod()]
        public void TestMoveToNext_NoPathSet_ExpectSuccess() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.MoveToNext();
        }

        [TestMethod()]
        public void TestGetEffectiveCoordinate() {
            Movable movable = new Movable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovementPath movementPath = new MovementPath(new Coordinate(0, 0, 0));
            movementPath.AddMovement(new Movement(1, 1, 0, 1));
            movementPath.AddMovement(new Movement(0, 1, 0, 1));
            movementPath.AddMovement(new Movement(1, 0, 0, 1));
            movable.SetPath(movementPath);
            Assert.AreEqual(movable.GetEffectiveCoordinate(), new Coordinate(0, 0, 0));
            Assert.AreEqual(movable.GetCurrentCoordinate(), new Coordinate(0, 0, 0));
            movable.StartTransitionToNext();
            Assert.AreEqual(movable.GetEffectiveCoordinate(), new Coordinate(1, 1, 0));
            Assert.AreEqual(movable.GetCurrentCoordinate(), new Coordinate(0, 0, 0));
            movable.MoveToNext();
            Assert.AreEqual(movable.GetEffectiveCoordinate(), new Coordinate(1, 1, 0));
            Assert.AreEqual(movable.GetCurrentCoordinate(), new Coordinate(1, 1, 0));

        }
    }
}