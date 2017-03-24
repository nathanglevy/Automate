using System;
using System.Collections.Generic;
using System.Linq;
using Assets.src.Model.GameWorldComponents;
using Assets.src.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Model.GameWorldComponents {
    [TestClass()]
    public class TestGameWorld {

        [TestMethod()]
        public void TestGameWorldNew_AssertNotNull() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3,3,3));
            Assert.IsNotNull(gameWorld);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGameWorldNew_WithZeroDimension_ExpectArgumentException() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 0, 3));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGameWorldNew_WithNegativeDimension_ExpectArgumentException() {
            GameWorld gameWorld = new GameWorld(new Coordinate(-3, 3, 3));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGameWorldNew_WithNull_ExpectArgumentNullException() {
            GameWorld gameWorld = new GameWorld(null);
        }

        [TestMethod()]
        public void TestCreateMovable_ExpectSuccess() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            gameWorld.CreateMovable(new Coordinate(0, 0, 0));
        }

        [TestMethod()]
        public void TestGetMovableIdList_ExpectIdToBeInList() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            List<long> moveableList = new List<long>();
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0)));
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0)));
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0)));
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0)));
            Assert.IsTrue(moveableList.All(gameWorld.GetMovableIdList().Contains));
        }

        //[TestMethod()]
        public void TestIssueMoveCommand_ExpectSuccess() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            long movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0));
            gameWorld.IssueMoveCommand(movableId, new Coordinate(0, 0, 0));

        }

        //[TestMethod()]
        public void TestGetNextMovement() {
            throw new NotImplementedException();
        }

        //[TestMethod()]
        public void TestGetNextCoordinate() {
            throw new NotImplementedException();
        }

        //[TestMethod()]
        public void TestMoveToNext() {
            throw new NotImplementedException();
        }
    }
}