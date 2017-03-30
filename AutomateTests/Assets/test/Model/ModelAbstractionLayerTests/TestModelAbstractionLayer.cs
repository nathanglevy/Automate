using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.Model;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;

namespace AutomateTests.Model.ModelAbstractionLayerTests
{
    [TestClass()]
    public class TestModelAbstractionLayer {
        IModelAbstractionLayer _modelAbstractionLayer;
        Guid _baseWorldId;

        [TestInitialize]
        public void TestInitialize()
        {
            _modelAbstractionLayer = new ModelAbstractionLayer();
            _baseWorldId = _modelAbstractionLayer.CreateGameWorld(new Coordinate(10, 10, 2));
            _modelAbstractionLayer.FocusWorld(_baseWorldId);
        }

        [TestMethod()]
        public void TestCreateGameWorld_ExpectSuccess() {
            Guid gameWorld = _modelAbstractionLayer.CreateGameWorld(new Coordinate(10, 10, 2));
        }

        [TestMethod()]
        public void TestFocusWorld_ExpectCorrectValue()
        {
            Guid gameWorld = _modelAbstractionLayer.CreateGameWorld(new Coordinate(10, 10, 2));
            _modelAbstractionLayer.FocusWorld(gameWorld);
            Assert.AreEqual(gameWorld, _modelAbstractionLayer.GetCurrentFocusGuid());
        }

        [TestMethod()]
        public void TestCreateMovable_ExpectSuccess()
        {
            _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
        }

        [TestMethod()]
        public void TestCreateStructure_ExepectSuccess()
        {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 2, 1), StructureType.Basic);
        }

        [TestMethod()]
        public void TestGetStructureBoundary_ExpectValidValue() {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 2, 1), StructureType.Basic);
            Assert.AreEqual(structure.StructureBoundary, new Boundary(new Coordinate(0,0,0), new Coordinate(1,2,1) ));
        }

        [TestMethod()]
        public void TestGetMovableListInBoundary() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _modelAbstractionLayer.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            List<MovableItem> movableList = _modelAbstractionLayer.GetMovableListInBoundary(_modelAbstractionLayer.GetWorldBoundary());
            Assert.IsTrue(movableList.SequenceEqual(new List<MovableItem>() { movable, movable2 }));
        }

        [TestMethod()]
        public void TestGetMovableListInCoordinate() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _modelAbstractionLayer.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            List<MovableItem> movableList = _modelAbstractionLayer.GetMovableListInCoordinate(new Coordinate(1, 1, 0));
            Assert.IsTrue(movableList.SequenceEqual(new List<MovableItem>() { movable2 }));
        }

        [TestMethod()]
        public void TestGetStructureItem_ExpectValidResult() {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_modelAbstractionLayer.GetStructureItem(structure.Guid),structure);
        }

        [TestMethod()]
        public void TestIsStructureAtCoordinate_ExpectTrue()
        {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.IsTrue(_modelAbstractionLayer.IsStructureAtCoordinate(new Coordinate(1, 1, 1)));
        }

        [TestMethod()]
        public void TestIsStructureAtCoordinate_ExpectFalse() {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.IsFalse(_modelAbstractionLayer.IsStructureAtCoordinate(new Coordinate(3, 2, 1)));
        }

        [TestMethod()]
        public void TestGetStructureAtCoordinate_ExpectCorrectValue() {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_modelAbstractionLayer.GetStructureAtCoordinate(new Coordinate(2, 2, 1)),structure);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetStructureAtCoordinate_NoStructureAtLocation_ExpectArgumentException() {
            StructureItem structure = _modelAbstractionLayer.CreateStructure(new Coordinate(0, 0, 0),
                new Coordinate(2, 2, 1), StructureType.Basic);
            Assert.AreEqual(_modelAbstractionLayer.GetStructureAtCoordinate(new Coordinate(2, 3, 1)), structure);
        }

        [TestMethod()]
        public void TestIsMovableInMotion_ExpectSettingPathToChangeValue() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsFalse(movable.IsInMotion());
            movable.IssueMoveCommand(new Coordinate(2, 2, 0));
            Assert.IsTrue(movable.IsInMotion());
        }

        [TestMethod()]
        public void TestGetAndSetMovableSpeed() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.Speed = 10;
            Assert.AreEqual(movable.Speed, 10);
        }

        [TestMethod()]
        public void TestSetMovableSpeed() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            movable.Speed = 10;
            Assert.AreEqual(movable.Speed, 10);
            movable.Speed = 14;
            Assert.AreEqual(movable.Speed, 14);
        }

        [TestMethod()]
        public void TestGetSelectedIdListAndSelectItemsById() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.SelectItemsById(new List<Guid>() {movable.Guid});
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() {movable.Guid}));
        }

        [TestMethod()]
        public void TestSelectMovableItems() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.SelectMovableItems(new List<MovableItem>() { movable });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));

        }

        [TestMethod()]
        public void TestAddToSelectedItemsById() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _modelAbstractionLayer.AddToSelectedItemsById(new List<Guid>() { movable2.Guid });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid, movable2.Guid }));
        }

        [TestMethod()]
        public void TestAddToSelectedItems_SameId() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _modelAbstractionLayer.AddToSelectedItemsById(new List<Guid>() { movable.Guid });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
        }

        [TestMethod()]
        public void TestAddToSelectedMovableItems() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.AddToSelectedMovableItems(new List<MovableItem>() { movable });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _modelAbstractionLayer.AddToSelectedMovableItems(new List<MovableItem>() { movable2 });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid, movable2.Guid }));
        }

        [TestMethod()]
        public void TestClearSelectedItems() {
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.AddToSelectedMovableItems(new List<MovableItem>() { movable });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable.Guid }));
            _modelAbstractionLayer.ClearSelectedItems();
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>()));
            _modelAbstractionLayer.AddToSelectedMovableItems(new List<MovableItem>() { movable2 });
            Assert.IsTrue(_modelAbstractionLayer.GetSelectedIdList().SequenceEqual(new List<Guid>() { movable2.Guid }));
        }

        [TestMethod()]
        public void TestGetWorldBoundary() {
            Assert.AreEqual(_modelAbstractionLayer.GetWorldBoundary(),new Boundary(new Coordinate(0,0,0), new Coordinate(9, 9, 1) ));
        }

        [TestMethod()]
        public void TestIsThereAnItemToBePlaced() {
            _modelAbstractionLayer.ClearItemsToBePlaced();
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(_modelAbstractionLayer.IsThereAnItemToBePlaced());
        }

        [TestMethod()]
        public void TestGetItemsToBePlaced() {
            _modelAbstractionLayer.ClearItemsToBePlaced();
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            MovableItem movable2 = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            Assert.IsTrue(_modelAbstractionLayer.GetItemsToBePlaced().SequenceEqual(new List<Item>() {movable, movable2}));
        }

        [TestMethod()]
        public void TestClearItemsToBePlaced() {
            _modelAbstractionLayer.ClearItemsToBePlaced();
            MovableItem movable = _modelAbstractionLayer.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);
            _modelAbstractionLayer.ClearItemsToBePlaced();
            Assert.IsTrue(_modelAbstractionLayer.GetItemsToBePlaced().SequenceEqual(new List<Item>()));
            Assert.IsFalse(_modelAbstractionLayer.IsThereAnItemToBePlaced());
        }
    }
}