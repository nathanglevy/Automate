using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Tasks;
using AutomateTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestController
    {
        private AutoResetEvent _ackSync = new AutoResetEvent(false);
        private int THREAD_TIMEOUT_VALUE = 1000;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IGameView view = new MockGameView();
            Guid model = GetMockGameWorld();

            GameController gameController = new GameController(view);
            gameController.FocusGameWorld(model);
            Assert.IsNotNull(gameController);
            Assert.IsNotNull(gameController.View);
            Assert.IsNotNull(gameController.GameWorldGuid);
        }

        private Guid GetMockGameWorld()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
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

            Assert.AreEqual(handlersCount + 1, gameController.GetHandlersCount());
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
            IGameController gameController = new GameController((IGameView)gameview);
            gameController.FocusGameWorld(gameModel);
            gameController.RegisterHandler(mockHandler);
            // Create the NotificationArgs
            Guid playerID = Guid.NewGuid();
            MockNotificationArgs mockNotificationArgs = new MockNotificationArgs(new Coordinate(12, 12, 3), playerID.ToString());
            System.Threading.Thread.CurrentThread.Name = "CurrentThread";
            IList<ThreadInfo> threads = gameController.Handle(mockNotificationArgs);

            foreach (var threadInfo in threads)
            {
                threadInfo.SyncEvent.WaitOne(300);
            }

            // check that only a single thread is executed
            Assert.AreEqual(1, threads.Count);
            Assert.AreEqual("AutomateTests.Mocks.MockHandler_HandleWorkerThread", threads[0].Thread.Name);
            Assert.AreNotEqual("AutomateTests.Mocks.MockHandler_HandleWorkerThread", Thread.CurrentThread.Name);

            gameController.OutputSched.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(2, gameController.OutputSched.ItemsCount);
            MasterAction masterAction1 = gameController.OutputSched.Pull();
            MasterAction masterAction2 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.AreaSelection, masterAction1.Type);
            Assert.AreEqual(ActionType.Movement, masterAction2.Type);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000001"), masterAction1.TargetId);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000002"), masterAction2.TargetId);

        }

        private Guid _onPreHandleGuid;
        private Guid _onPostHandleGuid;
        private Guid _onFinishHandleGuid;
        private bool _taskCompleteFired;

        [TestMethod]
        public void TestPreAndPostHandleEventsInvoke_ExpectEventsToFiredInCorrectOrder()
        {

            // Create View Object
            MockGameView gameview = new MockGameView();

            // Create GameWorldId Object
            Guid gameModel = GetMockGameWorld();

            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController((IGameView)gameview);
            gameController.FocusGameWorld(gameModel);
            gameController.RegisterHandler(mockHandler);


            gameController.OnPreHandle += PreHandle;
            gameController.OnPostHandle += PostHandle;
            gameController.OnFinishHandle += FinishHandle;


            // Create the NotificationArgs
            Guid playerID = Guid.NewGuid();
            MockNotificationArgs mockNotificationArgs = new MockNotificationArgs(new Coordinate(12, 12, 3), playerID.ToString());
            IList<ThreadInfo> threads = gameController.Handle(mockNotificationArgs);

            foreach (var threadInfo in threads)
            {
                threadInfo.SyncEvent.WaitOne(300);
            }

            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000001"),_onPreHandleGuid);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000002"),_onPostHandleGuid);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000003"),_onFinishHandleGuid);


        }

        private void FinishHandle(ControllerNotificationArgs controllerNotificationArgs)
        {
            var mockNotificationArgs = controllerNotificationArgs.Args as MockNotificationArgs;
            mockNotificationArgs.EventGuid = new Guid("00000000-0000-0000-0000-000000000003");
            _onFinishHandleGuid = mockNotificationArgs.EventGuid;
        }

        private void PostHandle(ControllerNotificationArgs controllerNotificationArgs)
        {
            var mockNotificationArgs = controllerNotificationArgs.Args as MockNotificationArgs;
            mockNotificationArgs.EventGuid = new Guid("00000000-0000-0000-0000-000000000002");
            _onPostHandleGuid = mockNotificationArgs.EventGuid;
        }

        private void PreHandle(ControllerNotificationArgs controllerNotificationArgs)
        {
            var mockNotificationArgs = controllerNotificationArgs.Args as MockNotificationArgs;
            mockNotificationArgs.EventGuid = new Guid("00000000-0000-0000-0000-000000000001");
            _onPreHandleGuid = mockNotificationArgs.EventGuid;
        }


        [TestMethod]
        public void TestAllTheLoop_ACTION_TO_HANDLE_TO_SCHED_TO_TIMERSCHED_TO_ACK_EXPECtITworks()
        {

            // Create View Object
            var gameview = new GameViewBase();

            // Create GameWorldId Object
            Guid gameModel = GetMockGameWorld();
            var gameWorldItem = GameUniverse.GetGameWorldItemById(gameModel);
            gameWorldItem.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);
            gameWorldItem.CreateMovable(new Coordinate(7, 7, 0), MovableType.NormalHuman);
            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController(gameview);
            gameController.FocusGameWorld(gameModel);
            gameController.RegisterHandler(mockHandler);
            // Create the NotificationArgs

            // Perform First Update To Mimic The World Creation
            //            gameview.PerformCompleteUpdate();
            gameview.PerformOnUpdateStart();
            gameview.PerformOnUpdate();

            gameController.OutputSched.OnPullStart(new ViewUpdateArgs());
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
                threadInfo.SyncEvent.WaitOne(400);
            }

            // check that only a single thread is executed
            Assert.AreEqual(1, threads.Count);
            Assert.AreEqual("AutomateTests.Mocks.MockHandler_HandleWorkerThread", threads[0].Thread.Name);
            Assert.AreNotEqual("AutomateTests.Mocks.MockHandler_HandleWorkerThread", Thread.CurrentThread.Name);

            gameController.OutputSched.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(2, gameController.OutputSched.ItemsCount);
            MasterAction masterAction1 = gameController.OutputSched.Pull();
            MasterAction masterAction2 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.AreaSelection, masterAction1.Type);
            Assert.AreEqual(ActionType.Movement, masterAction2.Type);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000001"), masterAction1.TargetId);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000002"), masterAction2.TargetId);

            gameController.OutputSched.OnEnqueue += ActionsAddedtoSched;
            // mimic update from the view
            gameview.PerformOnUpdateStart();
            gameview.PerformOnUpdate();

            gameController.OutputSched.OnPullStart(new ViewUpdateArgs());
             MasterAction masterAction4 = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.Movement, masterAction4.Type);
            Assert.AreEqual(new Guid("00000000-0000-0000-0000-000000000002"), masterAction4.TargetId);
            Assert.IsTrue(masterAction4.IsActionHasOver);

        }

      //  [TestMethod]
        public void TestCalcAPathAndChangeItInTheMiddle_ExpectTheMoveActionsToContinuetoNewTarget()
        {

            // Create View Object
            GameViewBase gameview = new GameViewBase();


            // Create GameWorldId Object
            Guid gameModel = GetMockGameWorld();

            // Create Handler
            var mockHandler = new MockHandler();

            // Init the Controller
            IGameController gameController = new GameController((IGameView)gameview);
            gameController.FocusGameWorld(gameModel);
            gameController.RegisterHandler(mockHandler);
            // Create the NotificationArgs

            // Perform First Update To Mimic The World Creation
            //            gameview.PerformCompleteUpdate();
            PartialUpdate(gameview);

            Assert.AreEqual(100, gameController.OutputSched.ItemsCount);
            for (int i = 0; i < 100; i++)
            {
                MasterAction placeObject = gameController.OutputSched.Pull();
                Assert.AreEqual(ActionType.PlaceGameObject, placeObject.Type);
            }
            // pull should remove everything from the Q
            Assert.AreEqual(0, gameController.OutputSched.ItemsCount);

            // Now Create & Select a Movable
            var gameWorldItem = GameUniverse.GetGameWorldItemById(gameModel);
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(0, 0, 0), MovableType.NormalHuman);

            PartialUpdate(gameview);

            // Now send SelectionNotification
            var selectPlayer = new ViewSelectionNotification(new Coordinate(0, 0, 0), new Coordinate(0, 0, 0), movableItem.Guid.ToString());
            var threadInfos = gameController.Handle(selectPlayer);
            WaitForThreads(threadInfos);

            ////////// NOW Need To Do Update To Get Actions - expected the movable and the SelectPlayer
            PartialUpdate(gameview);
            Assert.AreEqual(2, gameController.OutputSched.ItemsCount);

            MasterAction PlaceMovable = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.PlaceGameObject, PlaceMovable.Type);

            MasterAction selectMovable = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.SelectPlayer, selectMovable.Type);

            // pull should remove everything from the Q
            Assert.AreEqual(0, gameController.OutputSched.ItemsCount);


            /////////// NOW WE will do Right Click Notification 
            var firstTargetInfo = new RightClickNotification(new Coordinate(9, 9, 0));
            var secondTargetInfo = new RightClickNotification(new Coordinate(1, 9, 0));
            var firstTargetThreads = gameController.Handle(firstTargetInfo);
            WaitForThreads(firstTargetThreads);

            // update to start 
            PartialUpdate(gameview); // at this update the MoveCommand Will propogate to Sched
            Assert.AreEqual(1, gameController.OutputSched.ItemsCount);
            MasterAction moveToFirstTarget = gameController.OutputSched.Pull();
            Assert.AreEqual(ActionType.Movement, moveToFirstTarget.Type);

            // wait some time and Handle Again
            Thread.Sleep(100);
            var secondTargetThreads = gameController.Handle(secondTargetInfo);

            PartialUpdate(gameview);
            Assert.AreEqual(0, gameController.OutputSched.ItemsCount);



            string playerID1 = "00000000-0000-0000-0000-000000000001";
            string playerID2 = "00000000-0000-0000-0000-000000000002";
            MockNotificationArgs mockNotificationArgs = new MockNotificationArgs(new Coordinate(12, 12, 3), playerID1);
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
            Assert.AreEqual(new Guid(playerID1), masterAction1.TargetId);
            Assert.AreEqual(new Guid(playerID2), masterAction2.TargetId);

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

        private void WaitForThreads(IList<ThreadInfo> threadInfos)
        {
            foreach (var threadInfo in threadInfos)
            {
                threadInfo.SyncEvent.WaitOne(THREAD_TIMEOUT_VALUE);
            }
        }

        private static void PartialUpdate(GameViewBase gameview)
        {
            gameview.PerformOnUpdateStart();
            gameview.PerformOnUpdate();
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
            Assert.AreEqual(focusGameWorldId, gameWorldId);
            Assert.IsFalse(gameController.HasFocusedGameWorld);

        }

        private void ActionsAddedtoSched(MasterAction item)
        {
            // enque should be 
            _ackSync.Set();
        }

        [TestMethod]
        public void TestTaskAndSubAction_ExpectActionsToBeExecutedOneByOneAndOnCompleteFired()
        {
            IGameView gameWorldBase = new GameViewBase();
            IGameController gameController = new GameController(gameWorldBase) {MultiThreaded = false};

            // mimic on Start Method to create the world
            gameWorldBase.PerformOnStart(new Coordinate(20, 20, 1));

            var gameWorld = GameUniverse.GetGameWorldItemById(gameController.GameWorldGuid);

            // create Pickup Component Stack
            var cmpntGrp330 = gameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(3, 3, 0));
            var ironat330 = cmpntGrp330.AddComponentStack(ComponentType.IronOre, 50);

            // create Delivery Component Stack
            var cmpntGrp000 = gameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(0, 0, 0));
            var ironat000 = cmpntGrp000.AddComponentStack(ComponentType.IronOre, 0);

            // create Delivery Component Stack
            var cmpntGrp220 = gameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(2, 2, 0));
            var ironat220 = cmpntGrp220.AddComponentStack(ComponentType.IronOre, 0);




            // Create the Task and Actions
            var PickupAndDeliverTask = gameWorld.TaskDelegator.CreateNewTask();
            PickupAndDeliverTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(3, 3, 0), cmpntGrp330,
                Component.IronOre, 40);
            PickupAndDeliverTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(0, 0, 0), cmpntGrp000,
                Component.IronOre, 30);
            PickupAndDeliverTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(2, 2, 0), cmpntGrp000,
                Component.IronOre, 10);

            // Create the movable and Assign the task
            var movableItem = gameWorld.CreateMovable(new Coordinate(6, 3, 0), MovableType.NormalHuman);
            gameWorld.TaskDelegator.AssignTask(movableItem.Guid, PickupAndDeliverTask);

            gameWorldBase.PerformOnUpdateStart();
            // Handle all actions from GameWorldGuid
            gameWorldBase.PerformOnUpdate();

            // Move from Push to Pull Q
            gameWorldBase.PerformOnUpdateStart();

            gameWorldBase.PerformOnUpdate();

            for (int i = 0; i < 401; i++)
            {
                gameController.OutputSched.Pull();
            }


            // handle the Task
            var taskContainer = new TaskContainer(PickupAndDeliverTask) {OnCompleteDelegate = TaskonCompleteFired};
            var threadInfos = gameController.Handle(taskContainer);

            // Check that Something being executed
            //   Assert.IsTrue(threadInfos.Count > 0);


            // Wait till it finish
            //foreach (var threadInfo in threadInfos)
            //{
            //    threadInfo.SyncEvent.WaitOne();
            //}

            // 6,3,0 --> 5,3,0
            // 5,3,0 --> 4,3,0
            // 4,3,0 --> 3,3,0
            // 3,3,0 --> 3,3,0
            // PickupAction
            // 3,3,0 --> 2,2,0
            // 2,2,0 --> 1,1,0
            // 1,1,0 --> 0,0,0
            // 0,0,0 --> 0,0,0
            // DeliverAction

            gameWorldBase.PerformOnUpdateStart();
            gameWorldBase.PerformOnUpdate();

            List<MasterAction> actions = new List<MasterAction>();
            var index = 0;
            while (!PickupAndDeliverTask.IsTaskComplete() )
            {
                index++;
                gameWorldBase.PerformOnUpdateStart();
                gameWorldBase.PerformOnUpdate();

                while (gameController.OutputSched.HasItems)
                {
                    var action = gameController.OutputSched.Pull();
                    actions.Add(action);
           

                }
                gameWorldBase.PerformOnUpdateStart();
                gameWorldBase.PerformOnUpdate();
            }



            //threadInfos = gameController.Handle(action);
            //foreach (var threadInfo in threadInfos)
            //{
            //    threadInfo.SyncEvent.WaitOne();
            //}


            //Assert.IsTrue(action is MoveAction);
            //Assert.AreEqual(new Coordinate(5, 3, 0), (action as MoveAction).To);
            // First Action


            // Second Action
            //var moveAction0Result = gameController.Handle(moveAction0);


            Assert.AreEqual(new Coordinate(2, 2, 0), gameWorld.GetMovable(movableItem.Guid).CurrentCoordinate);
            Assert.IsTrue(_taskCompleteFired);
            Assert.AreEqual(10, ironat330.CurrentAmount);
            Assert.AreEqual(30, ironat000.CurrentAmount);
            Assert.AreEqual(10, ironat220.CurrentAmount);

        }

        private void TaskonCompleteFired(ControllerNotificationArgs args)
        {
            _taskCompleteFired = true;
        }
    }
}
