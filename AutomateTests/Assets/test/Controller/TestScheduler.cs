using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using AutomateTests.test.Controller;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestScheduler
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IScheduler scheduler = new Scheduler();
            Assert.IsNotNull(scheduler);
        }

        [TestMethod]
        public void TestCount_expect0()
        {
            IScheduler scheduler = new Scheduler();
            Assert.AreEqual(0, scheduler.ActionsCount);

        }

        [TestMethod]
        public void TestEnqueJobsFromSingleThread_ExpectJobsinQ()
        {
            IScheduler scheduler = new Scheduler();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId"));
            scheduler.Enqueue(actions);
            Assert.AreEqual(1, scheduler.ActionsCount);
        }

        [TestMethod]
        public void TestHasActions_Expectfalse()
        {
            IScheduler scheduler = new Scheduler();
            Assert.IsFalse(scheduler.HasActions);
        }

        [TestMethod]
        public void TestHasActions_expectTrue()
        {
            IScheduler scheduler = new Scheduler();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId"));
            scheduler.Enqueue(actions);
            Assert.IsTrue(scheduler.HasActions);
        }

        [TestMethod]
        public void TestPull_expectToGetTheSameActionAdded()
        {
            IScheduler scheduler = new Scheduler();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId1"));
            actions.Add(new MockMasterAction(ActionType.Movement, "MyId2"));
            scheduler.Enqueue(actions);
            MasterAction action = scheduler.Pull();
            Assert.IsNotNull(action);
            Assert.AreEqual(ActionType.AreaSelection,action.Type);
        }

        [TestMethod]
        public void TestPullOnEmptryQ_expectNull()
        {
            IScheduler scheduler = new Scheduler();
            MasterAction action = scheduler.Pull();
            Assert.IsNull(action);
        }

        [TestMethod]
        public void TestPushActionsEvent_ExpectActionsToBeAdded()
        {
            IScheduler scheduler = new Scheduler();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId1"));
            actions.Add(new MockMasterAction(ActionType.Movement, "MyId2"));
            IHandlerResult handlerResult = new HandlerResult(actions);

            HandlerResultListner pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            Assert.AreEqual(2, scheduler.ActionsCount);
            MasterAction action = scheduler.Pull();
            Assert.AreEqual(1, scheduler.ActionsCount);
            Assert.IsNotNull(action);
            Assert.AreEqual(ActionType.AreaSelection, action.Type);
        }

        //TODO: i think we should add another event to notify that jobs added

     
    }
}
