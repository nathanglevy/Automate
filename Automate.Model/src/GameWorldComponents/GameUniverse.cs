using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldComponents {

    /// <summary>
    /// Game Universe Interface, this is the main object used to interract with the model.
    /// All new game worlds should be created using this interface.
    /// The Game Universe holds all existing worlds currently loaded by the game and should be accessed through it.
    /// </summary>
    public static class GameUniverse {
        private static Dictionary<Guid, IGameWorld> _gameWorldDictionary = new Dictionary<Guid, IGameWorld>();

        /// <summary>
        /// Create a new Game World and get the GameWorld interface item.
        /// </summary>
        /// <param name="mapDimensions">The map dimensions of the world to generate. For example, giving (2,2,1) will generate a 2x2 map with one layer.</param>
        /// <returns>Returns a GameWorld interface item to a new Game World</returns>
        public static IGameWorld CreateGameWorld(Coordinate mapDimensions) {
            IGameWorld newGameWorld = new GameWorld(mapDimensions);
            _gameWorldDictionary.Add(newGameWorld.Guid, newGameWorld);
            return newGameWorld;
        }

        /// <summary>
        /// Get all of Game Worlds existing in game via a list of GameWorld interface items
        /// </summary>
        /// <returns>List of all GameWorld interface items</returns>
        public static List<IGameWorld> GetGameWorldItemsInUniverse()
        {
            return _gameWorldDictionary.Values.ToList();
        }

        /// <summary>
        /// Get a specific GameWorld interface item by a given Guid
        /// </summary>
        /// <param name="gameWorldGuid">Guid of GameWorld to fetch the interface for</param>
        /// <returns>A GameWorld interface item matching the given Guid</returns>
        public static IGameWorld GetGameWorldItemById(Guid gameWorldGuid)
        {
            return _gameWorldDictionary[gameWorldGuid];
        }
    }
}
