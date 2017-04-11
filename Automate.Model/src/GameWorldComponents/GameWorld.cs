using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("AutomateTests")]
namespace Automate.Model.GameWorldComponents
{
    //TODO: Need to do comments!
    public class GameWorld
    {
        private Dictionary<Guid, Movable> _movables = new Dictionary<Guid, Movable>();
        private Dictionary<Guid, Structure> _structures = new Dictionary<Guid, Structure>();
        private Dictionary<Coordinate, Guid> _coordinateToStructureMap = new Dictionary<Coordinate, Guid>();
        private HashSet<Guid> _selectedItems = new HashSet<Guid>();
        private HashSet<Item> _itemsToBePlaced = new HashSet<Item>();
        private MapInfo _map;
        public Guid Guid { get; private set; }

        internal GameWorld(Coordinate mapDimensions)
        {
            if (mapDimensions == null)
                throw new ArgumentNullException();

            Coordinate bottomRight = mapDimensions - new Coordinate(1, 1, 1);
            Coordinate topLeft = new Coordinate(0, 0, 0);

            if (!(bottomRight >= topLeft))
                throw new ArgumentException("dimensions must all be positive");

            _map = new MapInfo(topLeft, bottomRight);
            _map.FillMapWithCells(new CellInfo(true, 1));
            foreach (Coordinate coordinate in _map.GetBoundary().GetListOfCoordinatesInBoundary())
            {
                _itemsToBePlaced.Add(new CellItem(this, coordinate));
            }
            Guid = Guid.NewGuid();
        }

        public Movable GetMovable(Guid movableGuid)
        {
            return _movables[movableGuid];
        }

        public Structure GetStructure(Guid structureGuid)
        {
            return _structures[structureGuid];
        }

        public CellInfo GetCellInfo(Coordinate cellCoordinate)
        {
            return _map.GetCell(cellCoordinate);
        }

        public MovableItem GetMovableItem(Guid movableGuid) {
            return new MovableItem(this, movableGuid);
        }

        public StructureItem GetStructureItem(Guid structureGuid)
        {
            return new StructureItem(this, structureGuid);
        }

        public bool IsStructureAtCoordinate(Coordinate coordinate)
        {
            return _coordinateToStructureMap.ContainsKey(coordinate);
        }

        public StructureItem GetStructureItemAtCoordinate(Coordinate coordinate)
        {
            if (!IsStructureAtCoordinate(coordinate))
                throw new ArgumentException("There is no structure at given coordinate");
            Guid structureGuid = _coordinateToStructureMap[coordinate];
            return new StructureItem(this, structureGuid);
        }

        public MovableItem CreateMovable(Coordinate coordinate, MovableType movableType)
        {
            Movable movable = new Movable(coordinate,movableType);
            MovableItem movableItem = new MovableItem(this, movable.GetId());
            _itemsToBePlaced.Add(movableItem);
            _movables.Add(movable.GetId(),movable);
            return movableItem;
        }

        public bool CanStructureBePlaced(Coordinate coordinate, Coordinate dimensions)
        {
            Boundary boundaryToCheck = new Boundary(coordinate, coordinate + dimensions - new Coordinate(1,1,1));
//            foreach (Structure structureValue in _structures.Values)
//            {
//                if (!structureValue.GetStructureBoundary().IsBoundaryDisjointToBoundary(boundaryToCheck))
//                    return false;
//            }
//            return true;

            foreach (Coordinate coordinateInBoundary in boundaryToCheck.GetListOfCoordinatesInBoundary())
            {
                if (_coordinateToStructureMap.ContainsKey(coordinateInBoundary))
                    return false;
            }
            return true;
        }

        //needs to check if it can even place the structure
        public StructureItem CreateStructure(Coordinate coordinate, Coordinate dimensions, StructureType structureType) {
            //throw new NotImplementedException();
            //check that we can make the structure with these dimensions:
            if (!CanStructureBePlaced(coordinate, dimensions))
                throw new ArgumentException("cannot create structure in this location -- occupied!");
            Structure structure = new Structure(coordinate,dimensions,structureType);
            StructureItem structureItem = new StructureItem(this, structure.Guid);
            _structures.Add(structure.Guid, structure);
            _itemsToBePlaced.Add(structureItem);
            foreach (Coordinate coordinteInBoundary in structure.GetStructureBoundary().GetListOfCoordinatesInBoundary())
            {
                _coordinateToStructureMap.Add(coordinteInBoundary,structure.Guid);
                if (!structureType.Equals(StructureType.NonBlocking))
                    _map.GetCell(coordinteInBoundary).SetPassability(false);
            }
            return structureItem;
        }

        //TODO: need to test false issue move command
        public bool IssueMoveCommand(Guid id, [NotNull] Coordinate targetCoordinate)
        {
            if (!_movables.ContainsKey(id))
                throw new ArgumentException("Given movable Guid is not defined as a movable in this world");
            Movable currentMovable = _movables[id];
            Coordinate startCoordinate = currentMovable.GetEffectiveCoordinate();
            try
            {
                MovementPath movementPath = PathFinderAStar.FindShortestPath(_map, startCoordinate, targetCoordinate);
                currentMovable.SetPath(movementPath);
                return true;
            }
            catch (NoPathFoundException)
            {
                return false;
            }
        }

        public List<Guid> GetMovableIdList()
        {
            return new List<Guid>(_movables.Keys);
        }

        public List<MovableItem> GetMovableItemList()
        {
            List<MovableItem> movableItemList = new List<MovableItem>();
            foreach (Guid movable in _movables.Keys)
            {
                movableItemList.Add(new MovableItem(this,movable));
            }
            return movableItemList;
        }

        public List<Guid> GetSelectedIdList() {
            return new List<Guid>(_selectedItems);
        }

        public List<MovableItem> GetSelectedMovableItemList() {
            List<MovableItem> movableItemList = new List<MovableItem>();
            foreach (Guid movable in _selectedItems) {
                movableItemList.Add(new MovableItem(this, movable));
            }
            return movableItemList;
        }

        public void SelectItemsById(List<Guid> itemListToSelect) {
            _selectedItems.Clear();
            _selectedItems.UnionWith(itemListToSelect);
        }

        public void SelectMovableItems(List<MovableItem> itemListToSelect) {
            _selectedItems.Clear();
            AddToSelectedMovableItems(itemListToSelect);
        }

        public void AddToSelectedItemsById(List<Guid> itemListToSelect) {
            _selectedItems.UnionWith(itemListToSelect);
        }

        public void AddToSelectedMovableItems(List<MovableItem> itemListToSelect) {
            foreach (MovableItem movableItem in itemListToSelect) {
                _selectedItems.Add(movableItem.Guid);
            }
        }

        public void ClearSelectedItems() {
            _selectedItems.Clear();
        }

        public List<MovableItem> GetMovableListInBoundary(Boundary boundary)
        {
            List<MovableItem> movableList = new List<MovableItem>();
            foreach (Guid movableId in _movables.Keys)
            {
                Coordinate currentMovableCoordinate = _movables[movableId].GetCurrentCoordinate();
                if (boundary.IsCoordinateInBoundary(currentMovableCoordinate))
                    movableList.Add(new MovableItem(this, movableId));
            }
            return movableList;
        }

        public Boundary GetWorldBoundary()
        {
            return _map.GetBoundary();
        }

        public bool IsThereAnItemToBePlaced()
        {
            return (_itemsToBePlaced.Count > 0);
        }

        public List<Item> GetItemsToBePlaced()
        {
            return new List<Item>(_itemsToBePlaced);
        }

        public void ClearItemsToBePlaced()
        {
            _itemsToBePlaced.Clear();
        }

        public List<MovableItem> GetMovableItemsInMotion()
        {
            List<MovableItem> result = new List<MovableItem>();
            foreach (Movable movable in _movables.Values)
            {
                if (movable.IsInMotion())
                    result.Add(new MovableItem(this,movable.GetId()));
            }
            return result;
        }
    }
}