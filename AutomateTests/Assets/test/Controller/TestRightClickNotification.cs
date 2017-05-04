using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using AutomateTests.Mocks;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestRightClickNotification
    {
        private  const int TIMEOUT = 200;
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            ObserverArgs RightClickNotification = new RightClickNotification(new Coordinate(20, 20, 0));
            Assert.IsNotNull(RightClickNotification);
        }

        [TestMethod]
        public void TestCreateNewHandler_ShouldPass()
        {
            var rightClickNotificationHandler = new RightClickNotificationHandler();
            Assert.IsNotNull(rightClickNotificationHandler);
        }

        [TestMethod]
        public void TestRightClickNotificationHandling_ExpectMoveActionsInQueue()
        {

            ObserverArgs viewSelectionNotification = new RightClickNotification(
                new Coordinate(18, 10, 0));

            IHandler<IObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();

            var mockGameView = new MockGameView();
            Guid gameModel = GetMockGameWorld();
            var controller = new GameController((IGameView) mockGameView);
            controller.FocusGameWorld(gameModel);
            // controller.RegisterHandler(rightClickNotificationHandler);
            IList<ThreadInfo> threadInfos = controller.Handle(viewSelectionNotification);
            foreach (var threadInfo in threadInfos)
            {
                //threadInfo.SyncEvent.WaitOne();
                threadInfo.SyncEvent.WaitOne(TIMEOUT);
            }

            controller.OutputSched.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(2, controller.OutputSched.ItemsCount);
            Assert.AreEqual(ActionType.Movement, controller.OutputSched.Pull().Type);
            Assert.AreEqual(ActionType.Movement, controller.OutputSched.Pull().Type);


        }

        private Guid GetMockGameWorld()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 3, 0), MovableType.NormalHuman);
            var movableItem2 = gameWorldItem.CreateMovable(new Coordinate(7, 3, 0), MovableType.NormalHuman);
            gameWorldItem.SelectMovableItems(new List<IMovable>() { movableItem , movableItem2});
            return gameWorldItem.Guid;
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            ObserverArgs rightClickNotification = new RightClickNotification(new Coordinate(1, 0, 0));
            IHandler<IObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();
            Assert.IsTrue(rightClickNotificationHandler.CanHandle(rightClickNotification));

        }

        //[TestMethod]
        //public void TestOnActionFinish_ExpectEventToBeFired()
        //{
        //    ObserverArgs rightClickNotification = new RightClickNotification(new Coordinate(1, 0, 0));
        //    rightClickNotification.OnComplete += OnRightClickComplete;
        //    IHandler<IObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();
        //    Assert.IsTrue(rightClickNotificationHandler.CanHandle(rightClickNotification));
        //    Assert.IsTrue(_onCompleteFired);
        //}

        private void OnRightClickComplete(ControllerNotificationArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
