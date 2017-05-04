using System;
using System.Collections.Generic;
using Automate.Model.GameWorldComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.MapModelComponents;

namespace AutomateTests.test.Model.GameWorldInterface {
    [TestClass()]
    public class TestGameUniverse {
        [TestMethod()]
        public void TestCreateGameWorld_ExpectSuccess()
        {
            GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateGameWorld_InvalidDimensions_ExpectArgumentException() {
            GameUniverse.CreateGameWorld(new Coordinate(0, 10, 2));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCreateGameWorld_GiveNullDimensions_ExpectArgumentNullException() {
            GameUniverse.CreateGameWorld(null);
        }

        [TestMethod()]
        public void TestGetGameWorldItemsInUniverse_ExpectSuccess() {
            GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
            GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
            GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
            GameUniverse.GetGameWorldItemsInUniverse();
        }

        [TestMethod()]
        public void TestGetGameWorldItemsInUniverse_ExpectCorrectValues() {
            HashSet<IGameWorld> gameWorldIdList = new HashSet<IGameWorld>
            {
                GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2)),
                GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2)),
                GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2))
            };
            Assert.IsTrue(gameWorldIdList.IsProperSubsetOf(GameUniverse.GetGameWorldItemsInUniverse()));
        }

        [TestMethod()]
        public void TestGetGameWorldItemById_ExpectSuccess()
        {
            Guid guid = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2)).Guid;
            GameUniverse.GetGameWorldItemById(guid);
        }

        [TestMethod()]
        public void TestGetGameWorldItemById_ExpectCorrectValue() {
            IGameWorld gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
            IGameWorld gameWorldItem2 = GameUniverse.GetGameWorldItemById(gameWorldItem.Guid);
            Assert.AreEqual(gameWorldItem,gameWorldItem2);
        }
    }
}