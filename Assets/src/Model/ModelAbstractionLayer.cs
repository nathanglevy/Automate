using System;
using System.Collections.Generic;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;
using src.Model.PathFinding;

namespace src.Model
{
    //BIG TODO -- provide proper documentation for this addition!
    public class ModelAbstractionLayer : IModelAbstractionLayer
    {
        private Dictionary<Guid, GameWorld> _gameWorldDictionary = new Dictionary<Guid, GameWorld>();
        private GameWorld _focusedGameWorld;
        //TODO: Add game universe here -- selecting will put focus on a different world

        public void FocusWorld(Guid worldId)
        {
            if (!_gameWorldDictionary.ContainsKey(worldId))
                throw new ArgumentException("Following worldId does not exist in MAL: " + worldId);
            _focusedGameWorld = _gameWorldDictionary[worldId];
        }

        public Guid GetCurrentFocusGuid()
        {
            if (_focusedGameWorld == null)
                throw new NullReferenceException("Model does not have a focused world");
            return _focusedGameWorld.Guid;
        }

        public Guid CreateGameWorld(Coordinate mapDimensions)
        {
            GameWorld newGameWorld = new GameWorld(mapDimensions);
            _gameWorldDictionary.Add(newGameWorld.Guid, newGameWorld);
            return newGameWorld.Guid;
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