using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.src.Abstracts;
using Automate.Controller.src.Interfaces;
using Automate.Controller.src.Modules;
using Automate.Model.src;
using Automate.Model.src.MapModelComponents;
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
            IModelAbstractionLayer gameModel = null;
            IGameView gameview = null;
            IGameController gameController = new GameController(gameview, gameModel);

            Assert.AreEqual(0,gameController.GetHandlersCount());

        }

        [TestMethod]
        public void TestRegisterHandle_ExpectHandleToBeAddedtoList()
        {
            IModelAbstractionLayer gameModel = null;
            IGameView gameview = null;
            IGameController gameController = new GameController(gameview, gameModel);
            var mockHandler = new MockHandler();
            gameController.RegisterHandler(mockHandler);

            Assert.AreEqual(1,gameController.GetHandlersCount());
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
            var viewCallBack = gameview.GetCallBack();
            viewCallBack += CheckTestHandleViewArgsResult_ExpectActiontoBeAdded;
            System.Threading.Thread.CurrentThread.Name = "CurrentThread";
            IList<ThreadInfo> threads = gameController.Handle(mockNotificationArgs, viewCallBack);

            foreach (var threadInfo in threads)
            {
                threadInfo.SyncEvent.WaitOne(100);
            }

            // check that only a single thread is executed
            Assert.AreEqual(1,threads.Count);
            Assert.AreEqual("AutomateTests.test.Mocks.MockHandler_WorkerThread", threads[0].Thread.Name);
            Assert.AreNotEqual("AutomateTests.test.Mocks.MockHandler_WorkerThread", Thread.CurrentThread.Name);
            

            Assert.AreEqual(2, _queue.Count);
            MasterAction masterAction1 = null;
            MasterAction masterAction2 = null;
            _queue.TryDequeue(out masterAction1);
            _queue.TryDequeue(out masterAction2);
            Assert.AreEqual(ActionType.AreaSelection, masterAction1.Type);
            Assert.AreEqual(ActionType.Movement, masterAction2.Type);
            Assert.AreEqual("AhmadHamdan",masterAction1.TargetId);
            Assert.AreEqual("NaphLevy",masterAction2.TargetId);

        }

        private void CheckTestHandleViewArgsResult_ExpectActiontoBeAdded(IHandlerResult handlerResult)
        {
            // update concurent que
            foreach (var masterAction in handlerResult.GetActions())
            {
                _queue.Enqueue(masterAction);
            }

        }
    }
}
