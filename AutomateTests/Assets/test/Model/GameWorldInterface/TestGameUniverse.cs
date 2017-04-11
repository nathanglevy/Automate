using System;
using System.Collections.Generic;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Model.GameWorldInterface {
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
            HashSet<GameWorldItem> gameWorldIdList = new HashSet<GameWorldItem>
            {
                GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2)),
                GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2)),
                GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2))
            };
            Assert.IsTrue(gameWorldIdList.SetEquals(GameUniverse.GetGameWorldItemsInUniverse()));
        }

        [TestMethod()]
        public void TestGetGameWorldItemById_ExpectSuccess()
        {
            Guid guid = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2)).Guid;
            GameUniverse.GetGameWorldItemById(guid);
        }

        [TestMethod()]
        public void TestGetGameWorldItemById_ExpectCorrectValue() {
            GameWorldItem gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 2));
            GameWorldItem gameWorldItem2 = GameUniverse.GetGameWorldItemById(gameWorldItem.Guid);
            Assert.AreEqual(gameWorldItem,gameWorldItem2);
        }
    }
}