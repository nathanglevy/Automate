using System;
using System.Collections.Generic;
using Assets.src.Controller.Interfaces;
using src.Model;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;
using src.Model.PathFinding;

namespace AutomateTests.Mocks
{
    public class MockGameModel : IModelAbstractionLayer
    {
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SelectMovableItems(List<MovableItem> itemListToSelect)
        {
            throw new NotImplementedException();
        }

        public List<MovableItem> GetSelectedMovableItemList()
        {
            throw new NotImplementedException();
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