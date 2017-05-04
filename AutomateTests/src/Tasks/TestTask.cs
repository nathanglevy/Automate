using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks.Tests {
    [TestClass()]
    public class TestTask
    {
        public ComponentStackGroup ComponentStackGroup;
        //ComponentStackGroup
        [TestInitialize]
        public void TestInit() {
            ComponentStackGroup = new ComponentStackGroup();
        }

        [TestMethod()]
        public void TestAddAction_ExpectSuccess() {
            Task newTask = new Task();
            newTask.AddAction(new Coordinate(0, 0, 0), 10);
        }

        [TestMethod()]
        public void TestAddTransportAction_ExpectSuccess() {
            Task newTask = new Task();
            newTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(0, 0, 0), ComponentStackGroup, Component.IronOre, 10);
        }

        [TestMethod()]
        public void TestGetCurrentAction_ExpectSuccess() {
            Task newTask = new Task();
            newTask.AddAction(new Coordinate(0, 0, 0), 10);
            newTask.GetCurrentAction();
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskActionException))]
        public void TestGetCurrentAction_NoActionsLeft_ExpectTaskActionException() {
            Task newTask = new Task();
            newTask.GetCurrentAction();
        }

        [TestMethod()]
        public void TestGetCurrentAction_ExpectCorrectValue() {
            Task newTask = new Task();
            newTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(1, 0, 1), ComponentStackGroup, Component.IronOre, 10);
            Assert.AreEqual(newTask.GetCurrentAction().TaskActionType,TaskActionType.PickupTask);
            Assert.AreEqual(newTask.GetCurrentAction().TaskLocation, new Coordinate(1, 0, 1));
            Assert.AreEqual(newTask.GetCurrentAction().Amount, 10);
        }

        [TestMethod()]
        public void TestIsPositionChangeRequiredForCurrentAction_ExpectCorrectValues()
        {
            Task newTask = new Task();
            //should be false always when no actions attached
            Assert.IsFalse(newTask.IsPositionChangeRequiredForCurrentAction(new Coordinate(0,0,0)));
            newTask.AddAction(new Coordinate(0,0,0), 10);
            newTask.AddAction(new Coordinate(1,1,1), 10);
            Assert.IsTrue(newTask.IsPositionChangeRequiredForCurrentAction(new Coordinate(1,1,1)));
            Assert.IsTrue(newTask.IsPositionChangeRequiredForCurrentAction(new Coordinate(2,1,2)));
            Assert.IsFalse(newTask.IsPositionChangeRequiredForCurrentAction(new Coordinate(0,0,0)));
        }

        [TestMethod()]
        public void TestMoveTaskToNextAction_ExpectSuccess() {
            Task newTask = new Task();
            newTask.AddAction(new Coordinate(0, 0, 0), 10);
            newTask.CommitActionAndMoveTaskToNextAction();
        }

        [TestMethod()]
        public void TestMoveTaskToNextAction_ExpectCorrectValue() {
            Task newTask = new Task();
            newTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(0, 0, 0), ComponentStackGroup, Component.IronOre, 10);
            newTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(2, 2, 0), ComponentStackGroup, Component.IronOre, 5);
            newTask.CommitActionAndMoveTaskToNextAction();
            Assert.AreEqual(newTask.GetCurrentAction().Amount, 5);
            Assert.AreEqual(newTask.GetCurrentAction().TaskActionType, TaskActionType.DeliveryTask);
            Assert.AreEqual(newTask.GetCurrentAction().TaskLocation, new Coordinate(2, 2, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskActionException))]
        public void TestMoveTaskToNextAction_NoActionsLeft_ExpectTaskActionException() {
            Task newTask = new Task();
            newTask.AddAction(new Coordinate(0, 0, 0), 10);
            newTask.CommitActionAndMoveTaskToNextAction();
            newTask.CommitActionAndMoveTaskToNextAction();
        }

        [TestMethod()]
        public void TestIsTaskComplete_ExpectSuccess() {
            Task newTask = new Task();
            Assert.IsTrue(newTask.IsTaskComplete());
            newTask.AddAction(new Coordinate(0, 0, 0), 10);
            Assert.IsFalse(newTask.IsTaskComplete());
            newTask.CommitActionAndMoveTaskToNextAction();
            Assert.IsTrue(newTask.IsTaskComplete());
        }

        [TestMethod()]
        public void TestEquals() {
            Task newTask = new Task();
            Assert.AreEqual(newTask,newTask);
        }
    }
}