using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Modules;
using AutomateTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Controller
{
    [TestClass]
    public class TestTimersSched
    {
        private int _timedOutCount = 0;

        [TestMethod]
        public void TestEnqueue_ExpectItemToBeAdded()
        {
            var testTimersSched = new TimersScheduler<MasterAction>(OnTimeOut);
            MasterAction testingAction = new MockMasterAction(ActionType.Movement, Guid.Empty.ToString());
            var timeoutTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, 100));
            testTimersSched.Enqueue(timeoutTime,testingAction);
            Assert.AreEqual(1,testTimersSched.ItemsCount);
            Assert.IsTrue(testTimersSched.HasItems);
        }

        [TestMethod]
        public void TestEnqueueAndUpdate_ExpectItemToTimedOut()
        {
            var testTimersSched = new TimersScheduler<MasterAction>(OnTimeOut);
            MasterAction testingAction = new MockMasterAction(ActionType.Movement, Guid.Empty.ToString());
            var timeoutTime = DateTime.Now;
            testTimersSched.Enqueue(timeoutTime.Add(new TimeSpan(0, 0, 0, 0, 100)), testingAction);
            testTimersSched.Enqueue(timeoutTime.Add(new TimeSpan(0, 0, 0, 0, 500)), testingAction);
            Assert.AreEqual(2, testTimersSched.ItemsCount);
            Assert.IsTrue(testTimersSched.HasItems);

            testTimersSched.Update(new TimerSchudulerUpdateArgs() { Time = timeoutTime.Add(new TimeSpan(0, 0, 0, 0, 20)) });
            Assert.AreEqual(2, testTimersSched.ItemsCount);
            Assert.IsTrue(testTimersSched.HasItems);
            Assert.AreEqual(0,_timedOutCount);

            testTimersSched.Update(new TimerSchudulerUpdateArgs() { Time = timeoutTime.Add(new TimeSpan(0, 0, 0, 0, 200)) });
            Assert.AreEqual(1, testTimersSched.ItemsCount);
            Assert.IsTrue(testTimersSched.HasItems);
            Assert.AreEqual(1, _timedOutCount);

            testTimersSched.Update(new TimerSchudulerUpdateArgs() { Time = timeoutTime.Add(new TimeSpan(0, 0, 0, 0, 600)) });
            Assert.AreEqual(0, testTimersSched.ItemsCount);
            Assert.IsFalse(testTimersSched.HasItems);
            Assert.AreEqual(2, _timedOutCount);
        }


        private IList<ThreadInfo> OnTimeOut(MasterAction args)
        {
            _timedOutCount++;
            return null;
        }
    }
}
