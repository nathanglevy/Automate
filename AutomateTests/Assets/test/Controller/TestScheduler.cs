﻿using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using AutomateTests.test.Controller;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestScheduler
    {
        private AutoResetEvent _onPullSync = new AutoResetEvent(false);
        private bool _listnerActivated = false;

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
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId"));
            scheduler.Enqueue(actions);
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
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId"));
            scheduler.Enqueue(actions);
            Assert.IsTrue(scheduler.HasItems);
        }

        [TestMethod]
        public void TestPull_expectToGetTheSameActionAdded()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId1"));
            actions.Add(new MockMasterAction(ActionType.Movement, "MyId2"));
            scheduler.Enqueue(actions);
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
            actions.Add(new MockMasterAction(ActionType.AreaSelection, "MyId1"));
            actions.Add(new MockMasterAction(ActionType.Movement, "MyId2"));
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            Thread.Sleep(100);
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
            for (int i = 0; i < 50; i++)
            {
                actions.Add(new MockMasterAction(ActionType.Movement, "MyId" + i));
            }
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            
            Thread.Sleep(20);
            //Assert.AreEqual(50, scheduler.ItemsCount);
            for (int i = 0; i < 50; i++)
            {
                MasterAction action = scheduler.Pull();
                Assert.AreEqual(ActionType.Movement, action.Type);
                Assert.AreEqual("MyId" + i,action.TargetId);
            }


        }


        //TODO: i think we should add another event to notify that jobs added
       // [TestMethod]
        public void TestAcknowledgeToTimerAndQueueWhenPullOccurs_ExpectAckSentToHandle()
        {
            IScheduler<MasterAction> scheduler = new Scheduler<MasterAction>();
            List<MasterAction> actions = new List<MasterAction>();
            actions.Add(new MockMasterAction(ActionType.Movement, "MyId2"));
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
            actions.Add(new MockMasterAction(ActionType.Movement, "MyId2"));
            IHandlerResult<MasterAction> handlerResult = new HandlerResult(actions);

            HandlerResultListner<MasterAction> pusher = scheduler.GetPushInvoker();
            Assert.IsNotNull(pusher);
            pusher(handlerResult);
            Thread.Sleep(100);
            Assert.AreEqual(1, scheduler.ItemsCount);
            MasterAction action = scheduler.Pull();
            Assert.AreEqual(ActionType.Movement, action.Type);
            _onPullSync.WaitOne(200);
            Assert.IsTrue(_listnerActivated);
        }

        private void PullListner(MasterAction item)
        {
            Assert.AreEqual("MyId2", item.TargetId);
            _listnerActivated = true;
            _onPullSync.Set();
            

        }
    }
}
