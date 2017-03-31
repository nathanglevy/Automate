using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.src.MapModelComponents;
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
            IHandlerResult result = null;
            MasterAction action0 = controller.OutputSched.Pull();
            MasterAction action1 = controller.OutputSched.Pull();
            Assert.AreEqual(ActionType.SelectPlayer, action0.Type);
            Assert.AreEqual(ActionType.SelectPlayer, action1.Type);
        }

    }
}

