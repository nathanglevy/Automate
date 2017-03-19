using System.Collections.Generic;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestSchedular
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var target = new PrimaryObserver();
            IScheduler scheduler = new ControllerScheduler(target);
            Assert.IsNotNull(scheduler);
        }

        [TestMethod]
        public void TestGetActionsCount_Expect0()
        {
            var target = new PrimaryObserver();
            IScheduler scheduler = new ControllerScheduler(target);
            Assert.AreEqual(0,scheduler.GetActionsCount());
        }

        [TestMethod]
        public void TestGetObservable_ExpectNotNull()
        {
            var target = new PrimaryObserver();
            IScheduler scheduler = new ControllerScheduler(target);
            Assert.IsNotNull(scheduler.getTarget());
        }


        [TestMethod]
        public void TestSched3Actions_Expect3InCount()
        {
            IPrimaryObserver target = new PrimaryObserver();
            IScheduler scheduler = new ControllerScheduler(target);
            var actions = new List<MasterAction>();
            actions.Add(new MasterTestMasterAction(ActionType.AreaSelection));
            actions.Add(new MasterTestMasterAction(ActionType.AreaSelection));
            actions.Add(new MasterTestMasterAction(ActionType.AreaSelection));
            scheduler.Schedule(actions);
            Assert.AreEqual(3, scheduler.GetActionsCount());
        }


    }

}
