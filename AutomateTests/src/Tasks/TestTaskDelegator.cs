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
        public void TestAddPendingTask_ExpectSuccess() {
            _taskDelegator.AddPendingTask(new PickupTask(new Coordinate(0, 0, 0), 10));
        }

        [TestMethod()]
        public void TestIsPendingTasksForDelegation() {
            Assert.IsFalse(_taskDelegator.IsPendingTasksForDelegation());
            _taskDelegator.AddPendingTask(new PickupTask(new Coordinate(0, 0, 0), 10));
            Assert.IsTrue(_taskDelegator.IsPendingTasksForDelegation());
        }

        [TestMethod()]
        public void TestAssignTask_ExpectSuccess() {
            _taskDelegator.AssignTask(Guid.NewGuid(), new PickupTask(new Coordinate(0, 0, 0), 10));
        }

        [TestMethod()]
        public void TestAssignTask_ExpectAssignment()
        {
            Task todo = new PickupTask(new Coordinate(0, 0, 0), 10);
            _taskDelegator.AddPendingTask(todo);
            Assert.IsTrue(_taskDelegator.IsPendingTasksForDelegation());
            _taskDelegator.AssignTask(Guid.NewGuid(), todo);
            Assert.IsFalse(_taskDelegator.IsPendingTasksForDelegation());
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskAssignmentException))]
        public void TestAssignTask_AssignTwice_ExpectTaskAssignmentException() {
            Task todo = new PickupTask(new Coordinate(0, 0, 0), 10);

            _taskDelegator.AssignTask(Guid.NewGuid(), todo);
            _taskDelegator.AssignTask(Guid.NewGuid(), todo);
        }

        [TestMethod()]
        public void TestGetDelegatedTasksForGuid_ExpectNotNull()
        {
            Assert.IsNotNull(Guid.NewGuid());
        }

        [TestMethod()]
        public void TestGetDelegatedTasksForGuid_ExpectCorrectValues() {
            Task todo = new PickupTask(new Coordinate(0, 0, 0), 10);
            Guid assignee = Guid.NewGuid();
            _taskDelegator.AddPendingTask(todo);
            _taskDelegator.AssignTask(assignee, todo);
            Assert.AreEqual(todo,_taskDelegator.GetDelegatedTasksForGuid(assignee).FirstOrDefault());
        }

        [TestMethod()]
        public void TestGetNextPendingTask() {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void TestGetNextPendingTask1() {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void TestGetNextDelegatedTaskForGuid() {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void TestGetNextDelegatedTaskForGuid1() {
            throw new NotImplementedException();
        }
    }
}