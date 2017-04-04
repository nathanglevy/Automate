using System;
using System.Collections.Generic;
using Model.MapModelComponents;

namespace Model.GameWorldComponents {
    public static class GameUniverse {
        private static Dictionary<Guid, GameWorld> _gameWorldDictionary = new Dictionary<Guid, GameWorld>();

        public static GameWorldItem CreateGameWorld(Coordinate mapDimensions) {
            GameWorld newGameWorld = new GameWorld(mapDimensions);
            _gameWorldDictionary.Add(newGameWorld.Guid, newGameWorld);
            GameWorldItem gameWorldItem = new GameWorldItem(newGameWorld);
            return gameWorldItem;
        }

        public static List<GameWorldItem> GetGameWorldItemsInUniverse()
        {
            List<GameWorldItem> result = new List<GameWorldItem>();
            foreach (GameWorld gameWorld in _gameWorldDictionary.Values)
            {
                result.Add(new GameWorldItem(gameWorld));
            }
            return result;
        }

        public static GameWorldItem GetGameWorldItemById(Guid gameWorldGuid)
        {
            return new GameWorldItem(_gameWorldDictionary[gameWorldGuid]);
        }
    }
}
