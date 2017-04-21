using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using AutomateTests.Mocks;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestViewSelectionEvent
    {
        private int THREADS_TIME_OUT = 1000;
        private List<MasterAction> _handledActions = new List<MasterAction>();
        [TestMethod]
        public void TestCreateSelectionArgs_ExpectToPass()
        {
            IObserverArgs viewSelectionNotification = new ViewSelectionNotification(
                new Coordinate(0, 30, 0), new Coordinate(10, 0, 0), Guid.NewGuid());
            Assert.IsNotNull(viewSelectionNotification);
        }

        [TestMethod]
        public void TestCreateSelectionHandler_ExpectToPss()
        {
            var viewSelectionHandler = new ViewSelectionHandler();
            Assert.IsNotNull(viewSelectionHandler);

        }

        [TestMethod]
        public void TestHandleViewSelection_ExpectActionsToBeSentToView()
        {
            ObserverArgs viewSelectionNotification = new ViewSelectionNotification(
                new Coordinate(1, 1, 0), new Coordinate(20, 10, 0), Guid.NewGuid());


            var mockGameView = new MockGameView();
            var gameModel = GetMockGameModel();
            var controller = new GameController((IGameView) mockGameView);
            controller.FocusGameWorld(gameModel);

           // mockGameView.PerformOnStart();

            mockGameView.PerformCompleteUpdate();

            IList<ThreadInfo> syncEvents = controller.Handle(viewSelectionNotification);
            foreach (var threadInfo in syncEvents)
            {
                threadInfo.SyncEvent.WaitOne(THREADS_TIME_OUT);
            }
            foreach (var threadInfo in syncEvents)
            {
                  Assert.AreEqual(false, threadInfo.Thread.IsAlive);
                threadInfo.SyncEvent.WaitOne(THREADS_TIME_OUT);
            }

            mockGameView.PerformCompleteUpdate();

            Assert.AreEqual(404, controller.OutputSched.ItemsCount);
          
            for (int i = 0; i < 402; i++)
            {
                MasterAction action = controller.OutputSched.Pull();
                Assert.AreEqual(ActionType.PlaceGameObject, action.Type);
            }
            MasterAction action0 = controller.OutputSched.Pull();
            MasterAction action1 = controller.OutputSched.Pull();
            Assert.AreEqual(ActionType.SelectPlayer, action0.Type);
            Assert.AreEqual(ActionType.SelectPlayer, action1.Type);
        }

        private void saveActions(ViewHandleActionArgs args)
        {
            _handledActions.Add(args.Action);
        }

        private Guid GetMockGameModel()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 3, 0), MovableType.NormalHuman);
            var movableItem2 = gameWorldItem.CreateMovable(new Coordinate(7, 3, 0), MovableType.NormalHuman);
            gameWorldItem.SelectMovableItems(new List<MovableItem>() { movableItem, movableItem2 });
            return gameWorldItem.Guid;
        }



        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            ObserverArgs rightClickNotification = new ViewSelectionNotification(new Coordinate(1, 0, 0), new Coordinate(1, 10, 0),Guid.NewGuid());
            var rightClickNotificationHandler = new ViewSelectionHandler();
            Assert.IsTrue(rightClickNotificationHandler.CanHandle(rightClickNotification));

        }


    }
}

