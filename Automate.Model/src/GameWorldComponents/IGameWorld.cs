using System;
using System.Collections.Generic;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.PathFinding;
using Automate.Model.Requirements;
using Automate.Model.Tasks;
using JetBrains.Annotations;

namespace Automate.Model.GameWorldComponents
{
    public interface  IGameWorld {
        TaskDelegator TaskDelegator { get; }
        Guid Guid { get; }
        IRequirementAgent RequirementAgent { get; }
        List<ICell> GetCells { get; }

        /// <summary>Create a new movable character on the map</summary>
        /// <param name="spawnCoordinate">Starting position of spawned character</param>
        /// <param name="movableType">What type of movable character to create</param>
        /// <returns>A movableItem interface encapsulating the new movable</returns>
        IMovable CreateMovable(Coordinate spawnCoordinate, MovableType movableType);

        /// <summary>Create a new structure on the map</summary>
        /// <param name="spawnTopLeftCoordinate">Corner coordinate of structure</param>
        /// <param name="dimensions">Dimensions of the structure - eg. 1,1,1 is one block size</param>
        /// <param name="structureType">Type of the given structure</param>
        /// <returns>A StructureItem interface encapsulating the new structure</returns>
        IStructure CreateStructure(Coordinate spawnTopLeftCoordinate, Coordinate dimensions, StructureType structureType);

        /// <summary>Get List of all movables in a given boundary</summary>
        /// <param name="selectionArea">Boundary in which to search for movables</param>
        /// <returns>List of MovableItem interfaces giving access to the movables in the boundary</returns>
        List<IMovable> GetMovableListInBoundary(Boundary selectionArea);

        /// <summary>Get List of all movables in a given coordinate</summary>
        /// <param name="selectionCoordinate">Coordinate in which to search for movables</param>
        /// <returns>List of MovableItem interfaces giving access to the movables in the coordinate</returns>
        List<IMovable> GetMovableListInCoordinate(Coordinate selectionCoordinate);

        /// <summary>
        /// Checks if there is a structure placed on the map at the given coordinate
        /// </summary>
        /// <param name="coordinate">Given coordinate to test for structures</param>
        /// <returns>True if any structure is located at the coordinate, false if there is none</returns>
        bool IsStructureAtCoordinate(Coordinate coordinate);

        /// <summary>
        /// Returns a StructureItem interface to a structure at a given coordinate
        /// </summary>
        /// <param name="selectionCoordinate">Coordinate in which to find the structure</param>
        /// <returns>StructureItem interface to structure at given coordinate</returns>
        /// <exception cref="ArgumentException">Exception thrown if no structure exists given
        /// location -- first test for IsStructureAtCoordinate before calling this method</exception>
        IStructure GetStructureAtCoordinate(Coordinate selectionCoordinate);

        /// <summary>
        /// Returns a list of the Guids of all items that are selected by the game model on this map
        /// </summary>
        /// <returns>A list of the Guids of all items that are selected</returns>
        List<Guid> GetSelectedIdList();

        /// <summary>
        /// Selects items by a list of given Guid to items on the map
        /// This type of selection DELETES previous selections, in order to
        /// add to a selection, use AddToSelected* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of guids of items to be selected</param>
        void SelectItemsById(List<Guid> itemListToSelect);

        /// <summary>
        /// Selects movables by a list of given MovableItems on the map
        /// This type of selection DELETES previous selections, in order to
        /// add to a selection, use AddToSelected* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of MovableItems to be selected</param>
        void SelectMovableItems(List<IMovable> itemListToSelect);

        /// <summary>
        /// Adds items to selection by a list of given Guid to items on the map
        /// This type of selection ADDS to previous selections, in order to
        /// do a new selection, use regular Select* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of guids of items to be selected</param>
        void AddToSelectedItemsById(List<Guid> itemListToSelect);

        /// <summary>
        /// Selects movables by a list of given MovableItems on the map
        /// This type of selection ADDS to previous selections, in order to
        /// do a new selection, use regular Select* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of MovableItems to be selected</param>
        void AddToSelectedMovableItems(List<IMovable> itemListToSelect);

        /// <summary>
        /// Clears all selected items from all prior selections on this map
        /// </summary>
        void ClearSelectedItems();

        /// <summary>
        /// Returns a boundary describing the given gameworld's size and dimensions
        /// </summary>
        /// <returns>A boundary describing the given gameworld's size and dimensions</returns>
        Boundary GetWorldBoundary();

        /// <summary>
        /// Checks if there are items that need to be placed on the map -- this is important
        /// to check periodically because some actions may have byproducts that require adding
        /// one or more items to map.
        /// </summary>
        /// <returns>True if there are items waiting to be placed, false otherwise</returns>
        bool IsThereAnItemToBePlaced();

        /// <summary>
        /// Get a list of item interfaces waiting to be placed in the view or to be recognized.
        /// This list should be taken periodically and cleared afterwards using the ClearItemsToBePlaced
        /// method. Items returned should be checked for their type using the ItemType field, then
        /// cast their respective derived classes.
        /// </summary>
        /// <returns>List of Item interfaces of all objects that need to be placed in the view</returns>
        List<Item> GetItemsToBePlaced();

        /// <summary>
        /// Clears the list of items to be placed in the view world. It is important that all items taken
        /// prior to calling this method be passed on to view and taken care of, and only then call this method.
        /// </summary>
        void ClearItemsToBePlaced();

        /// <summary>
        /// Get a list of all MovableItems that are in motion (still have a path to take)
        /// </summary>
        /// <returns>A List of MovableItems that are in motion and have not completed their path</returns>
        List<IMovable> GetMovablesInMotion();

        ComponentStackGroup GetComponentStackGroupAtCoordinate(Coordinate location);
        bool HasMovableWithGuid(Guid movableGuid);
        MovementPath GetMovementPathWithLowestCostToCoordinate(Coordinate startCoordinate, Coordinate endCoordinate);

        MovementPath GetMovementPathWithLowestCostToCoordinate(List<Coordinate> startCoordinates,
            Coordinate endCoordinate);

        MovementPath GetMovementPathWithLowestCostToBoundary(List<Coordinate> startCoordinates,
            Boundary endBoundary, bool inclusive);

        List<IStructure> GetStructuresList();
        List<IMovable> GetMovableList();
        List<Guid> GetMovableIdList();
        bool IssueMoveCommand(Guid id, [NotNull] Coordinate targetCoordinate);
        bool CanStructureBePlaced(Coordinate coordinate, Coordinate dimensions);
        IStructure GetStructure(Guid structureGuid);
        IMovable GetMovable(Guid movableGuid);
        List<IMovable> GetSelectedMovableItemList();
        ICell GetCellAtCoordinate(Coordinate coordinate);
    }
}