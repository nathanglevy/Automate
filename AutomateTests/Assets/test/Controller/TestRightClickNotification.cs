using System;
using System.Collections.Generic;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Handlers.RightClickNotification;
using Assets.src.Controller.Handlers.SelectionNotification;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using Assets.src.Model.MapModelComponents;
using AutomateTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestRightClickNotification
    {
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
            {
                ObserverArgs viewSelectionNotification = new RightClickNotification(
                    new Coordinate(0, 0, 0));

                IHandler<ObserverArgs> viewSelectionHandler = new RightClickNotificationHandler();

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
                Assert.AreEqual(ActionType.Movement, result.GetActions()[0].Type);
                Assert.AreEqual(ActionType.Movement, result.GetActions()[1].Type);

            }
        }
    }
}
