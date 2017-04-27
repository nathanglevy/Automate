using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;

namespace Automate.Model.GameWorldInterface
{
    /// <summary>
    /// Interface class allowing access to the GameWorld Model.
    /// All methods called via this interface point to a specific gameworld in the model.
    /// All accesses are done only via this interface as it protects the model exposing the rep
    /// and breaking the model invariants.
    /// </summary>
    public class GameWorldItem : Item
    {
        private readonly GameWorld _focusedGameWorld;
        public override Coordinate Coordinate => _focusedGameWorld.GetWorldBoundary().topLeft;
        public TaskDelegator TaskDelegator => _focusedGameWorld.TaskDelegator;
        public GameWorld GameWorld => _focusedGameWorld;
        internal GameWorldItem(GameWorld gameWorld) {
            _focusedGameWorld = gameWorld;
            Guid = gameWorld.Guid;
        }

        /// <summary>Create a new movable character on the map</summary>
        /// <param name="spawnCoordinate">Starting position of spawned character</param>
        /// <param name="movableType">What type of movable character to create</param>
        /// <returns>A movableItem interface encapsulating the new movable</returns>
        public MovableItem CreateMovable(Coordinate spawnCoordinate, MovableType movableType)
        {
            return _focusedGameWorld.CreateMovable(spawnCoordinate, movableType);
        }

        /// <summary>Create a new structure on the map</summary>
        /// <param name="spawnTopLeftCoordinate">Corner coordinate of structure</param>
        /// <param name="dimensions">Dimensions of the structure - eg. 1,1,1 is one block size</param>
        /// <param name="structureType">Type of the given structure</param>
        /// <returns>A StructureItem interface encapsulating the new structure</returns>
        public StructureItem CreateStructure(Coordinate spawnTopLeftCoordinate, Coordinate dimensions, StructureType structureType)
        {
            return _focusedGameWorld.CreateStructure(spawnTopLeftCoordinate, dimensions, structureType);
        }

        /// <summary>Get List of all movables in a given boundary</summary>
        /// <param name="selectionArea">Boundary in which to search for movables</param>
        /// <returns>List of MovableItem interfaces giving access to the movables in the boundary</returns>
        public List<MovableItem> GetMovableListInBoundary(Boundary selectionArea)
        {
            return _focusedGameWorld.GetMovableListInBoundary(selectionArea);
        }

        /// <summary>Get List of all movables in a given coordinate</summary>
        /// <param name="selectionCoordinate">Coordinate in which to search for movables</param>
        /// <returns>List of MovableItem interfaces giving access to the movables in the coordinate</returns>
        public List<MovableItem> GetMovableListInCoordinate(Coordinate selectionCoordinate)
        {
            return _focusedGameWorld.GetMovableListInBoundary(new Boundary(selectionCoordinate, selectionCoordinate));
        }

//        public List<StructureItem> GetStructureListInBoundary(Boundary selectionArea)
//        {
//            throw new NotImplementedException();
//        }

        /// <summary>
        /// Checks if there is a structure placed on the map at the given coordinate
        /// </summary>
        /// <param name="coordinate">Given coordinate to test for structures</param>
        /// <returns>True if any structure is located at the coordinate, false if there is none</returns>
        public bool IsStructureAtCoordinate(Coordinate coordinate)
        {
            return _focusedGameWorld.IsStructureAtCoordinate(coordinate);
        }

        /// <summary>
        /// Returns a StructureItem interface based on the structure's Guid
        /// </summary>
        /// <param name="structureGuid">Given Guid of the structure in question</param>
        /// <returns>StructureItem interface giving access to the structure with the matching Guid given</returns>
        public StructureItem GetStructureItem(Guid structureGuid)
        {
            return _focusedGameWorld.GetStructureItem(structureGuid);
        }

        /// <summary>
        /// Returns a StructureItem interface to a structure at a given coordinate
        /// </summary>
        /// <param name="selectionCoordinate">Coordinate in which to find the structure</param>
        /// <returns>StructureItem interface to structure at given coordinate</returns>
        /// <exception cref="ArgumentException">Exception thrown if no structure exists given
        /// location -- first test for IsStructureAtCoordinate before calling this method</exception>
        public StructureItem GetStructureAtCoordinate(Coordinate selectionCoordinate)
        {
            return _focusedGameWorld.GetStructureItemAtCoordinate(selectionCoordinate);
        }

        /// <summary>
        /// Returns a list of the Guids of all items that are selected by the game model on this map
        /// </summary>
        /// <returns>A list of the Guids of all items that are selected</returns>
        public List<Guid> GetSelectedIdList()
        {
            return _focusedGameWorld.GetSelectedIdList();
        }

        /// <summary>
        /// Returns a list of the MovableItems that are selected by the game model on the map
        /// </summary>
        /// <returns>A list of the MovableItems that are selected</returns>
        public List<MovableItem> GetSelectedMovableItemList()
        {
            return _focusedGameWorld.GetSelectedMovableItemList();
        }

        /// <summary>
        /// Selects items by a list of given Guid to items on the map
        /// This type of selection DELETES previous selections, in order to
        /// add to a selection, use AddToSelected* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of guids of items to be selected</param>
        public void SelectItemsById(List<Guid> itemListToSelect)
        {
            _focusedGameWorld.SelectItemsById(itemListToSelect);
        }

        /// <summary>
        /// Selects movables by a list of given MovableItems on the map
        /// This type of selection DELETES previous selections, in order to
        /// add to a selection, use AddToSelected* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of MovableItems to be selected</param>
        public void SelectMovableItems(List<MovableItem> itemListToSelect) {
            _focusedGameWorld.SelectMovableItems(itemListToSelect);
        }

        /// <summary>
        /// Adds items to selection by a list of given Guid to items on the map
        /// This type of selection ADDS to previous selections, in order to
        /// do a new selection, use regular Select* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of guids of items to be selected</param>
        public void AddToSelectedItemsById(List<Guid> itemListToSelect)
        {
            _focusedGameWorld.AddToSelectedItemsById(itemListToSelect);
        }

        /// <summary>
        /// Selects movables by a list of given MovableItems on the map
        /// This type of selection ADDS to previous selections, in order to
        /// do a new selection, use regular Select* methods instead.
        /// </summary>
        /// <param name="itemListToSelect">List of MovableItems to be selected</param>
        public void AddToSelectedMovableItems(List<MovableItem> itemListToSelect) {
            _focusedGameWorld.AddToSelectedMovableItems(itemListToSelect);
        }

        /// <summary>
        /// Clears all selected items from all prior selections on this map
        /// </summary>
        public void ClearSelectedItems()
        {
            _focusedGameWorld.ClearSelectedItems();
        }

        /// <summary>
        /// Returns a boundary describing the given gameworld's size and dimensions
        /// </summary>
        /// <returns>A boundary describing the given gameworld's size and dimensions</returns>
        public Boundary GetWorldBoundary()
        {
            return _focusedGameWorld.GetWorldBoundary();
        }

        /// <summary>
        /// Checks if there are items that need to be placed on the map -- this is important
        /// to check periodically because some actions may have byproducts that require adding
        /// one or more items to map.
        /// </summary>
        /// <returns>True if there are items waiting to be placed, false otherwise</returns>
        public bool IsThereAnItemToBePlaced()
        {
            return _focusedGameWorld.IsThereAnItemToBePlaced();
        }

        /// <summary>
        /// Get a list of item interfaces waiting to be placed in the view or to be recognized.
        /// This list should be taken periodically and cleared afterwards using the ClearItemsToBePlaced
        /// method. Items returned should be checked for their type using the ItemType field, then
        /// cast their respective derived classes.
        /// </summary>
        /// <returns>List of Item interfaces of all objects that need to be placed in the view</returns>
        public List<Item> GetItemsToBePlaced()
        {
            return _focusedGameWorld.GetItemsToBePlaced();
        }

        /// <summary>
        /// Clears the list of items to be placed in the view world. It is important that all items taken
        /// prior to calling this method be passed on to view and taken care of, and only then call this method.
        /// </summary>
        public void ClearItemsToBePlaced()
        {
            _focusedGameWorld.ClearItemsToBePlaced();
        }

        /// <summary>
        /// Get a MovableItem by a given Guid
        /// </summary>
        /// <param name="movableGuid">Guid of movable to get</param>
        /// <returns>A movableItem interface giving access to the movable</returns>
        public MovableItem GetMovableItem(Guid movableGuid)
        {
            return _focusedGameWorld.GetMovableItem(movableGuid);
        }

        /// <summary>
        /// Get a list of all MovableItems that are in motion (still have a path to take)
        /// </summary>
        /// <returns>A List of MovableItems that are in motion and have not completed their path</returns>
        public List<MovableItem> GetMovableItemsInMotion()
        {
            return _focusedGameWorld.GetMovableItemsInMotion();
        }

//        public ComponentStack AddComponentStack(Component component, Coordinate location, int amount) {
//            return _focusedGameWorld.AddComponentStack(component, location, amount);
//        }
//
//        public bool IsComponentStackAtCoordinate(Coordinate location) {
//            return _focusedGameWorld.IsComponentStackAtCoordinate(location);
//        }
//
        public ComponentStackGroup GetComponentStackGroupAtCoordinate(Coordinate location) {
            return _focusedGameWorld.GetComponentStackGroupAtCoordinate(location);
        }

        public bool HasMovableWithGuid(Guid movableGuid)
        {
            return (_focusedGameWorld.GetMovableIdList().Contains(movableGuid));
        }


    }
}