using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.src.Abstracts;
using Automate.Controller.src.Handlers.SelectionNotification;
using Automate.Controller.src.Interfaces;
using Automate.Controller.src.Modules;
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
            IList<ThreadInfo> syncEvents = controller.Handle(viewSelectionNotification, mockGameView.GetCallBack());
            foreach (var threadInfo in syncEvents)
            {
                threadInfo.SyncEvent.WaitOne(20);
            }
            foreach (var threadInfo in syncEvents)
            {
                Assert.AreEqual(false, threadInfo.Thread.IsAlive);
                threadInfo.SyncEvent.WaitOne(20);
            }


            Assert.AreEqual(1, mockGameView.Results.Count);
            IHandlerResult result = null;
            mockGameView.Results.TryPeek(out result);
            Assert.AreEqual(2, result.GetActions().Count);
            Assert.AreEqual(ActionType.SelectPlayer, result.GetActions()[0].Type);
            Guid action0Guid;
            Assert.IsTrue(Guid.TryParse(result.GetActions()[0].TargetId,out action0Guid));
            Assert.AreEqual(ActionType.SelectPlayer, result.GetActions()[1].Type);
            Guid action1Guid;
            Assert.IsTrue(Guid.TryParse(result.GetActions()[1].TargetId, out action1Guid));

        }

    }
}

