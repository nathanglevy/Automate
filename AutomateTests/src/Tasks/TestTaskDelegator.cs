using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks.Tests {
    [TestClass()]
    public class TestTaskDelegator
    {
        private TaskDelegator _taskDelegator;
        [TestInitialize]
        public void TestInit()
        {
            _taskDelegator = new TaskDelegator();
        }

        [TestMethod()]
        public void TestCreateNewTask_ExpectSuccess() {
            _taskDelegator.CreateNewTask();
        }

        [TestMethod()]
        public void TestIsPendingTasksForDelegation() {
            Assert.IsFalse(_taskDelegator.IsPendingTasksForDelegation());
            _taskDelegator.CreateNewTask();
            Assert.IsTrue(_taskDelegator.IsPendingTasksForDelegation());
        }

        [TestMethod()]
        public void TestAssignTask_ExpectSuccess() {
            Task newTask = _taskDelegator.CreateNewTask();
            _taskDelegator.AssignTask(Guid.NewGuid(), newTask);
        }

        [TestMethod()]
        public void TestAssignTask_ExpectAssignment()
        {
            Task newTask = _taskDelegator.CreateNewTask();
            Assert.IsTrue(_taskDelegator.IsPendingTasksForDelegation());
            _taskDelegator.AssignTask(Guid.NewGuid(), newTask);
            Assert.IsFalse(_taskDelegator.IsPendingTasksForDelegation());
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskAssignmentException))]
        public void TestAssignTask_AssignTwice_ExpectTaskAssignmentException() {
            Task newTask = _taskDelegator.CreateNewTask();
            _taskDelegator.AssignTask(Guid.NewGuid(), newTask);
            _taskDelegator.AssignTask(Guid.NewGuid(), newTask);
        }

        [TestMethod()]
        public void TestGetDelegatedTasksForGuid_ExpectNotNull()
        {
            Assert.IsNotNull(_taskDelegator.GetDelegatedTasksForGuid(Guid.NewGuid()));
        }

        [TestMethod()]
        public void TestGetDelegatedTasksForGuid_ExpectCorrectValues() {
            Task newTask = _taskDelegator.CreateNewTask();
            Guid assignee = Guid.NewGuid();
            _taskDelegator.AssignTask(assignee, newTask);
            Assert.AreEqual(newTask, _taskDelegator.GetDelegatedTasksForGuid(assignee).FirstOrDefault());
        }

        [TestMethod()]
        public void TestGetNextPendingTask_ExpectSuccess() {
            Task newTask = _taskDelegator.CreateNewTask();
            _taskDelegator.GetNextPendingTask();
        }

        [TestMethod()]
        [ExpectedException(typeof(NoTaskException))]
        public void TestGetNextPendingTask_NoTaskExisting_ExpectNoTaskException() {
            _taskDelegator.GetNextPendingTask();
        }

        [TestMethod()]
        public void TestGetNextPendingTask_ExpectCorrectValue() {
            Task newTask = _taskDelegator.CreateNewTask();
            Assert.AreEqual(_taskDelegator.GetNextPendingTask(), newTask);
        }

        [TestMethod()]
        public void TestGetNextDelegatedTaskForGuid_ExpectCorrectValue() {
            Task newTask = _taskDelegator.CreateNewTask();
            Guid assignee = Guid.NewGuid();
            _taskDelegator.AssignTask(assignee, newTask);
            Assert.AreEqual(_taskDelegator.GetNextDelegatedTaskForGuid(assignee),newTask);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoTaskException))]
        public void TestGetNextDelegatedTaskForGuid_NoTasks_ExpectNoTaskException() {
            Guid assignee = Guid.NewGuid();
            _taskDelegator.GetNextDelegatedTaskForGuid(assignee);
        }

        [TestMethod()]
        public void TestHasDelegatedTasks_ExpectCorrectValue() {
            Task newTask = _taskDelegator.CreateNewTask();
            Guid assignee = Guid.NewGuid();
            _taskDelegator.AssignTask(assignee, newTask);
            Assert.IsTrue(_taskDelegator.HasDelegatedTasks(assignee));
        }

        [TestMethod()]
        public void TestComplexTask_ExpectSuccess() {
            Task newTask = _taskDelegator.CreateNewTask();
            newTask.AddAction(TaskActionType.PickupTask, new Coordinate(0, 0, 0), 10);
            newTask.AddAction(TaskActionType.DeliveryTask, new Coordinate(1, 1, 1), 5);
            newTask.AddAction(TaskActionType.DeliveryTask, new Coordinate(1, 2, 1), 5);
            Guid assignee = Guid.NewGuid();
            _taskDelegator.AssignTask(assignee, newTask);
            Assert.IsTrue(_taskDelegator.HasDelegatedTasks(assignee));
            Assert.AreEqual(_taskDelegator.GetNextDelegatedTaskForGuid(assignee).GetCurrentAction().TaskActionType, TaskActionType.PickupTask);
            Assert.AreEqual(_taskDelegator.GetNextDelegatedTaskForGuid(assignee).GetCurrentAction().Amount, 10);
            _taskDelegator.GetNextDelegatedTaskForGuid(assignee).CommitActionAndMoveTaskToNextAction();
            Assert.AreEqual(_taskDelegator.GetNextDelegatedTaskForGuid(assignee).GetCurrentAction().TaskActionType, TaskActionType.DeliveryTask);
            Assert.AreEqual(_taskDelegator.GetNextDelegatedTaskForGuid(assignee).GetCurrentAction().Amount, 5);
            _taskDelegator.GetNextDelegatedTaskForGuid(assignee).CommitActionAndMoveTaskToNextAction();
            Assert.AreEqual(_taskDelegator.GetNextDelegatedTaskForGuid(assignee).GetCurrentAction().Amount, 5);
            _taskDelegator.GetNextDelegatedTaskForGuid(assignee).CommitActionAndMoveTaskToNextAction();
            Assert.IsTrue(_taskDelegator.GetNextDelegatedTaskForGuid(assignee).IsTaskComplete());
        }
    }
}