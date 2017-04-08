using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.MapModelComponents;
using AutomateTests.Mocks;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestViewSelectionEvent
    {
        [TestMethod]
        public void TestCreateSelectionArgs_ExpectToPass()
        {
            IObserverArgs viewSelectionNotification = new ViewSelectionNotification(
                new Coordinate(0, 30, 0), new Coordinate(10, 0, 0), "ObjectID");
            Assert.IsNotNull(viewSelectionNotification);
        }

        [TestMethod]
        public void TestCreateSelectionHandler_ExpectToPss()
        {
            Handler<ObserverArgs> viewSelectionHandler = new ViewSelectionHandler();
            Assert.IsNotNull(viewSelectionHandler);

        }

        [TestMethod]
        public void TestHandleViewSelection_ExpectActionsToBeSentToView()
        {
            ObserverArgs viewSelectionNotification = new ViewSelectionNotification(
                new Coordinate(1, 1, 0), new Coordinate(20, 10, 0), "ObjectID");

            IHandler<ObserverArgs> viewSelectionHandler = new ViewSelectionHandler();

            var mockGameView = new MockGameView();
            var controller = new GameController(mockGameView, new MockGameModel());
            controller.RegisterHandler(viewSelectionHandler);
            IList<ThreadInfo> syncEvents = controller.Handle(viewSelectionNotification);
            foreach (var threadInfo in syncEvents)
            {
                threadInfo.SyncEvent.WaitOne(200);
            }
            foreach (var threadInfo in syncEvents)
            {
                Assert.AreEqual(false, threadInfo.Thread.IsAlive);
                threadInfo.SyncEvent.WaitOne(20);
            }


            Assert.AreEqual(2, controller.OutputSched.ActionsCount);
            MasterAction action0 = controller.OutputSched.Pull();
            MasterAction action1 = controller.OutputSched.Pull();
            Assert.AreEqual(ActionType.SelectPlayer, action0.Type);
            Assert.AreEqual(ActionType.SelectPlayer, action1.Type);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            ObserverArgs rightClickNotification = new ViewSelectionNotification(new Coordinate(1, 0, 0), new Coordinate(1, 10, 0),"ID");
            IHandler<ObserverArgs> rightClickNotificationHandler = new ViewSelectionHandler();
            Assert.IsTrue(rightClickNotificationHandler.CanHandle(rightClickNotification));

        }

        [TestMethod]
        public void TestCanAcknowledgeIncorrectArgs_ExpectFalse()
        {
            ObserverArgs clickNotification = new RightClickNotification(new Coordinate(0, 0, 0));
            IHandler<ObserverArgs> rightClickNotificationHandler = new ViewSelectionHandler();
            Assert.IsFalse(rightClickNotificationHandler.CanHandle(clickNotification));
        }

        [TestMethod]
        public void TestCannotAcknowledgeWithCorrectAction_ExpectTrue()
        {
            MasterAction selectAction = new SelectMovableAction(new Coordinate(1, 0, 0), "MyPlayer");
            IHandler<ObserverArgs> rightClickNotificationHandler = new ViewSelectionHandler();
            Assert.IsTrue(rightClickNotificationHandler.CanAcknowledge(selectAction));

        }

        [TestMethod]
        public void TestCannotAcknowledgeIncorrectAction_ExpectFalse()
        {
            MasterAction selectMovableAction = new MoveAction(new Coordinate(0, 0, 0), "MyPlayer");
            IHandler<ObserverArgs> rightClickNotificationHandler = new ViewSelectionHandler();
            Assert.IsFalse(rightClickNotificationHandler.CanAcknowledge(selectMovableAction));
        }

    }
}

