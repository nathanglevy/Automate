using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
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
        public void TestGameWorldNew_SmallWorld_ExpectSuccess() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 1));
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
            gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
        }

        [TestMethod()]
        public void TestGetMovableIdList_ExpectIdToBeInList() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            List<Guid> moveableList = new List<Guid>();
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid);
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid);
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid);
            moveableList.Add(gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid);
            Assert.IsTrue(moveableList.All(gameWorld.GetMovableIdList().Contains));
        }

        [TestMethod()]
        public void TestIssueMoveCommand_ExpectSuccess() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.IssueMoveCommand(movableId, new Coordinate(0, 0, 0));
        }

        [TestMethod()]
        public void TestSelectItems_ExpectItemToBeSelected() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.SelectItemsById(new List<Guid>() {movableId});
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId }));
        }

        [TestMethod()]
        public void TestAddToSelectedItems_ExpectListToAppend() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.SelectItemsById(new List<Guid>() { movableId });
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId }));
            Guid movableId2 = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.AddToSelectedItemsById(new List<Guid>() { movableId2 });
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId, movableId2 }));
        }

        [TestMethod()]
        public void TestSelectItems_ExpectAutoClear() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.SelectItemsById(new List<Guid>() { movableId });
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId }));
            Guid movableId2 = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.SelectItemsById(new List<Guid>() { movableId2 });
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId2 }));
        }

        [TestMethod()]
        public void TestClearSelectedItems_ExpectClear() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.SelectItemsById(new List<Guid>() { movableId });
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId }));
            gameWorld.ClearSelectedItems();
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>()));
            Guid movableId2 = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            gameWorld.AddToSelectedItemsById(new List<Guid>() { movableId2 });
            Assert.IsTrue(gameWorld.GetSelectedIdList().SequenceEqual(new List<Guid>() { movableId2 }));
        }

        [TestMethod()]
        public void TestGetMovablesInBoundary() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            MovableItem movable = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(gameWorld.GetMovableListInBoundary(gameWorld.GetWorldBoundary()).SequenceEqual(new List<MovableItem>() { movable }));
        }

        [TestMethod()]
        public void TestIsThereAnItemToBePlaced_ExpectFalseThenTrue() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            gameWorld.ClearItemsToBePlaced();
            Assert.IsFalse(gameWorld.IsThereAnItemToBePlaced());
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            Assert.IsTrue(gameWorld.IsThereAnItemToBePlaced());
        }

        [TestMethod()]
        public void TestGetItemsToBePlaced_ExpectItemsToBePlaced() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            gameWorld.ClearItemsToBePlaced();
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            Assert.AreEqual(gameWorld.GetItemsToBePlaced().Count, 1);
            Assert.AreEqual(gameWorld.GetItemsToBePlaced()[0].Guid, movableId);
        }

        [TestMethod()]
        public void TestClearItemsToBePlaced_ExpectItemsToBeCleared() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            gameWorld.ClearItemsToBePlaced();
            Assert.IsFalse(gameWorld.IsThereAnItemToBePlaced());
            Guid movableId = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman).Guid;
            Assert.IsTrue(gameWorld.IsThereAnItemToBePlaced());
            gameWorld.ClearItemsToBePlaced();
            Assert.IsFalse(gameWorld.IsThereAnItemToBePlaced());
        }

        [TestMethod()]
        public void TestExpectCellsTobePlaced() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            Assert.AreEqual(gameWorld.GetItemsToBePlaced().Count, 27);
        }

        [TestMethod()]
        public void TestRecalculateMovablePaths_ExpectSucess() {
            GameWorld gameWorld = new GameWorld(new Coordinate(3, 3, 3));
            gameWorld.RecalculateMovablePaths();
        }

        [TestMethod()]
        public void IMPLEMENT_PATH_AND_RECALC_TESTS() {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void IMPLEMENT_COMPONENT_STACK_TESTS() {
            throw new NotImplementedException();
        }
        //TODO: Tests for structures!!!!
        //TODO: Need a testcase to check that recalculation is indeed happening properly
    }
}