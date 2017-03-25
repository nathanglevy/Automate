using System.Collections.Generic;
using System.Threading;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Handlers.SelectionNotification;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using AutomateTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.Model.MapModelComponents;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestViewSelectionEvent
    {
        private AutoResetEvent _sync1 = new AutoResetEvent(false);
        [TestMethod]
        public void TestCreateSelectionArgs_ExpectToPass()
        {
            IObserverArgs viewSelectionNotification = new ViewSelectionNotification(
                new Coordinate(0, 0, 0), new Coordinate(10, 10, 10));
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
                new Coordinate(0, 0, 0), new Coordinate(10, 10, 0));

            IHandler<ObserverArgs> viewSelectionHandler = new ViewSelectionHandler();

            var mockGameView = new MockGameView();
            var controller = new GameController(mockGameView, new MockGameModel());
            controller.RegisterHandler(viewSelectionHandler);
            IList<ThreadInfo> syncEvents = controller.Handle(viewSelectionNotification, mockGameView.GetCallBack());
            foreach (var threadInfo in syncEvents)
            {
                threadInfo.SyncEvent.WaitOne(20);
            }
            Assert.AreEqual(1, mockGameView.Results.Count);
            IHandlerResult result = null;
            mockGameView.Results.TryPeek(out result);
            Assert.AreEqual(2, result.GetActions().Count);
            Assert.AreEqual(ActionType.SelectPlayer, result.GetActions()[0].Type);
            Assert.AreEqual("AhmadHamdan", result.GetActions()[0].TargetId);
            Assert.AreEqual(ActionType.SelectPlayer, result.GetActions()[1].Type);
            Assert.AreEqual("NaphLevy", result.GetActions()[1].TargetId);


        }
    }
}

