using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestScheduler
    {
        private AutoResetEvent _onPullSync = new AutoResetEvent(false);
        private AutoResetEvent _onEnqSync = new AutoResetEvent(false);
        private bool _listnerActivated = false;
        private bool _OnEnqueueFired = false;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            Assert.IsNotNull(scheduler);
        }

        [TestMethod]
        public void TestCount_expect0()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            Assert.AreEqual(0, scheduler.ItemsCount);

        }

        [TestMethod]
        public void TestEnqueJobsFromSingleThread_ExpectJobsinQ()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, Guid.Empty.ToString()));
            scheduler.Enqueue(actions);
            scheduler.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(1, scheduler.ItemsCount);
        }

        [TestMethod]
        public void TestHasActions_Expectfalse()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            Assert.IsFalse(scheduler.HasItems);
        }

        [TestMethod]
        public void TestHasActions_expectTrue()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, Guid.Empty.ToString()));
            scheduler.Enqueue(actions);
            scheduler.OnPullStart(new ViewUpdateArgs());
            Assert.IsTrue(scheduler.HasItems);
        }

        [TestMethod]
        public void TestPull_expectToGetTheSameActionAdded()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, Guid.Empty.ToString()));
            actions.Add(new MockMasterAction(ActionType.Movement, Guid.Empty.ToString()));
            scheduler.Enqueue(actions);
            scheduler.OnPullStart(new ViewUpdateArgs());
            MasterAction action = scheduler.Pull();
            Assert.IsNotNull(action);
            Assert.AreEqual(ActionType.AreaSelection,action.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestPullOnEmptryQ_expectNull()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            MasterAction action = scheduler.Pull();
            Assert.IsNull(action);
        }

        [TestMethod]
        public void TestPushActionsEvent_ExpectActionsToBeAdded()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, Guid.Empty.ToString()));
            actions.Add(new MockMasterAction(ActionType.Movement, Guid.Empty.ToString()));
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            Thread.Sleep(100);
            scheduler.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(2, scheduler.ItemsCount);
            MasterAction action = scheduler.Pull();
            Assert.AreEqual(1, scheduler.ItemsCount);
            Assert.IsNotNull(action);
            Assert.AreEqual(ActionType.AreaSelection, action.Type);
        }


        [TestMethod]
        public void TestOurImplToThreadSafeLocksForPullAndPush_ExpectCorrectBehaviour()
        {

            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            var ids = new Dictionary<int, Guid>();
            for (int i = 0; i < 50; i++)
            {
                ids.Add(i,Guid.NewGuid());
                actions.Add(new MockMasterAction(ActionType.Movement, ids[i].ToString()));
            }
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            
            Thread.Sleep(20);
            //Assert.AreEqual(50, scheduler.ItemsCount);
            scheduler.OnPullStart(new ViewUpdateArgs());
            for (int i = 0; i < 50; i++)
            {
                MasterAction action = scheduler.Pull();
                Assert.AreEqual(ActionType.Movement, action.Type);
                Assert.AreEqual(ids[i],action.TargetId);
            }


        }


        //TODO: i think we should add another event to notify that jobs added
       // [TestMethod]
        public void TestAcknowledgeToTimerAndQueueWhenPullOccurs_ExpectAckSentToHandle()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.Movement, Guid.Empty.ToString()));
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            Thread.Sleep(100);
            Assert.AreEqual(1, scheduler.ItemsCount);
            MasterAction action = scheduler.Pull();
            Assert.AreEqual(ActionType.Movement,action.Type);
            // by doing pull, we expect an action is added to Wait/Timer and send to Handler
            Thread.Sleep(300);
            Assert.AreEqual(1, scheduler.ItemsCount);
            MasterAction action2 = scheduler.Pull();
            Assert.AreEqual(ActionType.Movement, action2.Type);

        }

        [TestMethod]
        public void TestOnPullEvent_ExpectSniffMethodToBeActivated()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            scheduler.OnPull += PullListner;
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.Movement, Guid.Empty.ToString()));
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            Thread.Sleep(100);
            scheduler.OnPullStart(new ViewUpdateArgs());
            Assert.AreEqual(1, scheduler.ItemsCount);
            MasterAction action = scheduler.Pull();
            Assert.AreEqual(ActionType.Movement, action.Type);
            _onPullSync.WaitOne(200);
            scheduler.OnPullStart(new ViewUpdateArgs());
            Assert.IsTrue(_listnerActivated);
        }

        [TestMethod]
        public void TestOnAddEvent_ExpectItTOHappen()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            scheduler.OnEnqueue += ConfirmEnqueue;

            scheduler.Enqueue(new MasterAction(ActionType.AreaSelection));

            // mimic OnPullFinish Event -- scheduler will copy items at staging area to the final Q
            scheduler.OnPullStart(new ViewUpdateArgs());
            scheduler.OnPullFinish(new ViewUpdateArgs());

            _onEnqSync.WaitOne(100);
            Assert.IsTrue(_OnEnqueueFired);
        }

        private void ConfirmEnqueue(MasterAction item)
        {
            _OnEnqueueFired = true;
            _onEnqSync.Set();
        }

        private void PullListner(MasterAction item)
        {
            Assert.AreEqual(Guid.Empty, item.TargetId);
            _listnerActivated = true;
            _onPullSync.Set();
        }

        
    }
}
