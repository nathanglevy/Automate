using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GameWorldComponents;
using Model.MapModelComponents;
using Model.PathFinding;

namespace AutomateTests.Model.GameWorldComponents {
    [TestClass()]
    public class TestMovableItem
    {
        private GameWorldItem _gameWorldItem;
        [TestInitialize]
        public void TestInitialize() {
            _gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
        }

        [TestMethod()]
        public void TestMovableNew_ExpectNotNull() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0,0,0), MovableType.NormalHuman);
            Assert.IsNotNull(movable);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMovableNew_WithNull_ExpectArgumentNullException() {
            MovableItem movable = _gameWorldItem.CreateMovable(null, MovableType.NormalHuman);
        }

        [TestMethod()]
        public void TestIsInMotion_ExpectNotInMotion() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsFalse(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestIsInMotion_ExpectInMotionToggle() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsFalse(movable.IsInMotion());
            movable.IssueMoveCommand(new Coordinate(2,2,0));
            Assert.IsTrue(movable.IsInMotion());
            movable.MoveToNext();
            movable.MoveToNext();
            Assert.IsFalse(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestGetNextMovement() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.IssueMoveCommand(new Coordinate(3, 3, 0));
            Assert.AreEqual(movable.NextMovement, new Movement(1, 1, 0, 1));
            movable.MoveToNext();
            Assert.AreEqual(movable.NextMovement, new Movement(1, 1, 0, 1));
            movable.MoveToNext();
            Assert.AreEqual(movable.NextMovement, new Movement(1, 1, 0, 1));
            movable.MoveToNext();
            //check when there are no more moves
            Assert.AreEqual(movable.NextMovement, new Movement(0, 0, 0, 0));

        }

        [TestMethod()]
        public void TestGetNextCoordinate_ExpectCorrectValues() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.IssueMoveCommand(new Coordinate(3,3,0));
            Assert.AreEqual(movable.NextCoordinate, new Coordinate(1, 1, 0));
            movable.MoveToNext();
            Assert.AreEqual(movable.NextCoordinate, new Coordinate(2, 2, 0));
            movable.MoveToNext();
            Assert.AreEqual(movable.NextCoordinate, new Coordinate(3, 3, 0));
            movable.MoveToNext();
            //check when there are no more moves
            Assert.AreEqual(movable.NextCoordinate, new Coordinate(3, 3, 0));
        }

        [TestMethod()]
        public void TestMoveToNext_NoPathSet_ExpectSuccess() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.MoveToNext();
        }

        [TestMethod()]
        public void TestIssuePath_NoExistingPath_ExpectSuccessAndFalseReturn() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            bool success = movable.IssueMoveCommand(new Coordinate(0, 0, 1));
            movable.MoveToNext();
            Assert.IsFalse(success);
        }

        //[TestMethod()]
        //TODO: think of implementing this?

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