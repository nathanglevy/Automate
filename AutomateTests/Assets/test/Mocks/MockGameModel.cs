using System;
using System.Collections.Generic;
using Automate.Model;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace AutomateTests.test.Mocks
{
    public class MockGameModel : IModelAbstractionLayer
    {
        private GameWorldItem _testingGameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 20));
        private List<MovableItem> _selected = new List<MovableItem>();
        Dictionary<string, MovableItem> _movableItems;
        public MockGameModel()
        {
            
            _movableItems = new Dictionary<string, MovableItem>();
            _movableItems.Add("Player1", _testingGameWorld.CreateMovable(new Coordinate(1, 1, 1), MovableType.NormalHuman));
            _movableItems.Add("Player2", _testingGameWorld.CreateMovable(new Coordinate(7, 7, 7), MovableType.NormalHuman));
            _movableItems.Add("Player3", _testingGameWorld.CreateMovable(new Coordinate(19, 19, 19), MovableType.NormalHuman));
        }

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
            foreach (var movableItemsValue in _movableItems.Values)
            {
                if (movableItemsValue.Guid.Equals(movableGuid))
                    return movableItemsValue;
            }
            throw new Exception("cannot find any player with ID: " + movableGuid.ToString());
        }

        public Boundary GetWorldBoundary()
        {
            throw new NotImplementedException();
        }

        public List<MovableItem> GetMovableListInBoundary(Boundary selectionArea)
        {
            MovableItem player1;
            _movableItems.TryGetValue("Player1", out player1);
            MovableItem player2;
            _movableItems.TryGetValue("Player2", out player2);
            return new List<MovableItem>()
            {
                player1,
                player2
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

            MovableItem player1;
            _movableItems.TryGetValue("Player1", out player1);
            MovableItem player2;
            _movableItems.TryGetValue("Player2", out player2);
            return new List<MovableItem>()
            {
                player1,
                player2
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

        public Guid GetGuidByAlias(string playerName)
        {
            if (!_movableItems.ContainsKey(playerName))
                throw new Exception(String.Format("cannot find any key called: {0} at Mock Players List",playerName));
            MovableItem playerObj;
            if (_movableItems.TryGetValue(playerName, out playerObj))
            {
                return playerObj.Guid;
            }
            else
            {
                throw new Exception(String.Format("Failed to get the Key:{0} from the MockPlayers Dict",playerName));
            }
            
        }
    }
}