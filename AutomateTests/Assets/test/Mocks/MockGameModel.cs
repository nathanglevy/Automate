using System;
using System.Collections.Generic;
using Automate.Model;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.PathFinding;
using Automate.Model.Requirements;
using Automate.Model.Tasks;

namespace AutomateTests.test.Mocks
{
    public class MockGameModel : IGameWorld
    {
        private IGameWorld _testingGameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 20));
        private List<IMovable> _selected = new List<IMovable>();
        Dictionary<string, IMovable> _movableItems;
        public MockGameModel()
        {
            
            _movableItems = new Dictionary<string, IMovable>();
            _movableItems.Add("Player1", _testingGameWorld.CreateMovable(new Coordinate(1, 1, 1), MovableType.NormalHuman));
            _movableItems.Add("Player2", _testingGameWorld.CreateMovable(new Coordinate(7, 7, 7), MovableType.NormalHuman));
            _movableItems.Add("Player3", _testingGameWorld.CreateMovable(new Coordinate(19, 19, 19), MovableType.NormalHuman));
        }

        public IMovable GetMovableItem(Guid movableGuid)
        {
            foreach (var movableItemsValue in _movableItems.Values)
            {
                if (movableItemsValue.Guid.Equals(movableGuid))
                    return movableItemsValue;
            }
            throw new Exception("cannot find any player with ID: " + movableGuid.ToString());
        }

        public List<IMovable> GetMovableListInBoundary(Boundary selectionArea)
        {
            IMovable player1;
            _movableItems.TryGetValue("Player1", out player1);
            IMovable player2;
            _movableItems.TryGetValue("Player2", out player2);
            return new List<IMovable>()
            {
                player1,
                player2
            };
        }

  

        public void AddToSelectedMovableItems(List<IMovable> itemListToSelect)
        {
            _selected.AddRange(itemListToSelect);
        }

        public List<IMovable> GetSelectedMovableItemList()
        {

            IMovable player1;
            _movableItems.TryGetValue("Player1", out player1);
            IMovable player2;
            _movableItems.TryGetValue("Player2", out player2);
            return new List<IMovable>()
            {
                player1,
                player2
            };
        }

        public List<string> GetSelectedMovables()
        {
            return new List<string>() {"10","17"};
        }

        public Guid GetGuidByAlias(string playerName)
        {
            if (!_movableItems.ContainsKey(playerName))
                throw new Exception(String.Format("cannot find any key called: {0} at Mock Players List",playerName));
            IMovable playerObj;
            if (_movableItems.TryGetValue(playerName, out playerObj))
            {
                return playerObj.Guid;
            }
            else
            {
                throw new Exception(String.Format("Failed to get the Key:{0} from the MockPlayers Dict",playerName));
            }
            
        }

        public TaskDelegator TaskDelegator { get; }
        public Guid Guid { get; }
        public IMovable CreateMovable(Coordinate spawnCoordinate, MovableType movableType)
        {
            throw new NotImplementedException();
        }

        public IStructure CreateStructure(Coordinate spawnTopLeftCoordinate, Coordinate dimensions, StructureType structureType)
        {
            throw new NotImplementedException();
        }

        public List<IMovable> GetMovableListInCoordinate(Coordinate selectionCoordinate)
        {
            throw new NotImplementedException();
        }

        public bool IsStructureAtCoordinate(Coordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public IStructure GetStructureAtCoordinate(Coordinate selectionCoordinate)
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

        public void SelectMovableItems(List<IMovable> itemListToSelect)
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

        public Boundary GetWorldBoundary()
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

        public List<IMovable> GetMovablesInMotion()
        {
            throw new NotImplementedException();
        }

        public ComponentStackGroup GetComponentStackGroupAtCoordinate(Coordinate location)
        {
            throw new NotImplementedException();
        }

        public bool HasMovableWithGuid(Guid movableGuid)
        {
            throw new NotImplementedException();
        }

        public MovementPath GetMovementPathWithLowestCostToCoordinate(Coordinate startCoordinate, Coordinate endCoordinate)
        {
            throw new NotImplementedException();
        }

        public MovementPath GetMovementPathWithLowestCostToCoordinate(List<Coordinate> startCoordinates, Coordinate endCoordinate)
        {
            throw new NotImplementedException();
        }

        public MovementPath GetMovementPathWithLowestCostToBoundary(List<Coordinate> startCoordinates, Boundary endBoundary, bool inclusive)
        {
            throw new NotImplementedException();
        }

        public List<IStructure> GetStructuresList()
        {
            throw new NotImplementedException();
        }

        public List<IMovable> GetMovableList()
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetMovableIdList()
        {
            throw new NotImplementedException();
        }

        public bool IssueMoveCommand(Guid id, Coordinate targetCoordinate)
        {
            throw new NotImplementedException();
        }

        public bool CanStructureBePlaced(Coordinate coordinate, Coordinate dimensions)
        {
            throw new NotImplementedException();
        }

        public IStructure GetStructure(Guid structureGuid)
        {
            throw new NotImplementedException();
        }

        public IMovable GetMovable(Guid movableGuid)
        {
            throw new NotImplementedException();
        }

        public RequirementAgent RequirementAgent { get; }
    }
}