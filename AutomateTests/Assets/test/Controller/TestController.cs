using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model;
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

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IGameView view = new MockGameView();
            IModelAbstractionLayer model = new MockGameModel();

            GameController gameController = new GameController(view, model);
            Assert.IsNotNull(gameController);
            Assert.IsNotNull(gameController.View);
            Assert.IsNotNull(gameController.Model);
        }

        [TestMethod]
        public void TestHandlersCount_Expect0()
        {
            IModelAbstractionLayer gameModel = new MockGameModel();
            IGameView gameview = new MockGameView();
            IGameController gameController = new GameController(gameview, gameModel);

            Assert.IsTrue(gameController.GetHandlersCount() > 0);

        }

        [TestMethod]
        public void TestRegisterHandle_ExpectHandleToBeAddedtoList()
        {
            IModelAbstractionLayer gameModel = new MockGameModel();
            IGameView gameview = new MockGameView();
            IGameController gameController = new GameController(gameview, gameModel);
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

            // Create Model Object
            IModelAbstractionLayer gameModel = new MockGameModel();

            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController(gameview, gameModel);
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

        private void CheckTestHandleViewArgsResult_ExpectActiontoBeAdded(IHandlerResult<MasterAction> handlerResult)
        {
            // update concurent que
            foreach (var masterAction in handlerResult.GetItems())
            {
                _queue.Enqueue(masterAction);
            }

        }

        [TestMethod]
        public void TestAllTheLoop_ACTION_TO_HANDLE_TO_SCHED_TO_TIMERSCHED_TO_ACK_EXPECtITworks()
        {

            // Create View Object
            MockGameView gameview = new MockGameView();

            // Create Model Object
            IModelAbstractionLayer gameModel = new MockGameModel();

            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController(gameview, gameModel);
            gameController.RegisterHandler(mockHandler);
            // Create the NotificationArgs
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

            // mimic update from the view
            gameview.PerformUpdate();

            int retry = 0;
            while (gameController.OutputSched.ItemsCount != 2 && retry < 3)
            {
                Thread.Sleep(2000);
                retry++;
            }
            
            // NOW I WILL PULL AGAIN FROM SCHED - it SHOULD HAS SOME ACTIONS WHICH Resultd from the 
            // Ack
            Assert.AreEqual(2, gameController.OutputSched.ItemsCount);
            MasterAction masterAction3 = gameController.OutputSched.Pull();
            MasterAction masterAction4 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.AreaSelection, masterAction3.Type);
            Assert.AreEqual(ActionType.Movement, masterAction4.Type);
            Assert.AreEqual("AhmadHamdan_ACK", masterAction3.TargetId);
            Assert.AreEqual("NaphLevy_ACK", masterAction4.TargetId);


        }

    }
}
