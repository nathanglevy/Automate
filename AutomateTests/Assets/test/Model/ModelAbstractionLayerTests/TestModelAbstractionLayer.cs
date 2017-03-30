using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.Model;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;
using src.Model.PathFinding;

namespace AutomateTests.Model.ModelAbstractionLayerTests
{
    [TestClass()]
    public class TestModelAbstractionLayer {
        GameWorldItem _gameWorldItem;
        Guid _baseWorldId;

        [TestInitialize]
        public void TestInitialize()
        {
            _gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
        }

        [TestMethod()]
        public void TestGetGameWorldItemById_ExpectSuccess()
        {
            GameUniverse.GetGameWorldItemById(_gameWorldItem.Guid);
        }

        [TestMethod()]
        public void TestGetGameWorldItemsInUniverse_ExpectSuccess() {
            GameUniverse.GetGameWorldItemsInUniverse();
        }

        [TestMethod()]
        public void TestCreateMovable_ExpectSuccess()
        {
            _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
        }

        [TestMethod()]
        public void TestCreateStructure_ExepectSuccess()
        {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 2, 1), StructureType.Basic);
        }

        [TestMethod()]
        public void TestGetStructureBoundary_ExpectValidValue() {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 2, 1), StructureType.Basic);
            Assert.AreEqual(structure.StructureBoundary, new Boundary(new Coordinate(0,0,0), new Coordinate(1,2,1) ));
        }

        [TestMethod()]
        public void TestGetMovableListInBoundary() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _gameWorldItem.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            List<MovableItem> movableList = _gameWorldItem.GetMovableListInBoundary(_gameWorldItem.GetWorldBoundary());
            Assert.IsTrue(movableList.SequenceEqual(new List<MovableItem>() { movable, movable2 }));
        }

        [TestMethod()]
        public void TestGetMovableListInCoordinate() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _gameWorldItem.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            List<MovableItem> movableList = _gameWorldItem.GetMovableListInCoordinate(new Coordinate(1, 1, 0));
            Assert.IsTrue(movableList.SequenceEqual(new List<MovableItem>() { movable2 }));
        }

        [TestMethod()]
        public void TestGetStructureItem_ExpectValidResult() {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_gameWorldItem.GetStructureItem(structure.Guid),structure);
        }

        [TestMethod()]
        public void TestIsStructureAtCoordinate_ExpectTrue()
        {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.IsTrue(_gameWorldItem.IsStructureAtCoordinate(new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void TestIsStructureAtCoordinate_ExpectFalse() {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.IsFalse(_gameWorldItem.IsStructureAtCoordinate(new Coordinate(3, 2, 1)));
        }

        [TestMethod()]
        public void TestGetStructureAtCoordinate_ExpectCorrectValue() {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_gameWorldItem.GetStructureAtCoordinate(new Coordinate(2, 2, 1)),structure);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetStructureAtCoordinate_NoStructureAtLocation_ExpectArgumentException() {
            StructureItem structure = _gameWorldItem.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_gameWorldItem.GetStructureAtCoordinate(new Coordinate(2, 3, 1)), structure);
        }

        [TestMethod()]
        public void TestIsMovableInMotion_ExpectSettingPathToChangeValue() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsFalse(movable.IsInMotion());
            movable.IssueMoveCommand(new Coordinate(2, 2, 0));
            Assert.IsTrue(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestGetAndSetMovableSpeed() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.Speed = 10;
            Assert.AreEqual(movable.Speed, 10);
        }

        [TestMethod()]
        public void TestSetMovableSpeed() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.Speed = 10;
            Assert.AreEqual(movable.Speed, 10);
            movable.Speed = 14;
            Assert.AreEqual(movable.Speed, 14);
        }

        [TestMethod()]
        public void TestGetSelectedIdListAndSelectItemsById() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.SelectItemsById(new List<Guid>() {movable.Guid});
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() {movable.Guid}));
        }

        [TestMethod()]
        public void TestSelectMovableItems() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.SelectMovableItems(new List<MovableItem>() { movable });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));

        }

        [TestMethod()]
        public void TestAddToSelectedItemsById() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable2.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid, movable2.Guid }));
        }

        [TestMethod()]
        public void TestAddToSelectedItems_SameId() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
        }

        [TestMethod()]
        public void TestAddToSelectedMovableItems() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedMovableItems(new List<MovableItem>() { movable });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.AddToSelectedMovableItems(new List<MovableItem>() { movable2 });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid, movable2.Guid }));
        }

        [TestMethod()]
        public void TestClearSelectedItems() {
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.AddToSelectedMovableItems(new List<MovableItem>() { movable });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _gameWorldItem.ClearSelectedItems();
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>()));
            _gameWorldItem.AddToSelectedMovableItems(new List<MovableItem>() { movable2 });
            Assert.IsTrue(_gameWorldItem.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable2.Guid }));
        }

        [TestMethod()]
        public void TestGetWorldBoundary() {
            Assert.AreEqual(_gameWorldItem.GetWorldBoundary(),new Boundary(new Coordinate(0,0,0), new Coordinate(9, 9, 1) ));
        }

        [TestMethod()]
        public void TestIsThereAnItemToBePlaced() {
            _gameWorldItem.ClearItemsToBePlaced();
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(_gameWorldItem.IsThereAnItemToBePlaced());
        }

        [TestMethod()]
        public void TestGetItemsToBePlaced() {
            _gameWorldItem.ClearItemsToBePlaced();
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(_gameWorldItem.GetItemsToBePlaced().SequenceEqual(new List<Item>() {movable, movable2}));
        }

        [TestMethod()]
        public void TestClearItemsToBePlaced() {
            _gameWorldItem.ClearItemsToBePlaced();
            MovableItem movable = _gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _gameWorldItem.ClearItemsToBePlaced();
            Assert.IsTrue(_gameWorldItem.GetItemsToBePlaced().SequenceEqual(new List<Item>()));
            Assert.IsFalse(_gameWorldItem.IsThereAnItemToBePlaced());
        }
    }
}