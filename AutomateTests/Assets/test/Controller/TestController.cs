using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using AutomateTests.Mocks;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestController
    {

        private ConcurrentQueue<MasterAction> _queue = new ConcurrentQueue<MasterAction>();
        private AutoResetEvent _syncEvent = new AutoResetEvent(false);

        private AutoResetEvent _ackSync = new AutoResetEvent(false);

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IGameView view = new MockGameView();
            Guid model = GetMockGameWorld();

            GameController gameController = new GameController(view);
            gameController.FocusGameWorld(model);
            Assert.IsNotNull(gameController);
            Assert.IsNotNull(gameController.View);
            Assert.IsNotNull(gameController.Model);
        }

        private Guid GetMockGameWorld()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            gameWorldItem.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);
            gameWorldItem.CreateMovable(new Coordinate(7, 7, 0), MovableType.NormalHuman);
            return gameWorldItem.Guid;
        }

        [TestMethod]
        public void TestHandlersCount_Expect0()
        {
            Guid gameModel = GetMockGameWorld();
            IGameView gameview = new MockGameView();
            IGameController gameController = new GameController(gameview);
            gameController.FocusGameWorld(gameModel);

            Assert.IsTrue(gameController.GetHandlersCount() > 0);

        }

        [TestMethod]
        public void TestRegisterHandle_ExpectHandleToBeAddedtoList()
        {
            Guid gameModel = GetMockGameWorld();
            IGameView gameview = new MockGameView();
            IGameController gameController = new GameController(gameview);
            gameController.FocusGameWorld(gameModel);
            var mockHandler = new MockHandler();

            var handlersCount = gameController.GetHandlersCount();
            gameController.RegisterHandler(mockHandler);

            Assert.AreEqual(handlersCount + 1,gameController.GetHandlersCount());
        }

        [TestMethod]
        public void TestHandleViewArgsInControllerWithThreading_ExpectMasterActionSentToView()
        {

            // Create View Object
            MockGameView gameview = new MockGameView();

            // Create GameWorldId Object
            Guid gameModel = GetMockGameWorld();

            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController((IGameView) gameview);
            gameController.FocusGameWorld(gameModel);
            gameController.RegisterHandler(mockHandler);
            // Create the NotificationArgs
            string playerID = "AhmadHamdan";
            MockNotificationArgs mockNotificationArgs = new MockNotificationArgs(new Coordinate(12, 12, 3), playerID);
            System.Threading.Thread.CurrentThread.Name = "CurrentThread";
            IList<ThreadInfo> threads = gameController.Handle(mockNotificationArgs);

            foreach (var threadInfo in threads)
            {
                threadInfo.SyncEvent.WaitOne(300);
            }

            // check that only a single thread is executed
            Assert.AreEqual(1,threads.Count);
            Assert.AreEqual("AutomateTests.test.Mocks.MockHandler_HandleWorkerThread", threads[0].Thread.Name);
            Assert.AreNotEqual("AutomateTests.test.Mocks.MockHandler_HandleWorkerThread", Thread.CurrentThread.Name);
            

            Assert.AreEqual(2, gameController.OutputSched.ItemsCount);
            MasterAction masterAction1 = gameController.OutputSched.Pull();
            MasterAction masterAction2 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.AreaSelection, masterAction1.Type);
            Assert.AreEqual(ActionType.Movement, masterAction2.Type);
            Assert.AreEqual("AhmadHamdan",masterAction1.TargetId);
            Assert.AreEqual("NaphLevy",masterAction2.TargetId);

        }


       //[TestMethod]
        public void TestAllTheLoop_ACTION_TO_HANDLE_TO_SCHED_TO_TIMERSCHED_TO_ACK_EXPECtITworks()
        {

            // Create View Object
            MockGameView gameview = new MockGameView();

            // Create GameWorldId Object
            Guid gameModel = GetMockGameWorld();

            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController((IGameView) gameview);
            gameController.FocusGameWorld(gameModel);
            gameController.RegisterHandler(mockHandler);
            // Create the NotificationArgs

            // Perform First Update To Mimic The World Creation
            gameview.PerformCompleteUpdate();

            Assert.AreEqual(102, gameController.OutputSched.ItemsCount);
            for (int i = 0; i < 102; i++)
            {
                MasterAction placeObject = gameController.OutputSched.Pull();
                Assert.AreEqual(ActionType.PlaceGameObject, placeObject.Type);
            }

            string playerID = "AhmadHamdan";
            MockNotificationArgs mockNotificationArgs = new MockNotificationArgs(new Coordinate(12, 12, 3), playerID);
//            System.Threading.Thread.CurrentThread.Name = "CurrentThread";
            IList<ThreadInfo> threads = gameController.Handle(mockNotificationArgs);

            foreach (var threadInfo in threads)
            {
                threadInfo.SyncEvent.WaitOne(300);
            }

            // check that only a single thread is executed
            Assert.AreEqual(1, threads.Count);
            Assert.AreEqual("AutomateTests.test.Mocks.MockHandler_HandleWorkerThread", threads[0].Thread.Name);
            Assert.AreNotEqual("AutomateTests.test.Mocks.MockHandler_HandleWorkerThread", Thread.CurrentThread.Name);


            Assert.AreEqual(2, gameController.OutputSched.ItemsCount);
            MasterAction masterAction1 = gameController.OutputSched.Pull();
            MasterAction masterAction2 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.AreaSelection, masterAction1.Type);
            Assert.AreEqual(ActionType.Movement, masterAction2.Type);
            Assert.AreEqual("AhmadHamdan", masterAction1.TargetId);
            Assert.AreEqual("NaphLevy", masterAction2.TargetId);

            gameController.OutputSched.OnEnqueue += ActionsAddedtoSched;
            // mimic update from the view
            gameview.PerformOnUpdateStart();
            gameview.PerformOnUpdate();

            // wait till action being added to sched
            _ackSync.WaitOne(1000);
           
            MasterAction masterAction4 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.Movement, masterAction4.Type);
            Assert.AreEqual("NaphLevy_ACK", masterAction4.TargetId);
        }

        [TestMethod]
        public void TestHasFocusedWorld_ExpectFalse()
        {
            IGameController gameController = new GameController(new MockGameView());
            Assert.IsFalse(gameController.HasFocusedGameWorld);
        }

        [TestMethod]
        public void TestFocusGameWorld_ExpectFocusedToBeTrue()
        {
            IGameController gameController = new GameController(new MockGameView());
            gameController.FocusGameWorld(Guid.NewGuid());
            Assert.IsTrue(gameController.HasFocusedGameWorld);
        }

        [TestMethod]
        public void TestUnfocusGameWorld_ExpectFocusedToBeFalse()
        {
            IGameController gameController = new GameController(new MockGameView());
            var gameWorldId = Guid.NewGuid();
            gameController.FocusGameWorld(gameWorldId);
            Assert.IsTrue(gameController.HasFocusedGameWorld);
            Guid focusGameWorldId = gameController.UnfocusGameWorld();
            Assert.AreEqual(focusGameWorldId,gameWorldId);
            Assert.IsFalse(gameController.HasFocusedGameWorld);

        }

        private void ActionsAddedtoSched(MasterAction item)
        {
            // enque should be 
            _ackSync.Set();
        }
    }
}
