using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.src.MapModelComponents;
using AutomateTests.test.Mocks;
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
                    new Coordinate(20, 10, 0));

                IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();

                var mockGameView = new MockGameView();
                var controller = new GameController(mockGameView, new MockGameModel());
                controller.RegisterHandler(rightClickNotificationHandler);
                IList<ThreadInfo> syncEvents = controller.Handle(viewSelectionNotification);
                foreach (var threadInfo in syncEvents)
                {
                    threadInfo.SyncEvent.WaitOne(20);
                }
                foreach (var threadInfo in syncEvents)
                {
                    Assert.AreEqual(false, threadInfo.Thread.IsAlive);
                    threadInfo.SyncEvent.WaitOne(20);
                }


                Assert.AreEqual(2, controller.OutputSched.ActionsCount);
                Assert.AreEqual(ActionType.Movement, controller.OutputSched.Pull().Type);
                Assert.AreEqual(ActionType.Movement, controller.OutputSched.Pull().Type);

            }
        }
    }
}
