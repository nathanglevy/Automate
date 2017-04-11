using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestTimeSchuduler
    {

        private int _diffrentKeysSniffCounter = 0;
        private int _sameIdSniffer = 0;
        private event TimedOut<MasterAction> handlerDelegate;
        private const int INTERVAL = 100;
        private readonly Queue<MasterAction> _testingSameIDSniffOrder = new Queue<MasterAction>();
        private readonly Queue<MasterAction> _testingDiffIDSniffOrder = new Queue<MasterAction>();

        [TestMethod]
        public void TestCreateNew_shouldPass()
        {
            ITimerScheduler<MasterAction> timerScheduler = new TimersSchedular<MasterAction>(handlerDelegate);
            Assert.IsNotNull(timerScheduler);

        }


        [TestMethod]
        public void GetActionsCount_ExpectZero()
        {
            ITimerScheduler<MasterAction> timerScheduler = new TimersSchedular<MasterAction>(handlerDelegate);
            Assert.AreEqual(0, timerScheduler.ItemsCount);
        }

        [TestMethod]
        public void AddActions_ExpectNonZero()
        {
            handlerDelegate += TestAckSniff;

            ITimerScheduler<MasterAction> timerScheduler = new TimersSchedular<MasterAction>(handlerDelegate);

            // start adding actions
            timerScheduler.Enqueue(new DateTime(System.DateTime.Now.Ticks + INTERVAL),
                new MasterAction(ActionType.Movement));
            timerScheduler.Enqueue(new DateTime(System.DateTime.Now.Ticks + 3 * INTERVAL + 5),
                new MasterAction(ActionType.AreaSelection));
            timerScheduler.Enqueue(new DateTime(System.DateTime.Now.Ticks + 3 * INTERVAL + 10),
                new MasterAction(ActionType.AreaSelection));
            timerScheduler.Enqueue(new DateTime(System.DateTime.Now.Ticks + 6 * INTERVAL),
                new MasterAction(ActionType.SelectPlayer));

            Assert.AreEqual(4, timerScheduler.ItemsCount);
        }


        [TestMethod]
        public void AddActionsWithDiffrentDelays_ExpectAckByOrder()
        {
            _diffrentKeysSniffCounter = 0;

            handlerDelegate = null;
            handlerDelegate += TestAckSniff;
            ITimerScheduler<MasterAction> timerScheduler = new TimersSchedular<MasterAction>(handlerDelegate);

            // start adding actions
            var intervalInTicks = new TimeSpan(0, 0, 0, 0, INTERVAL).Ticks;

            // in 100 ms
            var action1 = new MasterAction(ActionType.Movement, "ID" + 1);
            var action2 = new MasterAction(ActionType.AreaSelection, "ID" + 2);
            var action3 = new MasterAction(ActionType.SelectPlayer, "ID" + 3);
            var action4 = new MasterAction(ActionType.Movement, "ID" + 4);

            _testingDiffIDSniffOrder.Enqueue(action1);
            _testingDiffIDSniffOrder.Enqueue(action2);
            _testingDiffIDSniffOrder.Enqueue(action3);
            _testingDiffIDSniffOrder.Enqueue(action4);

            timerScheduler.Enqueue(DateTime.Now.Add(new TimeSpan(intervalInTicks)), action1);
            timerScheduler.Enqueue(DateTime.Now.Add(new TimeSpan(3 * intervalInTicks)), action2);
            timerScheduler.Enqueue(DateTime.Now.Add(new TimeSpan(4 * intervalInTicks + 1)), action3);
            timerScheduler.Enqueue(DateTime.Now.Add(new TimeSpan(6 * intervalInTicks)), action4);

            var timer1 = new Timer(FireUpdate,
                new UpdateMimicArgs() {Sched = timerScheduler, Queue = _testingSameIDSniffOrder}, 0, INTERVAL);


            Thread.Sleep(10 * INTERVAL);
            Assert.AreEqual(0, timerScheduler.ItemsCount);
            Assert.AreEqual(4, _diffrentKeysSniffCounter);

        }


        [TestMethod]
        public void AddActionsWithSameTimeOutDelay_ExpectAckByOrderAndNoException()
        {
            handlerDelegate = null;
            handlerDelegate += TestSameAckSniff;
            ITimerScheduler<MasterAction> timerScheduler = new TimersSchedular<MasterAction>(handlerDelegate);

            // start adding actions
            var intervalInTicks = new TimeSpan(0, 0, 0, 0, INTERVAL).Ticks;

            // in 100 ms
            MasterAction action1 = new MasterAction(ActionType.Movement);
            MasterAction action2 = new MasterAction(ActionType.SelectPlayer);
            timerScheduler.Enqueue(DateTime.Now.Add(new TimeSpan(intervalInTicks)), action1);
            timerScheduler.Enqueue(DateTime.Now.Add(new TimeSpan(intervalInTicks)), action2);

            _testingSameIDSniffOrder.Enqueue(action1);
            _testingSameIDSniffOrder.Enqueue(action2);

            var timer1 = new Timer
            (FireUpdate,
                new UpdateMimicArgs() {Sched = timerScheduler, Queue = _testingSameIDSniffOrder}, 0, INTERVAL);


            Thread.Sleep(2 * INTERVAL);
            Assert.AreEqual(0, timerScheduler.ItemsCount);
            Assert.AreEqual(2, _sameIdSniffer);

        }

/*        [TestMethod]
        public void TestPushToTimerQ_ExpectCounterToIncrease()
        {
            handlerDelegate = null;
            handlerDelegate += TestSameAckSniff;
            ITimerScheduler<MasterAction> timerScheduler = new TimersSchedular<MasterAction>(handlerDelegate);

            timeUpdate += timerScheduler.Update;
            timerScheduler.GetEnqueueDelegate(
            var thread = new Thread()
            {
                delegate()
                {
                    for (int i = 0; i < 5; i++)
                    {
                        
                    }

                }
            };
        }*/

        // Mimic the View Update Method
        private void FireUpdate(object state)
        {
            UpdateMimicArgs args = (UpdateMimicArgs) state;
            var random = new Random();
            var randomNumber = random.Next(0, INTERVAL);
            args.Sched.Update(new TimerSchudulerUpdateArgs()
            {
                Time = System.DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, randomNumber))
            });
        }

        private IList<ThreadInfo> TestSameAckSniff(MasterAction args)
        {
            Console.Out.WriteLine("SAME ID SNIFF Activated: " + args.Type);
            _sameIdSniffer++;

            if (_testingSameIDSniffOrder.Count == 0)
                throw new Exception("Testing Q must be full, check you test");
            var masterAction = _testingSameIDSniffOrder.Dequeue();
            Assert.AreEqual(masterAction.TargetId, args.TargetId);
            Assert.AreEqual(masterAction.Type, args.Type);

            return null;
        }


        private IList<ThreadInfo> TestAckSniff(MasterAction args)
        {
            Console.Out.WriteLine("DIFF ID SNIFF Activated: " + args.Type);

            if (_testingDiffIDSniffOrder.Count == 0)
                throw new Exception("Testing Q must be full, check you test");

            var masterAction = _testingDiffIDSniffOrder.Dequeue();
            Assert.AreEqual(masterAction.TargetId, args.TargetId);
            Assert.AreEqual(masterAction.Type, args.Type);

            _diffrentKeysSniffCounter++;

            return null;
        }




    }

    internal class UpdateMimicArgs
    {
        public Queue<MasterAction> Queue { get; set; }
        public ITimerScheduler<MasterAction> Sched { get; set; }
    }


}
