using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.StructureComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Model.GameWorldInterface {
    [TestClass()]
    public class TestGameWorldItem
    {
        private IGameWorld _gameWorldItem;

        [TestInitialize]
        public void TestInitialize() {
            _gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
        }

        [TestMethod()]
        public void TestGetGameWorldItemById_ExpectSuccess() {
            GameUniverse.GetGameWorldItemById(_gameWorldItem.Guid);
        }

        [TestMethod()]
        public void TestGetGameWorldItemsInUniverse_ExpectSuccess() {
            GameUniverse.GetGameWorldItemsInUniverse();
        }

        [TestMethod()]
        public void TestCreateMovable_ExpectSuccess() {
            _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
        }

        [TestMethod()]
        public void TestCreateStructure_ExepectSuccess() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 2, 1), StructureType.Basic);
        }

        [TestMethod()]
        public void TestGetStructureBoundary_ExpectValidValue() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 2, 1), StructureType.Basic);
            Assert.AreEqual(structure.Boundary, new Boundary(new Coordinate(0, 0, 0), new Coordinate(0, 1, 0)));
        }

        [TestMethod()]
        public void TestGetMovableListInBoundary() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            IMovable movable2 = _gameWorldItem.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            List<IMovable> movableList = _gameWorldItem.GetMovableListInBoundary(_gameWorldItem.GetWorldBoundary());
            Assert.IsTrue(movableList.SequenceEqual(new List<IMovable>() { movable, movable2 }));
        }

        [TestMethod()]
        public void TestGetMovableListInCoordinate() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            IMovable movable2 = _gameWorldItem.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            List<IMovable> movableList = _gameWorldItem.GetMovableListInCoordinate(new Coordinate(1, 1, 0));
            Assert.IsTrue(movableList.SequenceEqual(new List<IMovable>() { movable2 }));
        }

        [TestMethod()]
        public void TestGetIStructure_ExpectValidResult() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_gameWorldItem.GetStructure(structure.Guid), structure);
        }

        [TestMethod()]
        public void TestIsStructureAtCoordinate_ExpectTrue() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.IsTrue(_gameWorldItem.IsStructureAtCoordinate(new Coordinate(1, 1, 0)));
        }

        [TestMethod()]
        public void TestIsStructureAtCoordinate_ExpectFalse() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.IsFalse(_gameWorldItem.IsStructureAtCoordinate(new Coordinate(3, 2, 1)));
        }

        [TestMethod()]
        public void TestGetStructureAtCoordinate_ExpectCorrectValue() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_gameWorldItem.GetStructureAtCoordinate(new Coordinate(1, 1, 0)), structure);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetStructureAtCoordinate_NoStructureAtLocation_ExpectArgumentException() {
            IStructure structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_gameWorldItem.GetStructureAtCoordinate(new Coordinate(2, 3, 1)), structure);
        }

        [TestMethod()]
        public void TestIsMovableInMotion_ExpectSettingPathToChangeValue() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsFalse(movable.IsInMotion());
            movable.IssueMoveCommand(new Coordinate(2, 2, 0));
            Assert.IsTrue(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestGetAndSetMovableSpeed() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.Speed = 10;
            Assert.AreEqual(movable.Speed, 10);
        }

        [TestMethod()]
        public void TestSetMovableSpeed() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.Speed = 10;
            Assert.AreEqual(movable.Speed, 10);
            movable.Speed = 14;
            Assert.AreEqual(movable.Speed, 14);
        }

        [TestMethod()]
        public void TestGetSelectedIdListAndSelectItemsById() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.SelectItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
        }

        [TestMethod()]
        public void TestSelectMovableItems() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.SelectMovableItems(new List<IMovable>() { movable });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));

        }

        [TestMethod()]
        public void TestAddToSelectedItemsById() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            IMovable movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable2.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid, movable2.Guid }));
        }

        [TestMethod()]
        public void TestAddToSelectedItems_SameId() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
        }

        [TestMethod()]
        public void TestAddToSelectedMovableItems() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            IMovable movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedMovableItems(new List<IMovable>() { movable });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.AddToSelectedMovableItems(new List<IMovable>() { movable2 });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid, movable2.Guid }));
        }

        [TestMethod()]
        public void TestClearSelectedItems() {
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            IMovable movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedMovableItems(new List<IMovable>() { movable });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.ClearSelectedItems();
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>()));
            _gameWorldItem.AddToSelectedMovableItems(new List<IMovable>() { movable2 });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable2.Guid }));
        }

        [TestMethod()]
        public void TestGetWorldBoundary() {
            Assert.AreEqual(_gameWorldItem.GetWorldBoundary(), new Boundary(new Coordinate(0, 0, 0), new Coordinate(9, 9, 1)));
        }

        [TestMethod()]
        public void TestIsThereAnItemToBePlaced() {
            _gameWorldItem.ClearItemsToBePlaced();
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(_gameWorldItem.IsThereAnItemToBePlaced());
        }

        [TestMethod()]
        public void TestGetItemsToBePlaced() {
            _gameWorldItem.ClearItemsToBePlaced();
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            IMovable movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(_gameWorldItem.GetItemsToBePlaced().SequenceEqual(new List<Item>() { movable as Item, movable2 as Item }));
        }

        [TestMethod()]
        public void TestClearItemsToBePlaced() {
            _gameWorldItem.ClearItemsToBePlaced();
            IMovable movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.ClearItemsToBePlaced();
            Assert.IsTrue(_gameWorldItem.GetItemsToBePlaced().SequenceEqual(new List<Item>()));
            Assert.IsFalse(_gameWorldItem.IsThereAnItemToBePlaced());
        }
    }
}