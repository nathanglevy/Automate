using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using AutomateTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Controller
{
    [TestClass]
    public class TestModelPushToView
    {
        [TestMethod]
        public void TestRegisterOnItemsToBePlacedAdd_ExpectATrigger()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            gameWorldItem.CreateMovable(new Coordinate(3, 3, 0), MovableType.FastHuman);


            var mockGameView = new MockGameView();
            var gameController = new GameController((IGameView) mockGameView);
            gameController.FocusGameWorld(gameWorldItem.Guid);
            mockGameView.PerformOnUpdateStart();
            mockGameView.PerformOnUpdate();

            gameController.OutputSched.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(101,gameController.OutputSched.ItemsCount);
            Assert.AreEqual(ActionType.PlaceGameObject,gameController.OutputSched.Pull().Type);
        }
    }
}
