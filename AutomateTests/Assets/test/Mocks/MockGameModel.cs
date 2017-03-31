using System;
using System.Collections.Generic;
using Automate.Model.src;
using Automate.Model.src.GameWorldComponents;
using Automate.Model.src.MapModelComponents;

namespace AutomateTests.test.Mocks
{
    public class MockGameModel : IModelAbstractionLayer
    {
        private GameWorldItem _testingGameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 20));
        private List<MovableItem> _selected = new List<MovableItem>();

        public void FocusWorld(Guid worldId)
        {
            throw new NotImplementedException();
        }

        public Guid CreateGameWorld(Coordinate mapDimensions)
        {
            throw new NotImplementedException();
        }

        public MovableItem CreateMovable(Coordinate spawnCoordinate, MovableType movableType)
        {
            throw new NotImplementedException();
        }

        public StructureItem CreateStructure(Coordinate spawnTopLeftCoordinate, StructureType structureType)
        {
            throw new NotImplementedException();
        }

        public MovableItem GetMovableItem(Guid movableGuid)
        {
            throw new NotImplementedException();
        }

        public Boundary GetWorldBoundary()
        {
            throw new NotImplementedException();
        }

        public List<MovableItem> GetMovableListInBoundary(Boundary selectionArea)
        {
            return new List<MovableItem>()
            {
               _testingGameWorld.CreateMovable(new Coordinate(1,1,1),MovableType.NormalHuman),
               _testingGameWorld.CreateMovable(new Coordinate(7,7,7),MovableType.NormalHuman),
            };
        }

        public List<MovableItem> GetMovableListInCoordinate(Coordinate selectionCoordinate)
        {
            throw new NotImplementedException();
        }

        public List<StructureItem> GetStructureListInBoundary(Boundary selectionArea)
        {
            throw new NotImplementedException();
        }

        public StructureItem GetStructureAtCoordinate(Coordinate selectionCoordinate)
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetSelectedIdList()
        {
            throw new NotImplementedException();
        }

        public void SelectItemsById(List<Guid> itemListToSelect)
        {
            throw new NotImplementedException();
        }

        public void AddToSelectedItemsById(List<Guid> itemListToSelect)
        {
            throw new NotImplementedException();
        }

        public void ClearSelectedItems()
        {
            throw new NotImplementedException();
        }

        public bool IsThereAnItemToBePlaced()
        {
            throw new NotImplementedException();
        }

        public List<Item> GetItemsToBePlaced()
        {
            throw new NotImplementedException();
        }

        public void ClearItemsToBePlaced()
        {
            throw new NotImplementedException();
        }

        public Guid GetCurrentFocusGuid()
        {
            throw new NotImplementedException();
        }

        public void AddToSelectedMovableItems(List<MovableItem> itemListToSelect)
        {
            _selected.AddRange(itemListToSelect);
        }

        public void SelectMovableItems(List<MovableItem> itemListToSelect)
        {
            throw new NotImplementedException();
        }

        public List<MovableItem> GetSelectedMovableItemList()
        {
            return new List<MovableItem>()
            {
               _testingGameWorld.CreateMovable(new Coordinate(1,1,1),MovableType.NormalHuman),
               _testingGameWorld.CreateMovable(new Coordinate(7,7,7),MovableType.NormalHuman),
            };
        }

        public StructureItem CreateStructure(Coordinate spawnTopLeftCoordinate, Coordinate dimensions, StructureType structureType)
        {
            throw new NotImplementedException();
        }

        public bool IsStructureAtCoordinate(Coordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public StructureItem GetStructureItem(Guid structureGuid)
        {
            throw new NotImplementedException();
        }

        public List<string> GetSelectedMovables()
        {
            return new List<string>() {"10","17"};
        }
    }
}