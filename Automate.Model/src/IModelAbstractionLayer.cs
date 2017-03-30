using System;
using System.Collections.Generic;
using Automate.Model.src.GameWorldComponents;
using Automate.Model.src.MapModelComponents;

namespace Automate.Model.src
{
    public interface IModelAbstractionLayer
    {
        void FocusWorld(Guid worldId);
        Guid CreateGameWorld(Coordinate mapDimensions);
        MovableItem CreateMovable(Coordinate spawnCoordinate, MovableType movableType);
        StructureItem CreateStructure(Coordinate spawnTopLeftCoordinate, Coordinate dimensions, StructureType structureType);

        MovableItem GetMovableItem(Guid movableGuid);
        //StructureItem GetStructureItem(Guid structureGuid);
        //TileItem GetTileItem(Guid itemGuid);

        //this is for the world
        Boundary GetWorldBoundary();

        //Get ids of structures and movables
        List<MovableItem> GetMovableListInBoundary(Boundary selectionArea);
        List<MovableItem> GetMovableListInCoordinate(Coordinate selectionCoordinate);
        List<StructureItem> GetStructureListInBoundary(Boundary selectionArea);
        StructureItem GetStructureAtCoordinate(Coordinate selectionCoordinate);
        bool IsStructureAtCoordinate(Coordinate coordinate);

        //selection related
        List<Guid> GetSelectedIdList();
        List<MovableItem> GetSelectedMovableItemList();
        void SelectItemsById(List<Guid> itemListToSelect);
        void AddToSelectedItemsById(List<Guid> itemListToSelect);
        void ClearSelectedItems();

        //items to be placed related
        bool IsThereAnItemToBePlaced();
        List<Item> GetItemsToBePlaced();
        void ClearItemsToBePlaced();
        Guid GetCurrentFocusGuid();
        void AddToSelectedMovableItems(List<MovableItem> itemListToSelect);
        void SelectMovableItems(List<MovableItem> itemListToSelect);
        StructureItem GetStructureItem(Guid structureGuid);
    }
}