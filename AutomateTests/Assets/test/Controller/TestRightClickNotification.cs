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
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
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

            IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();

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


            Assert.AreEqual(2, controller.OutputSched.ItemsCount);
            Assert.AreEqual(ActionType.Movement, controller.OutputSched.Pull().Type);
            Assert.AreEqual(ActionType.Movement, controller.OutputSched.Pull().Type);


        }

        private Guid GetMockGameWorld()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 3, 0), MovableType.NormalHuman);
            var movableItem2 = gameWorldItem.CreateMovable(new Coordinate(7, 3, 0), MovableType.NormalHuman);
            gameWorldItem.SelectMovableItems(new List<MovableItem>() { movableItem , movableItem2});
            return gameWorldItem.Guid;
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            ObserverArgs rightClickNotification = new RightClickNotification(new Coordinate(1, 0, 0));
            IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();
            Assert.IsTrue(rightClickNotificationHandler.CanHandle(rightClickNotification));

        }

        [TestMethod]
        public void TestCanAcknowledgeIncorrectArgs_ExpectFalse()
        {
            ObserverArgs clickNotification = new ViewSelectionNotification(new Coordinate(0, 0, 0), new Coordinate(10, 10, 0),"MyID");
            IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();
            Assert.IsFalse(rightClickNotificationHandler.CanHandle(clickNotification));
        }

        [TestMethod]
        public void TestCanAcknowledgeWithCorrectAction_ExpectTrue()
        {
            MasterAction moveAction = new MoveAction(new Coordinate(1, 0, 0), new Coordinate(1, 1, 0), "MyPlayer");
            IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();
            Assert.IsTrue(rightClickNotificationHandler.CanAcknowledge(moveAction));

        }

        [TestMethod]
        public void TestCanAcknowledgeIncorrectAction_ExpectFalse()
        {
            MasterAction moveAction = new SelectMovableAction(new Coordinate(0, 0, 0),"MyPlayer");
            IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();
            Assert.IsFalse(rightClickNotificationHandler.CanAcknowledge(moveAction));
        }

        [TestMethod]
        public void TestAcknowledgeMoveAction_ExpectNewMoveActionWithXAxisGreaterByOne()
        {

            ObserverArgs viewSelectionNotification = new RightClickNotification(
                new Coordinate(10, 10, 0));

            IHandler<ObserverArgs> rightClickNotificationHandler = new RightClickNotificationHandler();

            var mockGameView = new MockGameView();
            var mockGameModel = GetMockGameWorld();
            var controller = new GameController((IGameView) mockGameView);
            controller.FocusGameWorld(mockGameModel);

            IList<ThreadInfo> syncEvents = controller.Handle(viewSelectionNotification);
            foreach (var threadInfo in syncEvents)
            {
                threadInfo.SyncEvent.WaitOne(200);
            }

            Assert.AreEqual(2, controller.OutputSched.ItemsCount);
            MasterAction action1 = controller.OutputSched.Pull();
            MasterAction action2 = controller.OutputSched.Pull();
            Assert.AreEqual(ActionType.Movement, action1.Type);
            Assert.AreEqual(ActionType.Movement, action2.Type);


            var acknowledgeResult = rightClickNotificationHandler.Acknowledge(action1,new HandlerUtils(mockGameModel, null, null));
            Assert.AreEqual(1,acknowledgeResult.GetItems().Count);
            var nextMoveAction = acknowledgeResult.GetItems()[0];
            Assert.IsNotNull(nextMoveAction);
            var realNextMoveAction = nextMoveAction as MoveAction;
            Assert.IsNotNull(realNextMoveAction);
            Assert.AreEqual(new Coordinate(5,5,0),realNextMoveAction.To);

        }


    }
}
