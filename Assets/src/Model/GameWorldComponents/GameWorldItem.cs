using System;
using System.Collections.Generic;
using src.Model.MapModelComponents;

namespace src.Model.GameWorldComponents
{
    //BIG TODO -- provide proper documentation for this addition!
    public class GameWorldItem : Item
    {
        //private Dictionary<Guid, GameWorld> _gameWorldDictionary = new Dictionary<Guid, GameWorld>();
        private GameWorld _focusedGameWorld;
        //TODO: Add game universe here -- selecting will put focus on a different world
//        public Guid Guid { get; private set; }

        public GameWorldItem(GameWorld gameWorld) {
            _focusedGameWorld = gameWorld;
            Guid = gameWorld.Guid;
        }

        public MovableItem CreateMovable(Coordinate spawnCoordinate, MovableType movableType)
        {
            return _focusedGameWorld.CreateMovable(spawnCoordinate, movableType);
        }

        public StructureItem CreateStructure(Coordinate spawnTopLeftCoordinate, Coordinate dimensions, StructureType structureType)
        {
            return _focusedGameWorld.CreateStructure(spawnTopLeftCoordinate, dimensions, structureType);
        }

        public List<MovableItem> GetMovableListInBoundary(Boundary selectionArea)
        {
            return _focusedGameWorld.GetMovableListInBoundary(selectionArea);
        }

        public List<MovableItem> GetMovableListInCoordinate(Coordinate selectionCoordinate)
        {
            return _focusedGameWorld.GetMovableListInBoundary(new Boundary(selectionCoordinate, selectionCoordinate));
        }

        public List<StructureItem> GetStructureListInBoundary(Boundary selectionArea)
        {
            throw new NotImplementedException();
        }

        public bool IsStructureAtCoordinate(Coordinate coordinate)
        {
            return _focusedGameWorld.IsStructureAtCoordinate(coordinate);
        }

        public StructureItem GetStructureItem(Guid structureGuid)
        {
            return _focusedGameWorld.GetStructureItem(structureGuid);
        }

        public StructureItem GetStructureAtCoordinate(Coordinate selectionCoordinate)
        {
            return _focusedGameWorld.GetStructureItemAtCoordinate(selectionCoordinate);
        }

        public List<Guid> GetSelectedIdList()
        {
            return _focusedGameWorld.GetSelectedIdList();
        }

        public List<MovableItem> GetSelectedMovableItemList()
        {
            return _focusedGameWorld.GetSelectedMovableItemList();
        }

        public void SelectItemsById(List<Guid> itemListToSelect)
        {
            _focusedGameWorld.SelectItemsById(itemListToSelect);
        }

        public void SelectMovableItems(List<MovableItem> itemListToSelect) {
            _focusedGameWorld.SelectMovableItems(itemListToSelect);
        }

        public void AddToSelectedItemsById(List<Guid> itemListToSelect)
        {
            _focusedGameWorld.AddToSelectedItemsById(itemListToSelect);
        }

        public void AddToSelectedMovableItems(List<MovableItem> itemListToSelect) {
            _focusedGameWorld.AddToSelectedMovableItems(itemListToSelect);
        }

        public void ClearSelectedItems()
        {
            _focusedGameWorld.ClearSelectedItems();
        }

        public Boundary GetWorldBoundary()
        {
            return _focusedGameWorld.GetWorldBoundary();
        }

        public bool IsThereAnItemToBePlaced()
        {
            return _focusedGameWorld.IsThereAnItemToBePlaced();
        }

        public List<Item> GetItemsToBePlaced()
        {
            return _focusedGameWorld.GetItemsToBePlaced();
        }

        public void ClearItemsToBePlaced()
        {
            _focusedGameWorld.ClearItemsToBePlaced();
        }

        public MovableItem GetMovableItem(Guid movableGuid)
        {
            return _focusedGameWorld.GetMovableItem(movableGuid);
        }


    }
}