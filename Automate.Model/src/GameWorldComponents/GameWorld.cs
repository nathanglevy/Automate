using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.PathFinding;
using JetBrains.Annotations;
using Automate.Model.Tasks;

[assembly: InternalsVisibleTo("AutomateTests")]
namespace Automate.Model.GameWorldComponents
{
    //TODO: Need to do comments!
    public class GameWorld : IGameWorld
    {
        private readonly Dictionary<Guid, Movable> _movables = new Dictionary<Guid, Movable>();
        private readonly Dictionary<Coordinate, ComponentStackGroup> _componentStacks = new Dictionary<Coordinate, ComponentStackGroup>();
        private readonly Dictionary<Guid, Structure> _structures = new Dictionary<Guid, Structure>();
        private readonly Dictionary<Coordinate, Guid> _coordinateToStructureMap = new Dictionary<Coordinate, Guid>();
        private readonly HashSet<Guid> _selectedItems = new HashSet<Guid>();
        private readonly HashSet<Item> _itemsToBePlaced = new HashSet<Item>();
        private MapInfo _map;
        public Guid Guid { get; private set; }
        public TaskDelegator TaskDelegator { get; } = new TaskDelegator();

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

        public IMovable GetMovable(Guid movableGuid)
        {
            if (!_movables.ContainsKey(movableGuid))
                throw new ArgumentException("Cannot get movable with GUID -- does not exist!");
            return _movables[movableGuid];
        }

        public IStructure GetStructure(Guid structureGuid)
        {
            if (!_structures.ContainsKey(structureGuid))
                throw new ArgumentException("Cannot get structure with GUID -- does not exist!");
            return _structures[structureGuid];
        }

        public CellInfo GetCellInfo(Coordinate cellCoordinate)
        {
            if (!GetWorldBoundary().IsCoordinateInBoundary(cellCoordinate))
                throw new ArgumentOutOfRangeException(nameof(cellCoordinate), "cellcoordinate is out of map border bounds");
            return _map.GetCell(cellCoordinate);
        }

        public bool IsStructureAtCoordinate(Coordinate coordinate)
        {
            return _coordinateToStructureMap.ContainsKey(coordinate);
        }

        public IStructure GetStructureAtCoordinate(Coordinate coordinate)
        {
            if (!IsStructureAtCoordinate(coordinate))
                throw new ArgumentException("There is no structure at given coordinate");
            Guid structureGuid = _coordinateToStructureMap[coordinate];
            return _structures[structureGuid];
        }

        public IMovable CreateMovable(Coordinate coordinate, MovableType movableType)
        {
            Movable movable = new Movable(coordinate,movableType);
            _movables.Add(movable.GetId(), movable);
            _itemsToBePlaced.Add(movable);
            return movable;
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
                if (_movables.Any(pair => pair.Value.GetEffectiveCoordinate().Equals(coordinateInBoundary)))
                    return false;
            }
            return true;
        }

        //needs to check if it can even place the structure
        public IStructure CreateStructure(Coordinate coordinate, Coordinate dimensions, StructureType structureType) {
            //throw new NotImplementedException();
            //check that we can make the structure with these dimensions:
            if (!CanStructureBePlaced(coordinate, dimensions))
                throw new ArgumentException("cannot create structure in this location -- occupied!");
            Structure structure = new Structure(coordinate,dimensions,structureType);
            _structures.Add(structure.Guid, structure);
            _itemsToBePlaced.Add(structure);
            foreach (Coordinate coordinteInBoundary in structure.Boundary.GetListOfCoordinatesInBoundary())
            {
                _coordinateToStructureMap.Add(coordinteInBoundary,structure.Guid);
                if (!structureType.Equals(StructureType.NonBlocking))
                    _map.GetCell(coordinteInBoundary).SetPassability(false);
            }
            RecalculateMovablePaths();
            return structure;
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
                MovementPath movementPath = GetMovementPathWithLowestCostToCoordinate(startCoordinate, targetCoordinate);
                currentMovable.SetPath(movementPath);
                return true;
            }
            catch (NoPathFoundException)
            {
                if (currentMovable.IsInMotion())
                {
                    MovementPath movementPath = PathFinderAStar.FindShortestPath(_map,
                        currentMovable.GetEffectiveCoordinate(), currentMovable.GetEffectiveCoordinate());
                    currentMovable.SetPath(movementPath);
                }
                currentMovable.PathToTargetHasBeenBroken = true;
                return false;
            }
        }

        public List<Guid> GetMovableIdList()
        {
            return new List<Guid>(_movables.Keys);
        }

        public List<IMovable> GetMovableList()
        {
            List<IMovable> movableList = new List<IMovable>();
            foreach (Movable movablesValue in _movables.Values)
                movableList.Add(movablesValue);
            return movableList;
        }

        public List<Guid> GetSelectedIdList() {
            return new List<Guid>(_selectedItems);
        }

        public List<IMovable> GetSelectedMovableItemList() {
            List<IMovable> movableItemList = new List<IMovable>();
            foreach (Guid movable in _selectedItems) {
                movableItemList.Add(_movables[movable]);
            }
            return movableItemList;
        }

        public void SelectItemsById(List<Guid> itemListToSelect) {
            _selectedItems.Clear();
            _selectedItems.UnionWith(itemListToSelect);
        }

        public void SelectMovableItems(List<IMovable> itemListToSelect) {
            _selectedItems.Clear();
            AddToSelectedMovableItems(itemListToSelect);
        }

        public void AddToSelectedItemsById(List<Guid> itemListToSelect) {
            _selectedItems.UnionWith(itemListToSelect);
        }

        public void AddToSelectedMovableItems(List<IMovable> itemListToSelect) {
            foreach (IMovable movableItem in itemListToSelect) {
                _selectedItems.Add(movableItem.Guid);
            }
        }

        public void ClearSelectedItems() {
            _selectedItems.Clear();
        }

        public List<IMovable> GetMovableListInBoundary(Boundary boundary)
        {
            List<IMovable> movableList = new List<IMovable>();
            foreach (Guid movableId in _movables.Keys)
            {
                Coordinate currentMovableCoordinate = _movables[movableId].GetCurrentCoordinate();
                if (boundary.IsCoordinateInBoundary(currentMovableCoordinate))
                    movableList.Add(_movables[movableId]);
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

        public List<IMovable> GetMovablesInMotion()
        {
            return _movables.Values.Where(item => item.IsInMotion()).Select(item => item as IMovable).ToList();
        }

//        [Obsolete]
//        public ComponentStack AddComponentStack(Component component, Coordinate location, int amount)
//        {
//            if (_componentStacks.ContainsKey(location)) // && _componentStacks[location].GetType() != component.GetType())
//                throw new ArgumentException("Coordinate: " + location + " already has an item stack");
//            ComponentStack componentStack = new ComponentStack(component, amount);
//            _componentStacks[location] = componentStack;
//            return componentStack;
//        }

//        public void RemoveComponentStack(Coordinate location) {
//            if (!_componentStacks.ContainsKey(location))
//                throw new ArgumentException("Coordinate: " + location + " does not have an item stack to remove");
//            _componentStacks.Remove(location);
//        }

        public ComponentStackGroup GetComponentStackGroupAtCoordinate(Coordinate location)
        {
            if (!_componentStacks.ContainsKey(location))
                _componentStacks.Add(location, new ComponentStackGroup());
            return _componentStacks[location];
        }

        public void RecalculateMovablePaths()
        {
            IEnumerable<Movable> movablesInMotion = _movables.Where(pair => pair.Value.IsInMotion()).Select(pair => pair.Value);
            foreach (Movable movable in movablesInMotion)
            {
                IssueMoveCommand(movable.GetId(), movable.GetFinalDestination());
            }
        }

        //public Task CreateTask
        public MovementPath GetMovementPathWithLowestCostToBoundary(List<Coordinate> startCoordinates, Boundary endBoundary, bool inclusive)
        {
            MovementPath movementPath = PathFinderAStar.FindShortestPath(_map, startCoordinates, endBoundary, inclusive);
            return movementPath;
        }

        public MovementPath GetMovementPathWithLowestCostToCoordinate(Coordinate startCoordinate, Coordinate endCoordinate) {
            MovementPath movementPath = PathFinderAStar.FindShortestPath(_map, startCoordinate, endCoordinate);
            return movementPath;
        }

        public MovementPath GetMovementPathWithLowestCostToCoordinate(List<Coordinate> startCoordinates, Coordinate endCoordinate) {
            MovementPath movementPath = PathFinderAStar.FindShortestPath(_map, startCoordinates, endCoordinate);
            return movementPath;
        }

        public List<IStructure> GetStructuresList()
        {
            return _structures.Values.Select(item => item as IStructure).ToList();
        }

        public List<IMovable> GetMovableListInCoordinate(Coordinate selectionCoordinate)
        {
            return GetMovableListInBoundary(new Boundary(selectionCoordinate, selectionCoordinate));
        }

        public bool HasMovableWithGuid(Guid movableGuid)
        {
            return _movables.ContainsKey(movableGuid);
        }
    }
}